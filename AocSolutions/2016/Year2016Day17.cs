using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 17)]
    public class Year2016Day17 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day17()
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


            string puzzleInput = "edjrjqaa"; // Hardcoded input for Day 17

            _SW.Start();                       


            string shortestPath = CalculatePath(puzzleInput, true);
            
            _SW.Stop();

            Console.WriteLine("Part 1 - Shortest Path: {0}, Execution Time: {1}", shortestPath, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            string longestPath = CalculatePath(puzzleInput, false);
            
            _SW.Stop();

            Console.WriteLine("Part 2 - Longest Path Length {0}, Execution Time: {1}", longestPath.Length, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        private string CalculatePath(string input, bool findShortest)
        {
            var directions = new (int dx, int dy, char dir)[]
            {
                (0, -1, 'U'),
                (0, 1, 'D'),
                (-1, 0, 'L'),
                (1, 0, 'R')
            };

            var queue = new Queue<(int x, int y, string path)>();
            queue.Enqueue((0, 0, ""));

            string resultPath = findShortest ? null : "";

            while (queue.Count > 0)
            {
                var (x, y, path) = queue.Dequeue();

                if (x == 3 && y == 3)
                {
                    if (findShortest)
                    {
                        return path;
                    }
                    else
                    {
                        if (path.Length > resultPath.Length)
                        {
                            resultPath = path;
                        }
                    }
                    continue;
                }

                string hash = Utilities.ComputeMD5Hash(input + path).Substring(0, 4);

                for (int i = 0; i < directions.Length; i++)
                {
                    var (dx, dy, dir) = directions[i];
                    if ("bcdef".Contains(hash[i]))
                    {
                        int newX = x + dx;
                        int newY = y + dy;

                        if (newX >= 0 && newX <= 3 && newY >= 0 && newY <= 3)
                        {
                            queue.Enqueue((newX, newY, path + dir));
                        }
                    }
                }
            }

            return resultPath;
        }
    }
}
