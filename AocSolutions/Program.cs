using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AdventFileIO;
using AdventOfCode;

namespace PuzzleMain
{
    class Program
    {
        private static Dictionary<int, KeyValuePair<AdventOfCodeAttribute, IAdventOfCode>> SolutionCache;

        static int Main(string[] args)
        {
            bool verbose = args.Contains("--verbose");
            args = args.Where(argument => argument != "--verbose").ToArray();
            if (args.Length == 2 && args[0] == "--year")
                args = new[] { args[1], "all" };
            bool allYears = args.Length == 1 && args[0] == "--all";
            int year = 0;
            bool selectedYear = args.Length == 2 && int.TryParse(args[0], out year) && year >= 2015;
            bool allDays = selectedYear && args[1].Equals("all", StringComparison.OrdinalIgnoreCase);
            int day = 0;
            bool singleDay = selectedYear && int.TryParse(args[1], out day) && day >= 1 && day <= 25;

            if (!allYears && !allDays && !singleDay)
            {
                PrintUsage();
                return args.Length == 1 && args[0] == "--help" ? 0 : 2;
            }

            try
            {
                FileIOHelper.getInstance().Initialize();
                
                LoadSolutionCache();

                if (allYears || allDays)
                {
                    return RunAll(allYears ? null : year, verbose);
                }
                var solution = SolutionCache.Values.FirstOrDefault(candidate =>
                    candidate.Key.Year == year && candidate.Key.Day == day);
                if (solution.Value == null)
                {
                    Console.WriteLine("The selected year and day does not correlate to an Advent of Code Puzzle");
                    return 2;
                }

                Console.WriteLine($"Advent of Code {year} day {day:00}");
                solution.Value.GetSolution("daily.txt", true, new ConsolePuzzleOutput(true, verbose));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            return 0;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: AdventOfCode <year> <day> [--verbose]");
            Console.WriteLine("       AdventOfCode <year> all [--verbose]");
            Console.WriteLine("       AdventOfCode --year <year> [--verbose]");
            Console.WriteLine("       AdventOfCode --all [--verbose]");
            Console.WriteLine("Batch runs execute implemented puzzles in year/day order using daily.txt and any configured input overrides.");
        }

        private static int RunAll(int? year, bool verbose = false)
        {
            var solutions = SolutionCache.Values
                .Where(solution => !year.HasValue || solution.Key.Year == year.Value)
                .OrderBy(solution => solution.Key.Year)
                .ThenBy(solution => solution.Key.Day)
                .ToList();

            if (solutions.Count == 0)
            {
                Console.WriteLine("No implemented puzzles found for the selected year.");
                return 2;
            }

            ExpectedAnswers expectedAnswers = ExpectedAnswers.Load();
            const string rowFormat = "{0,4}  {1,3}  {2,4}  {3,-36}  {4,-32}  {5,-32}  {6,-8}  {7,9}  {8}";
            Console.WriteLine(rowFormat, "Year", "Day", "Part", "Description", "Computed", "Expected", "Check", "Run (s)", "Run status");
            Console.WriteLine(new string('=', 154));
            var details = new List<string>();
            var checks = new Dictionary<string, int>
            {
                ["Match"] = 0,
                ["Mismatch"] = 0,
                ["Missing"] = 0,
                ["Unknown"] = 0
            };
            bool showProgress = !Console.IsOutputRedirected && !Console.IsErrorRedirected;
            int failed = 0;
            int noAnswers = 0;
            foreach (var solution in solutions)
            {
                string progress = $"Running {solution.Key.Year} day {solution.Key.Day:00}...";
                if (showProgress)
                    Console.Error.Write(progress);
                var output = new CollectingPuzzleOutput(verbose);
                string status = "Completed";
                Exception error = null;
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    solution.Value.GetSolution("daily.txt", true, output);
                    if (output.Parts.Count == 0)
                    {
                        status = "No answers";
                        noAnswers++;
                    }
                }
                catch (Exception ex)
                {
                    failed++;
                    status = "Error";
                    error = ex;
                }
                finally
                {
                    stopwatch.Stop();
                    if (showProgress)
                        Console.Error.Write("\r" + new string(' ', progress.Length) + "\r");
                }

                string elapsed = stopwatch.Elapsed.TotalSeconds.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
                for (int part = 1; part <= 2; part++)
                {
                    output.Parts.TryGetValue(part, out PuzzlePart result);
                    string expected = expectedAnswers.GetAnswer(solution.Key.Year, solution.Key.Day, part);
                    string check = ExpectedAnswers.Reconcile(result?.Answer, expected);
                    checks[check]++;
                    string[] description = WrapDescription(result?.Description, 36).ToArray();
                    Console.WriteLine(rowFormat, solution.Key.Year, solution.Key.Day.ToString("00"), part,
                        description[0],
                        GetAnswerCell(result?.Answer, part, solution.Key, details),
                        GetAnswerCell(expected, part, solution.Key, details, true),
                        check, part == 1 ? elapsed : "", part == 1 ? status : "");
                    foreach (string line in description.Skip(1))
                        Console.WriteLine(rowFormat, "", "", "", line, "", "", "", "", "");
                }

                if (error != null)
                    details.Add($"{solution.Key.Year} day {solution.Key.Day:00} error\n{error}");
                if (output.Diagnostics.Count > 0)
                    details.Add($"{solution.Key.Year} day {solution.Key.Day:00} diagnostics\n{string.Join(Environment.NewLine, output.Diagnostics)}");
                Console.WriteLine(new string('-', 154));
            }

            Console.WriteLine($"\nRan {solutions.Count} puzzles. Completed {solutions.Count - failed - noAnswers}. No answers {noAnswers}. Errors {failed}.");
            Console.WriteLine($"Reconciliation by part. Matched {checks["Match"]}. Mismatched {checks["Mismatch"]}. Missing {checks["Missing"]}. Unknown {checks["Unknown"]}.");
            foreach (string detail in details)
            {
                Console.WriteLine();
                Console.WriteLine(detail);
            }
            return failed == 0 && checks["Mismatch"] == 0 && checks["Missing"] == 0 ? 0 : 1;
        }

        private static IEnumerable<string> WrapDescription(string description, int width)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                yield return "-";
                yield break;
            }

            foreach (string paragraph in description.Replace("\r\n", "\n").Replace('\r', '\n').Replace('\t', ' ').Split('\n'))
            {
                string remaining = paragraph.Trim();
                while (remaining.Length > width)
                {
                    int split = remaining.LastIndexOf(' ', width - 1, width);
                    if (split <= 0)
                        split = width;
                    yield return remaining[..split];
                    remaining = remaining[split..].TrimStart();
                }
                yield return remaining;
            }
        }

        private static string GetAnswerCell(string answer, int part, AdventOfCodeAttribute puzzle, List<string> details, bool expected = false)
        {
            if (answer == null)
                return expected ? "(unknown)" : "(unreported)";
            if (answer.Length == 0)
                return "(empty)";
            if (answer.Length <= 32 && answer.IndexOfAny(new[] { '\r', '\n', '\t', '|' }) < 0)
                return answer;

            string reference = $"[{puzzle.Year}-{puzzle.Day:00} {(expected ? "expected" : "part")} {part}]";
            details.Add($"{reference}\n{answer}");
            return reference;
        }

        private static void LoadSolutionCache()
        {
            if (SolutionCache == null) SolutionCache = new();

            SolutionCache.Clear();
            Type target = typeof(IAdventOfCode);
            Assembly a = Assembly.GetExecutingAssembly();
            IEnumerable<Type> solutions = a.GetTypes().Where(t => target.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            solutions.ToList().ForEach(t =>
            {
                if (Attribute.GetCustomAttribute(t, typeof(AdventOfCodeAttribute)) is AdventOfCodeAttribute ca)
                {
                    IAdventOfCode c = Activator.CreateInstance(t) as IAdventOfCode;
                    if (!SolutionCache.ContainsKey(ca.Index))
                    {
                        SolutionCache.Add(ca.Index, new KeyValuePair<AdventOfCodeAttribute, IAdventOfCode>(ca, c));
                    }
                }
            });
        }
    }
}
