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
    [AdventOfCode(Year = 2025, Day = 7)]
    public class Year2025Day07 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day07()
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
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            int splitCount = CountSplits(input);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Total Splits: {splitCount}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long timelineCount = CountTimelines(input);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Total Timelines: {timelineCount}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private int CountSplits(string[] grid)
        {
            // Find starting position (S)
            int startCol = -1;
            for (int i = 0; i < grid[0].Length; i++)
            {
                if (grid[0][i] == 'S')
                {
                    startCol = i;
                    break;
                }
            }

            // Track values at each column as they fall
            var current = new Dictionary<int, long> { { startCol, 1 } };
            int splitCount = 0;

            // Process each row
            // Note: splitCount counts the number of BEAM SPLITS, not the number of '^' characters.
            // Multiple beams can hit the same splitter, and each beam that hits creates a split.
            // So if 2 beams hit the same '^', that's 2 splits (counted separately).
            for (int row = 1; row < grid.Length; row++)
            {
                var next = new Dictionary<int, long>();

                foreach (var (col, value) in current)
                {
                    if (grid[row][col] == '^')
                    {
                        // Splitter: split into left and right
                        // Increment splitCount for each beam hitting this splitter
                        splitCount++;
                        
                        if (col > 0)
                            next[col - 1] = next.GetValueOrDefault(col - 1) + value;
                        if (col < grid[row].Length - 1)
                            next[col + 1] = next.GetValueOrDefault(col + 1) + value;
                    }
                    else
                    {
                        // Pass through
                        next[col] = next.GetValueOrDefault(col) + value;
                    }
                }

                current = next;
            }

            return splitCount;
        }

        private long CountTimelines(string[] grid)
        {
            // Find starting position (S)
            int startCol = -1;
            for (int i = 0; i < grid[0].Length; i++)
            {
                if (grid[0][i] == 'S')
                {
                    startCol = i;
                    break;
                }
            }

            var memo = new Dictionary<(int row, int col), long>();
            
            // DFS with memoization to count distinct timelines (paths)
            // Direction: 0 = down, 1 = left, 2 = right
            // Start moving down from the row after S
            long timelineCount = DFSMemo(grid, 1, startCol, memo);

            return timelineCount;
        }

        private long DFSMemo(string[] grid, int row, int col, Dictionary<(int, int), long> memo)
        {
            // Out of bounds
            if (row < 0 || row >= grid.Length || col < 0 || col >= grid[row].Length)
                return 0;

            // If we've reached the bottom row (last row in grid), this is one complete path
            if (row == grid.Length - 1)
                return 1;

            // Check memo
            var state = (row, col);
            if (memo.ContainsKey(state))
                return memo[state];

            long timelines = 0;
            char current = grid[row][col];

            if (current == '^')
            {
                // Splitter creates two paths: left and right
                timelines += DFSMemo(grid, row + 1, col - 1, memo); // Go left
                timelines += DFSMemo(grid, row + 1, col + 1, memo); // Go right
            }
            else
            {
                // Continue down
                timelines += DFSMemo(grid, row + 1, col, memo);
            }

            return memo[state] = timelines;
        }
    }
}
