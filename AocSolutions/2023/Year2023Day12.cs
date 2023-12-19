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

    [AdventOfCode(Year = 2023, Day = 12)]
    public class Year2023Day12 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day12()
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
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            long sum = Solve(input, 1);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Total number of arrangements: {sum}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Restart();
            
            _SW.Start();
            sum = Solve(input, 5);
            _SW.Stop();

            Console.WriteLine($"  Part 2: Total number of arrangements: {sum}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        long Solve(string[] input, int repeat)
        {
            long sum = 0;
            foreach (string line in input)
            {
                var parts = line.Split(" ");

                var pattern = Unfold(parts[0], '?', repeat);
                var numString = Unfold(parts[1], ',', repeat);
                var nums = numString.Split(',').Select(int.Parse);
                
                sum += Compute(pattern, ImmutableStack.CreateRange(nums.Reverse()), new Cache());
            }

            return sum;
        }

        string Unfold(string st, char join, int unfold) => string.Join(join, Enumerable.Repeat(st, unfold));

        long Compute(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            if (!cache.ContainsKey((pattern, nums)))
            {
                cache[(pattern, nums)] = Dispatch(pattern, nums, cache);
            }
            return cache[(pattern, nums)];
        }

        long Dispatch(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            return pattern.FirstOrDefault() switch
            {
                '.' => ProcessDot(pattern, nums, cache),
                '?' => ProcessQuestion(pattern, nums, cache),
                '#' => ProcessHash(pattern, nums, cache),
                _ => ProcessEnd(pattern, nums, cache),
            };
        }

        long ProcessEnd(string _, ImmutableStack<int> nums, Cache __)
        {
            // the good case is when there are no numbers left at the end of the pattern
            return nums.Any() ? 0 : 1;
        }

        long ProcessDot(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            // consume one spring and recurse
            return Compute(pattern[1..], nums, cache);
        }

        long ProcessQuestion(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            // recurse both ways
            return Compute("." + pattern[1..], nums, cache) + Compute("#" + pattern[1..], nums, cache);
        }

        long ProcessHash(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            // take the first number and consume that many dead springs, recurse

            if (!nums.Any())
            {
                return 0; // no more numbers left, this is no good
            }

            var n = nums.Peek();
            nums = nums.Pop();

            var potentiallyDead = pattern.TakeWhile(s => s == '#' || s == '?').Count();

            if (potentiallyDead < n)
            {
                return 0; // not enough dead springs 
            }
            else if (pattern.Length == n)
            {
                return Compute("", nums, cache);
            }
            else if (pattern[n] == '#')
            {
                return 0; // dead spring follows the range -> not good
            }
            else
            {
                return Compute(pattern[(n + 1)..], nums, cache);
            }
        }
    }
}