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
    [AdventOfCode(Year = 2016, Day = 13)]
    public class Year2016Day13 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private record State(int X, int Y, int Steps)
        {
            public override string ToString() => $"({X},{Y})";
        }

        public Year2016Day13()
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

           
            int favoriteNumber = 1362; // Default if parsing fails

            _SW.Start();

            int part1 = FindShortestPath(favoriteNumber, (1, 1), (31, 39));

            _SW.Stop();

            Console.WriteLine("  Part 1: {0}, Execution Time: {1}", part1, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            int part2 = CountLocationsWithinSteps(favoriteNumber, (1, 1), 50);

            _SW.Stop();

            Console.WriteLine("  Part 2: {0}, Execution Time: {1}", part2, StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        /// <summary>
        /// Uses A* algorithm to find the shortest path from start to target
        /// </summary>
        private int FindShortestPath(int favoriteNumber, (int x, int y) start, (int x, int y) target)
        {
            // Priority queue: sorted by f(n) = g(n) + h(n)
            // g(n) = cost from start to current node
            // h(n) = heuristic cost (Manhattan distance) from current to target
            var openSet = new PriorityQueue<State, int>();
            var closedSet = new HashSet<(int, int)>();
            var gScore = new Dictionary<(int, int), int>();

            var startState = new State(start.x, start.y, 0);
            openSet.Enqueue(startState, Heuristic(start, target));
            gScore[(start.x, start.y)] = 0;

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current.X == target.x && current.Y == target.y)
                    return current.Steps;

                if (closedSet.Contains((current.X, current.Y)))
                    continue;

                closedSet.Add((current.X, current.Y));

                // Explore neighbors (up, down, left, right)
                var neighbors = new[]
                {
                    (current.X + 1, current.Y),
                    (current.X - 1, current.Y),
                    (current.X, current.Y + 1),
                    (current.X, current.Y - 1)
                };

                foreach (var (nx, ny) in neighbors)
                {
                    if (IsWall(nx, ny, favoriteNumber) || closedSet.Contains((nx, ny)))
                        continue;

                    int tentativeG = current.Steps + 1;
                    var neighborKey = (nx, ny);

                    if (!gScore.ContainsKey(neighborKey) || tentativeG < gScore[neighborKey])
                    {
                        gScore[neighborKey] = tentativeG;
                        var fScore = tentativeG + Heuristic((nx, ny), target);
                        openSet.Enqueue(new State(nx, ny, tentativeG), fScore);
                    }
                }
            }

            return -1; // No path found
        }

        /// <summary>
        /// For Part 2: Count how many locations are reachable within a certain number of steps
        /// </summary>
        private int CountLocationsWithinSteps(int favoriteNumber, (int x, int y) start, int maxSteps)
        {
            var visited = new HashSet<(int, int)>();
            var queue = new Queue<State>();

            queue.Enqueue(new State(start.x, start.y, 0));
            visited.Add((start.x, start.y));

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Steps >= maxSteps)
                    continue;

                var neighbors = new[]
                {
                    (current.X + 1, current.Y),
                    (current.X - 1, current.Y),
                    (current.X, current.Y + 1),
                    (current.X, current.Y - 1)
                };

                foreach (var (nx, ny) in neighbors)
                {
                    if (!IsWall(nx, ny, favoriteNumber) && !visited.Contains((nx, ny)))
                    {
                        visited.Add((nx, ny));
                        queue.Enqueue(new State(nx, ny, current.Steps + 1));
                    }
                }
            }

            return visited.Count;
        }

        /// <summary>
        /// Determines if a location is a wall based on the favorite number
        /// </summary>
        private bool IsWall(int x, int y, int favoriteNumber)
        {
            if (x < 0 || y < 0)
                return true;

            long value = (long)x * x + 3 * x + 2 * x * y + y + (long)y * y + favoriteNumber;
            int bitCount = CountSetBits(value);

            return bitCount % 2 == 1; // Odd = wall, Even = open space
        }

        /// <summary>
        /// Counts the number of set bits (1s) in a number
        /// </summary>
        private int CountSetBits(long n)
        {
            int count = 0;
            while (n > 0)
            {
                count += (int)(n & 1);
                n >>= 1;
            }
            return count;
        }

        /// <summary>
        /// Heuristic function for A* (Manhattan distance)
        /// </summary>
        private int Heuristic((int x, int y) current, (int x, int y) target)
        {
            return Math.Abs(current.x - target.x) + Math.Abs(current.y - target.y);
        }
    }
}
