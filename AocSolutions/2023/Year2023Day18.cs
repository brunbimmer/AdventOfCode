using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;
using LINQPad.Controls;
using static Common.Utilities;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 18)]
    public class Year2023Day18 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day18()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new();
        }

        List<(CompassDirection direction, long distance)> instructionsPart1 = new();
        List<(CompassDirection direction, long distance)> instructionsPart2 = new();


        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            ParseInput(lines);

            _SW.Start();
            long result = GetDigArea(instructionsPart1);
            _SW.Stop();

            Console.WriteLine($"  Part 1: Volume of Lava: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Restart();
            
            _SW.Start();
            long result2 = GetDigArea(instructionsPart2);
            _SW.Stop();

            Console.WriteLine($"  Part 2: Volume of Lava: {result2}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        private void ParseInput(string[] input)
        {

            foreach (var l in input)
            {
                var parts = l.Split(' ');
                CompassDirection dir1 = GetDirection(parts[0].First()); 
                long dist1 = int.Parse(parts[1]);                             //Steps
                string color = parts[2].TrimStart('#', '(').TrimEnd(')');     //Color

                instructionsPart1.Add((dir1, dist1));

                //additional step for part 1
                //parse first 5 characters of hex number to get distance
                //parse last digit to get direction
                long dist2 = int.Parse(color[..5], System.Globalization.NumberStyles.HexNumber);
                CompassDirection dir2 = GetDirection(color.Last());

                instructionsPart2.Add((dir2, dist2));
            }

            CompassDirection GetDirection(char direction)
            {
                return (direction) switch
                {
                    'U' => N,
                    'D' => S,
                    'L' => W,
                    'R' => E,
                    '0' => E,
                    '1' => S,
                    '2' => W,
                    '3' => N,
                    _ => throw new ArgumentException()
                };
            }
        }

        protected long GetDigArea(List<(CompassDirection direction, long distance)> instructions)
        {
            List<Coordinate2DLong> points = new();
            Coordinate2DLong curLoc = (0, 0);

            long wallSize = 0;

            foreach (var s in instructions)
            {
                var (direction, distance) = s;
                wallSize += distance;

                curLoc = curLoc.MoveDirection(direction, distance: distance);
                points.Add(curLoc);
            }

            long res1 = 0;
            long res2 = 0;
            for (int i = 0; i < points.Count; i++)
            {
                res1 += (points[i].X * points[(i + 1) % points.Count].Y);
                res2 += (points[i].Y * points[(i + 1) % points.Count].X);
            }

            return (wallSize / 2) + Math.Abs((res1 - res2) / 2) + 1;
        }
    }
}