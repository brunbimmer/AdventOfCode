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
    [AdventOfCode(Year = 2016, Day = 24)]
    public class Year2016Day24 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day24()
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

            string[] map = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            int shortestPath = FindShortestPath(map);

            _SW.Stop();

            Console.WriteLine("Part 1 - Shortest Path: {0}, Execution Time: {1}", shortestPath, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            int shortestPathReturn = FindShortestPathWithReturn(map);

            _SW.Stop();

            Console.WriteLine("Part 2 - Shortest Path (Return to 0): {0}, Execution Time: {1}", shortestPathReturn, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        int FindShortestPath(string[] map)
        {
            return FindShortestPathInternal(map, returnToStart: false);
        }

        int FindShortestPathWithReturn(string[] map)
        {
            return FindShortestPathInternal(map, returnToStart: true);
        }

        int FindShortestPathInternal(string[] map, bool returnToStart)
        {
            // Parse map to find all numbered locations
            Dictionary<int, (int x, int y)> locations = ParseLocations(map);
            int numLocations = locations.Count;

            // Precompute distances between all pairs of locations using BFS
            int[,] distances = new int[numLocations, numLocations];
            for (int i = 0; i < numLocations; i++)
            {
                distances[i, i] = 0;
                for (int j = i + 1; j < numLocations; j++)
                {
                    int dist = BFS(map, locations[i], locations[j]);
                    distances[i, j] = dist;
                    distances[j, i] = dist;
                }
            }

            // Use permutations to find shortest path visiting all locations starting from 0
            List<int> indices = Enumerable.Range(1, numLocations - 1).ToList();
            int minSteps = int.MaxValue;

            foreach (var perm in Permutations(indices))
            {
                int steps = 0;
                int current = 0;
                foreach (int next in perm)
                {
                    steps += distances[current, next];
                    current = next;
                }
                // Return to 0 if required (Part 2)
                if (returnToStart)
                {
                    steps += distances[current, 0];
                }
                minSteps = Math.Min(minSteps, steps);
            }

            return minSteps;
        }

        Dictionary<int, (int, int)> ParseLocations(string[] map)
        {
            Dictionary<int, (int, int)> locations = new Dictionary<int, (int, int)>();

            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (char.IsDigit(map[y][x]))
                    {
                        int num = int.Parse(map[y][x].ToString());
                        locations[num] = (x, y);
                    }
                }
            }

            return locations;
        }

        int BFS(string[] map, (int x, int y) start, (int x, int y) end)
        {
            var queue = new Queue<((int x, int y), int)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue((start, 0));
            visited.Add(start);

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            while (queue.Count > 0)
            {
                var ((x, y), dist) = queue.Dequeue();

                if (x == end.x && y == end.y)
                    return dist;

                for (int i = 0; i < 4; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    if (nx >= 0 && nx < map[0].Length && ny >= 0 && ny < map.Length &&
                        map[ny][nx] != '#' && !visited.Contains((nx, ny)))
                    {
                        visited.Add((nx, ny));
                        queue.Enqueue(((nx, ny), dist + 1));
                    }
                }
            }

            return int.MaxValue;
        }

        IEnumerable<List<T>> Permutations<T>(List<T> items)
        {
            if (items.Count == 0)
            {
                yield return new List<T>();
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var remaining = items.Where((x, idx) => idx != i).ToList();
                    foreach (var perm in Permutations(remaining))
                    {
                        yield return new List<T> { items[i] }.Concat(perm).ToList();
                    }
                }
            }
        }       
    }
}
