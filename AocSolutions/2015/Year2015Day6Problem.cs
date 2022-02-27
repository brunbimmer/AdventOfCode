using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 6)]
    public class Year2015Day6Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day6Problem()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //no need to retrieve input from AOC. The input is a simple string

            string filename = _OverrideFile ?? path;

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            Dictionary<Coordinate2D, int> grid = new Dictionary<Coordinate2D, int>();


            if (trackTime) SW.Start();

            int lightsOn = Part1(lines, grid);

            if (trackTime) SW.Stop();

            Console.WriteLine("  Part 1: The number of lights that are turned on after Santa's instruction: {0}", lightsOn);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            grid.Clear();

            long brightness = Part2(lines, grid);

            Console.WriteLine("  Part 2: Total brightness of lights as per Santa's instructions: {0}", brightness);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        public int Part1(string[] list, Dictionary<Coordinate2D, int> grid)
        {

            foreach (string line in list)
            {
                string[] instructions = line.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (line.StartsWith("turn on"))
                {
                    Coordinate2D start = new Coordinate2D(Convert.ToInt32(instructions[2]), Convert.ToInt32(instructions[3]));
                    Coordinate2D stop = new Coordinate2D(Convert.ToInt32(instructions[5]), Convert.ToInt32(instructions[6]));

                    GridOn(grid, start, stop);
                }
                else if (line.StartsWith("turn off"))
                {
                    Coordinate2D start = new Coordinate2D(Convert.ToInt32(instructions[2]), Convert.ToInt32(instructions[3]));
                    Coordinate2D stop = new Coordinate2D(Convert.ToInt32(instructions[5]), Convert.ToInt32(instructions[6]));

                    GridOff(grid, start, stop);
                }
                else if (line.StartsWith("toggle"))
                {
                    Coordinate2D start = new Coordinate2D(Convert.ToInt32(instructions[1]), Convert.ToInt32(instructions[2]));
                    Coordinate2D stop = new Coordinate2D(Convert.ToInt32(instructions[4]), Convert.ToInt32(instructions[5]));

                    GridToggle(grid, start, stop);
                }

            }

            return grid.Count(x => x.Value == 1);
            
        }

        private void GridOn(Dictionary<Coordinate2D, int> grid, Coordinate2D start, Coordinate2D stop)
        {
            for (int x = start.X; x <= stop.X; x++)
            {
                for (int y = start.Y; y <= stop.Y; y++)
                {
                    Coordinate2D coord = new Coordinate2D(x, y);
                    grid[coord] = 1;
                }
            }
        }

        private void GridOff(Dictionary<Coordinate2D, int> grid, Coordinate2D start, Coordinate2D stop)
        {
            for (int x = start.X; x <= stop.X; x++)
            {
                for (int y = start.Y; y <= stop.Y; y++)
                {
                    Coordinate2D coord = new Coordinate2D(x, y);
                    grid[coord] = 0;
                }
            }
        }

        private void GridToggle(Dictionary<Coordinate2D, int> grid, Coordinate2D start, Coordinate2D stop)
        {
            for (int x = start.X; x <= stop.X; x++)
            {
                for (int y = start.Y; y <= stop.Y; y++)
                {
                    Coordinate2D coord = new Coordinate2D(x, y);
                    
                    if (grid.ContainsKey(coord) && grid[coord] == 1)
                        grid[coord] = 0;
                    else
                        grid[coord] = 1;
                }
            }
        }

        public long Part2(string[] list, Dictionary<Coordinate2D, int> grid)
        {
            foreach (string line in list)
            {
                string[] instructions = line.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (line.StartsWith("turn on"))
                {
                    Coordinate2D start = new Coordinate2D(Convert.ToInt32(instructions[2]), Convert.ToInt32(instructions[3]));
                    Coordinate2D stop = new Coordinate2D(Convert.ToInt32(instructions[5]), Convert.ToInt32(instructions[6]));

                    GridIncreaseBrightness(grid, start, stop);
                }
                else if (line.StartsWith("turn off"))
                {
                    Coordinate2D start = new Coordinate2D(Convert.ToInt32(instructions[2]), Convert.ToInt32(instructions[3]));
                    Coordinate2D stop = new Coordinate2D(Convert.ToInt32(instructions[5]), Convert.ToInt32(instructions[6]));

                    GridReduceBrightness(grid, start, stop);
                }
                else if (line.StartsWith("toggle"))
                {
                    Coordinate2D start = new Coordinate2D(Convert.ToInt32(instructions[1]), Convert.ToInt32(instructions[2]));
                    Coordinate2D stop = new Coordinate2D(Convert.ToInt32(instructions[4]), Convert.ToInt32(instructions[5]));

                    GridDoubleBrightness(grid, start, stop);
                }

            }

            return grid.Sum(x => x.Value);
        }



        private void GridIncreaseBrightness(Dictionary<Coordinate2D, int> grid, Coordinate2D start, Coordinate2D stop)
        {
            for (int x = start.X; x <= stop.X; x++)
            {
                for (int y = start.Y; y <= stop.Y; y++)
                {
                    Coordinate2D coord = new Coordinate2D(x, y);

                    if (!grid.ContainsKey(coord))
                        grid.Add(coord, 1);
                    else
                        grid[coord] += 1;
                }
            }
        }

        private void GridReduceBrightness(Dictionary<Coordinate2D, int> grid, Coordinate2D start, Coordinate2D stop)
        {
            for (int x = start.X; x <= stop.X; x++)
            {
                for (int y = start.Y; y <= stop.Y; y++)
                {
                    Coordinate2D coord = new Coordinate2D(x, y);

                    if (!grid.ContainsKey(coord))
                        grid.Add(coord, 0);
                    else
                        grid[coord] -= 1;

                    if (grid[coord] < 0)
                        grid[coord] = 0;
                }
            }
        }

        private void GridDoubleBrightness(Dictionary<Coordinate2D, int> grid, Coordinate2D start, Coordinate2D stop)
        {
            for (int x = start.X; x <= stop.X; x++)
            {
                for (int y = start.Y; y <= stop.Y; y++)
                {
                    Coordinate2D coord = new Coordinate2D(x, y);

                    if (!grid.ContainsKey(coord))
                        grid.Add(coord, 2);
                    else
                        grid[coord] += 2;
                }
            }
        }
    }
}
