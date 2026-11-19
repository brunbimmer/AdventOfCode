using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 11)]
    public class Year2024Day11: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2024Day11()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            //Build BasePath and retrieve input. 
 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            long[] stones = lines[0].Split(' ').Select(long.Parse).ToArray();

            SolvePart1(stones, output, trackTime);
            SolvePart2(stones, output, trackTime);

        }

        private void SolvePart1(long[] stones, IPuzzleOutput output, bool trackTime)
        {
            _SW.Start();

            long part1 = CountStonesAfterBlinks(stones, 25);

            _SW.Stop();

            output.WritePart(1, part1, trackTime ? _SW.Elapsed : null, "Stone count after 25 blinks");

        }

        private void SolvePart2(long[] stones, IPuzzleOutput output, bool trackTime)
        {
            _SW.Restart();

            long part2 = CountStonesAfterBlinks(stones, 75);

            _SW.Stop();

            output.WritePart(2, part2, trackTime ? _SW.Elapsed : null, "Stone count after 75 blinks");


        }

        private long CountStonesAfterBlinks(long[] stones, int blinks)
        {
            // Use memoization to track how many stones each value produces after N blinks
            Dictionary<(long, int), long> memo = new Dictionary<(long, int), long>();
            
            long totalCount = 0;
            foreach (long stone in stones)
            {
                totalCount += CountStones(stone, blinks, memo);
            }

            return totalCount;
        }

        private long CountStones(long value, int blinksRemaining, Dictionary<(long, int), long> memo)
        {
            // Base case
            if (blinksRemaining == 0)
                return 1;

            // Check memo
            var key = (value, blinksRemaining);
            if (memo.ContainsKey(key))
                return memo[key];

            long result = 0;

            // Apply transformation rule
            if (value == 0)
            {
                result = CountStones(1, blinksRemaining - 1, memo);
            }
            else
            {
                string digits = value.ToString();
                if (digits.Length % 2 == 0)
                {
                    // Split in half
                    string leftStr = digits.Substring(0, digits.Length / 2);
                    string rightStr = digits.Substring(digits.Length / 2);
                    long left = long.Parse(leftStr);
                    long right = long.Parse(rightStr);
                    result = CountStones(left, blinksRemaining - 1, memo) + CountStones(right, blinksRemaining - 1, memo);
                }
                else
                {
                    // Multiply by 2024
                    result = CountStones(value * 2024, blinksRemaining - 1, memo);
                }
            }

            memo[key] = result;
            return result;


        }
    }
}
