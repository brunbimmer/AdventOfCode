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
    [AdventOfCode(Year = 2022, Day = 18)]
    public class Year2022Day18 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day18()
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
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();
            
            var cubes = ParseCubes(lines);
            int part1 = CalculateSurfaceArea(cubes);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Surface Area: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            int part2 = CalculateExteriorSurfaceArea(cubes);
            
            _SW.Stop();

            Console.WriteLine($"  Part 2: Exterior Surface Area: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        HashSet<(int, int, int)> ParseCubes(string[] lines)
        {
            var cubes = new HashSet<(int, int, int)>();
            foreach (string line in lines)
            {
                var parts = line.Split(',');
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                int z = int.Parse(parts[2]);
                cubes.Add((x, y, z));
            }
            return cubes;
        }

        int CalculateSurfaceArea(HashSet<(int, int, int)> cubes)
        {
            int surfaceArea = 0;
            var directions = new[] { (1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1) };

            foreach (var cube in cubes)
            {
                foreach (var (dx, dy, dz) in directions)
                {
                    var neighbor = (cube.Item1 + dx, cube.Item2 + dy, cube.Item3 + dz);
                    if (!cubes.Contains(neighbor))
                    {
                        surfaceArea++;
                    }
                }
            }

            return surfaceArea;
        }

        int CalculateExteriorSurfaceArea(HashSet<(int, int, int)> cubes)
        {
            // Find the bounds of the lava droplet
            int minX = cubes.Min(c => c.Item1) - 1;
            int maxX = cubes.Max(c => c.Item1) + 1;
            int minY = cubes.Min(c => c.Item2) - 1;
            int maxY = cubes.Max(c => c.Item2) + 1;
            int minZ = cubes.Min(c => c.Item3) - 1;
            int maxZ = cubes.Max(c => c.Item3) + 1;

            // Flood fill from outside to find all exterior air
            var exterior = new HashSet<(int, int, int)>();
            var queue = new Queue<(int, int, int)>();
            queue.Enqueue((minX, minY, minZ));
            exterior.Add((minX, minY, minZ));

            var directions = new[] { (1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1) };

            while (queue.Count > 0)
            {
                var (x, y, z) = queue.Dequeue();

                foreach (var (dx, dy, dz) in directions)
                {
                    var neighbor = (x + dx, y + dy, z + dz);

                    // Check if neighbor is within bounds and not yet visited
                    if (neighbor.Item1 >= minX && neighbor.Item1 <= maxX &&
                        neighbor.Item2 >= minY && neighbor.Item2 <= maxY &&
                        neighbor.Item3 >= minZ && neighbor.Item3 <= maxZ &&
                        !exterior.Contains(neighbor) &&
                        !cubes.Contains(neighbor))
                    {
                        exterior.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // Count surface area where lava touches exterior air
            int exteriorSurfaceArea = 0;
            foreach (var cube in cubes)
            {
                foreach (var (dx, dy, dz) in directions)
                {
                    var neighbor = (cube.Item1 + dx, cube.Item2 + dy, cube.Item3 + dz);
                    if (exterior.Contains(neighbor))
                    {
                        exteriorSurfaceArea++;
                    }
                }
            }

            return exteriorSurfaceArea;
        }
    }
}
