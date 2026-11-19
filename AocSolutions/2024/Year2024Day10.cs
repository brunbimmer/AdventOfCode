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
    [AdventOfCode(Year = 2024, Day = 10)]
    public class Year2024Day10: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2024Day10()
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

            int height = lines.Length;
            int width = lines[0].Length;
            int[,] grid = new int[height, width];

            // Parse the grid
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    grid[row, col] = int.Parse(lines[row][col].ToString());
                }
            }

            _SW.Start();

            long part1 = SolvePart1(grid, height, width);

            _SW.Stop();

            output.WritePart(1, part1, trackTime ? _SW.Elapsed : null, "Sum of trailhead scores (unique 9s reachable)");


            _SW.Restart();

            long part2 = SolvePart2(grid, height, width);

            output.WritePart(2, part2, trackTime ? _SW.Elapsed : null, "Sum of all distinct paths from trailheads to 9s");


            _SW.Stop();

        }

        private long SolvePart1(int[,] grid, int height, int width)
        {
            
            long part1 = 0;
            // Find all trailheads (cells with height 0)
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (grid[row, col] == 0)
                    {
                        // Count unique 9s reachable from this trailhead
                        var reachable9s = new HashSet<(int, int)>();
                        CountPaths(grid, row, col, 0, reachable9s, null, height, width);
                        part1 += reachable9s.Count;
                    }
                }
            }

            return part1;
        }

        private long SolvePart2(int[,] grid, int height, int width)
        {
            long part2 = 0;
            // Find all trailheads (cells with height 0) and count all paths
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (grid[row, col] == 0)
                    {
                        // Count all distinct paths from this trailhead to any 9
                        part2 += CountPaths(grid, row, col, 0, null, new HashSet<(int, int)>(), height, width);
                    }
                }
            }

            return part2;
        }

        private long CountPaths(int[,] grid, int row, int col, int expectedHeight, HashSet<(int, int)> reachable9s, HashSet<(int, int)> visited, int height, int width)
        {
            // Check bounds
            if (row < 0 || row >= height || col < 0 || col >= width)
                return 0;

            // Check if current cell has expected height
            if (grid[row, col] != expectedHeight)
                return 0;

            // If we reached a 9
            if (expectedHeight == 9)
            {
                if (reachable9s != null)
                {
                    reachable9s.Add((row, col));
                    return 0; // For part 1, we just mark it as reachable
                }
                else
                {
                    return 1; // For part 2, count this path
                }
            }

            long pathCount = 0;
            int nextHeight = expectedHeight + 1;

            // Explore all 4 directions
            int[][] directions = new int[][] { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 } };

            foreach (var dir in directions)
            {
                int newRow = row + dir[0];
                int newCol = col + dir[1];
                pathCount += CountPaths(grid, newRow, newCol, nextHeight, reachable9s, visited, height, width);
            }

            return pathCount;
        }
    }
}
