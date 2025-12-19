using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 3)]
    public class Year2017Day03 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day03()
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

            int input = 277678;

            _SW.Start();
            long part1 = SolvePart1(input);
            _SW.Stop();
            Console.WriteLine("Part 1 (Manhattan distance from 1 to N in spiral): {0}, Execution Time: {1}", part1, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();
            long part2 = SolvePart2(input);
            _SW.Stop();
            Console.WriteLine("Part 2 (first spiral-sum value larger than N): {0}, Execution Time: {1}", part2, StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private long SolvePart1(int n)
        {
            if (n <= 1)
                return 0;

            int k = (int)Math.Ceiling((Math.Sqrt(n) - 1) / 2.0);
            int sideLen = 2 * k;
            long maxVal = (long)(2 * k + 1) * (2 * k + 1);

            long bestToMid = long.MaxValue;
            for (int t = 0; t < 4; t++)
            {
                long mid = maxVal - k - (long)t * sideLen;
                bestToMid = Math.Min(bestToMid, Math.Abs(n - mid));
            }

            return k + bestToMid;
        }

        private long SolvePart2(int target)
        {
            var grid = new Dictionary<(int x, int y), int>();
            grid[(0, 0)] = 1;
            if (1 > target)
                return 1;

            int x = 0;
            int y = 0;
            int stepLen = 1;

            while (true)
            {
                if (WalkAndFill(1, 0, stepLen, target, ref x, ref y, grid, out int value))
                    return value;
                if (WalkAndFill(0, 1, stepLen, target, ref x, ref y, grid, out value))
                    return value;

                stepLen++;

                if (WalkAndFill(-1, 0, stepLen, target, ref x, ref y, grid, out value))
                    return value;
                if (WalkAndFill(0, -1, stepLen, target, ref x, ref y, grid, out value))
                    return value;

                stepLen++;
            }
        }

        private bool WalkAndFill(int dx, int dy, int steps, int target, ref int x, ref int y,
            Dictionary<(int x, int y), int> grid, out int produced)
        {
            for (int i = 0; i < steps; i++)
            {
                x += dx;
                y += dy;

                int sum = 0;
                for (int ox = -1; ox <= 1; ox++)
                {
                    for (int oy = -1; oy <= 1; oy++)
                    {
                        if (ox == 0 && oy == 0)
                            continue;
                        if (grid.TryGetValue((x + ox, y + oy), out int v))
                            sum += v;
                    }
                }

                grid[(x, y)] = sum;
                if (sum > target)
                {
                    produced = sum;
                    return true;
                }
            }

            produced = 0;
            return false;
        }
    }
}
