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
using MoreLinq.Extensions;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2025, Day = 5)]
    public class Year2025Day05: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day05()
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
            string input = FileIOHelper.getInstance().ReadDataAsString(file);

            var (ranges, availableIds) = ParseInput(input);

            // Pre-merge ranges for both parts
            var mergedRanges = MergeRanges(ranges);

            _SW.Start();

            long freshCount = CountFreshIngredients(mergedRanges, availableIds);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Fresh Ingredient IDs: {freshCount}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            // Part 2: Count unique IDs that fall within any range (handling overlaps)
            long uniqueFreshIngredients = CalculateTotalFreshIngredients(mergedRanges);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Total Unique Fresh Ingredients: {uniqueFreshIngredients}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private (List<(long, long)>, List<long>) ParseInput(string input)
        {
            var parts = input.Split(new string[] { "\n\n" }, StringSplitOptions.None);
            var rangeLines = parts[0].Split('\n');
            var idLines = parts[1].Split('\n');

            var ranges = new List<(long, long)>();
            foreach (var line in rangeLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var nums = line.Split('-');
                long start = long.Parse(nums[0]);
                long end = long.Parse(nums[1]);
                ranges.Add((start, end));
            }

            var availableIds = new List<long>();
            foreach (var line in idLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                availableIds.Add(long.Parse(line));
            }

            return (ranges, availableIds);
        }

        private List<(long, long)> MergeRanges(List<(long, long)> ranges)
        {
            if (ranges.Count == 0) return ranges;

            var sortedRanges = ranges.OrderBy(r => r.Item1).ToList();
            var merged = new List<(long, long)>();

            long currentStart = sortedRanges[0].Item1;
            long currentEnd = sortedRanges[0].Item2;

            for (int i = 1; i < sortedRanges.Count; i++)
            {
                var (nextStart, nextEnd) = sortedRanges[i];

                // If ranges overlap or are adjacent, merge them
                if (nextStart <= currentEnd + 1)
                {
                    currentEnd = Math.Max(currentEnd, nextEnd);
                }
                else
                {
                    // Non-overlapping range, add current to merged list
                    merged.Add((currentStart, currentEnd));
                    currentStart = nextStart;
                    currentEnd = nextEnd;
                }
            }

            // Add the last range
            merged.Add((currentStart, currentEnd));

            return merged;
        }

        private long CountFreshIngredients(List<(long, long)> mergedRanges, List<long> availableIds)
        {
            long count = 0;
            foreach (var id in availableIds)
            {
                // Binary search-like check: since ranges are merged and sorted, check each
                foreach (var (start, end) in mergedRanges)
                {
                    if (id >= start && id <= end)
                    {
                        count++;
                        break; // Found in a range, no need to check others
                    }
                    else if (id < start)
                    {
                        break; // No point checking further, ranges are sorted
                    }
                }
            }
            return count;
        }

        private long CalculateTotalFreshIngredients(List<(long, long)> mergedRanges)
        {
            long totalUnique = 0;
            foreach (var (start, end) in mergedRanges)
            {
                totalUnique += (end - start + 1);
            }
            return totalUnique;
        }
    }
}
