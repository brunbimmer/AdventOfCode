using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 7)]
    public class Year2024Day07: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2024Day07()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            var equations = ParseEquations(lines);
            _SW.Start();

            long part1 = Solve(equations, includeConcat: false);

            _SW.Stop();

            Console.WriteLine($"  Part 1 (sum of test values solvable with +,*): {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            long part2 = Solve(equations, includeConcat: true);

            _SW.Stop();

            Console.WriteLine($"  Part 2 (sum solvable with +,*,concat): {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        private sealed record Equation(long Target, long[] Numbers);

        private static List<Equation> ParseEquations(string[] lines)
        {
            var result = new List<Equation>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(':', 2);
                long target = long.Parse(parts[0].Trim());
                long[] numbers = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse)
                    .ToArray();

                result.Add(new Equation(target, numbers));
            }
            return result;
        }

        private static long Solve(List<Equation> equations, bool includeConcat)
        {
            long sum = 0;
            foreach (var eq in equations)
            {
                if (IsSolvable(eq.Target, eq.Numbers, includeConcat))
                    sum += eq.Target;
            }
            return sum;
        }

        private static bool IsSolvable(long target, long[] numbers, bool includeConcat)
        {
            // Keep the set of all possible running totals after processing i numbers.
            // Operators evaluate strictly left-to-right.
            var current = new HashSet<long> { numbers[0] };

            for (int i = 1; i < numbers.Length; i++)
            {
                long n = numbers[i];
                var next = new HashSet<long>();

                foreach (var v in current)
                {
                    long a = v + n;
                    if (a <= target) next.Add(a);

                    long m = v * n;
                    if (m <= target) next.Add(m);

                    if (includeConcat)
                    {
                        long c = Concat(v, n);
                        if (c <= target) next.Add(c);
                    }
                }

                if (next.Count == 0)
                    return false;

                current = next;
            }

            return current.Contains(target);
        }

        private static long Concat(long left, long right)
        {
            // Decimal concatenation.
            long pow = 10;
            while (pow <= right)
                pow *= 10;
            return left * pow + right;
        }
    }
}
