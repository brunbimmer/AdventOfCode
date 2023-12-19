using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 13)]
    public class Year2023Day13 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day13()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

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
            string input = FileIOHelper.getInstance().ReadDataAsString(file);

            _SW.Start();

            int sum = SolvePart1(input);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Number after summarizing all the notes: {sum}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Restart();
            
            _SW.Start();
            sum = SolvePart2(input);
            _SW.Stop();

            Console.WriteLine($"  Part 2: New line of reflection summation: {sum}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        private List<Field> ParseInput(string input)
        {
            return input.Replace(".", "0").Replace("#", "1").Split("\n").SplitWhen(line => line.Length == 0)
                .Select(ParseFieldLines).ToList();
        }

        int SolvePart1(string input)
        {
            return ParseInput(input).Sum(field => field.Score);
            
        }

        int SolvePart2(string input)
        {
            return ParseInput(input).Sum(field => field.FindAlternativeReflections().
                First().
                GetAlternativeScore(field));
        }

        public Field ParseFieldLines(IEnumerable<string> lines) =>
            new(lines.Select(line => line.ToCharArray()).
                ToArray());
    }

    public record Field(char[][] FieldData)
    {
        public int Width => FieldData[0].Length;
        public int Height => FieldData.Length;

        public List<ulong> Horizontal =>
            Enumerable.Range(0, Width).
                       Select(x => Convert.ToUInt64(new string(Enumerable.Range(0, Height).
                                                                          Select(y => FieldData[y][x]).
                                                                          ToArray()), 2)).
                       ToList();

        public List<ulong> Vertical =>
            Enumerable.Range(0, Height).
                       Select(y => Convert.ToUInt64(new string(Enumerable.Range(0, Width).
                                                                          Select(x => FieldData[y][x]).
                                                                          Reverse().
                                                                          ToArray()), 2)).
                       ToList();

        public List<int> HorizontalPalindromes => FindPalindrome(Horizontal);
        public List<int> VerticalPalindromes => FindPalindrome(Vertical);

        public int FirstHorizontalPalindromeIndex => HorizontalPalindromes.Count == 0 ? -1 : HorizontalPalindromes[0];
        public int FirstVerticalPalindromeIndex => VerticalPalindromes.Count == 0 ? -1 : VerticalPalindromes[0];

        public int Score => (FirstVerticalPalindromeIndex + 1) * 100 + FirstHorizontalPalindromeIndex + 1;

        public Field GetAlternative(int x, int y)
        {
            var newFieldData = FieldData.Select(row => row.ToArray()).
                                         ToArray();
            newFieldData[y][x] = newFieldData[y][x] == '0' ? '1' : '0';
            return new Field(newFieldData);
        }

        public bool IsPalindrome(List<ulong> list, int index)
        {
            int minLength = Math.Min(index + 1, list.Count - index - 1);
            return list.Skip(index - minLength + 1).
                        Take(minLength).
                        SequenceEqual(list.Skip(index + 1).
                                           Take(minLength).
                                           Reverse());
        }

        public List<int> FindPalindrome(List<ulong> list) =>
            Enumerable.Range(0, list.Count - 1).
                       Where(i => list[i] == list[i + 1] && IsPalindrome(list, i)).
                       ToList();

        public IEnumerable<Field> FindAlternativeReflections() =>
            Enumerable.Range(0, Width).
                       SelectMany(x => Enumerable.Range(0, Height), (x, y) => GetAlternative(x, y)).
                       Where(newField => HasDifferentPalindromes(newField));

        private bool HasDifferentPalindromes(Field newField)
        {
            return (newField.HorizontalPalindromes.Any() && !newField.HorizontalPalindromes.SequenceEqual(HorizontalPalindromes)) ||
                   (newField.VerticalPalindromes.Any() && !newField.VerticalPalindromes.SequenceEqual(VerticalPalindromes));
        }

        public int GetAlternativeScore(Field original)
        {
            var newHorizontalPalindromes = HorizontalPalindromes.Where(o => !original.HorizontalPalindromes.Contains(o)).
                                                                 ToList();
            var newVerticalPalindromes = VerticalPalindromes.Where(o => !original.VerticalPalindromes.Contains(o)).
                                                             ToList();

            var numCols = newHorizontalPalindromes.Count == 0 ? 0 : newHorizontalPalindromes[0] + 1;
            var numRows = newVerticalPalindromes.Count == 0 ? 0 : newVerticalPalindromes[0] + 1;

            return numRows * 100 + numCols;
        }
    }
}