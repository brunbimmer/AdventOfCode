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

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 23)]
    public class Year2023Day23: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day23()
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

            long part1 = SolvePart1(lines);

            _SW.Stop();

            Console.WriteLine($"  Part 1: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            long part2 = SolvePart2(lines);

            _SW.Stop();

            Console.WriteLine($"  Part 2: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private long SolvePart1(string[] lines)
        {
            var grid = lines.Select(l => l.ToCharArray()).ToArray();
            int rows = grid.Length;
            int cols = grid[0].Length;
            
            var start = (0, 1);
            var end = (rows - 1, cols - 2);
            
            var visited = new HashSet<(int, int)>();
            return DFS(grid, start, end, visited, respectSlopes: true);
        }

        private long SolvePart2(string[] lines)
        {
            var grid = lines.Select(l => l.ToCharArray()).ToArray();
            int rows = grid.Length;
            int cols = grid[0].Length;
            
            var start = (0, 1);
            var end = (rows - 1, cols - 2);
            
            // Build junction graph
            var graph = BuildGraph(grid, start, end);
            
            var visited = new HashSet<(int, int)>();
            return DFSGraph(graph, start, end, visited);
        }

        private long DFS(char[][] grid, (int, int) current, (int, int) end, HashSet<(int, int)> visited, bool respectSlopes)
        {
            if (current == end)
                return 0;
            
            visited.Add(current);
            long maxSteps = -1;
            
            // 4 directions: up, down, left, right
            int[][] directions = new[] { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };
            
            for (int i = 0; i < directions.Length; i++)
            {
                int nr = current.Item1 + directions[i][0];
                int nc = current.Item2 + directions[i][1];
                
                if (nr < 0 || nr >= grid.Length || nc < 0 || nc >= grid[0].Length)
                    continue;
                
                if (grid[nr][nc] == '#')
                    continue;
                
                if (visited.Contains((nr, nc)))
                    continue;
                
                // Check slope restrictions for Part 1
                if (respectSlopes)
                {
                    char cell = grid[nr][nc];
                    if (cell == '^' && i != 0) continue;  // Up
                    if (cell == 'v' && i != 1) continue;  // Down
                    if (cell == '<' && i != 2) continue;  // Left
                    if (cell == '>' && i != 3) continue;  // Right
                }
                
                long steps = DFS(grid, (nr, nc), end, visited, respectSlopes);
                if (steps >= 0)
                    maxSteps = Math.Max(maxSteps, steps + 1);
            }
            
            visited.Remove(current);
            return maxSteps;
        }

        private Dictionary<(int, int), List<((int, int), int)>> BuildGraph(char[][] grid, (int, int) start, (int, int) end)
        {
            int rows = grid.Length;
            int cols = grid[0].Length;
            
            var graph = new Dictionary<(int, int), List<((int, int), int)>>();
            var junctions = new HashSet<(int, int)>();
            
            // Find all junctions (including start and end)
            junctions.Add(start);
            junctions.Add(end);
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r][c] != '#')
                    {
                        int neighbors = 0;
                        int[][] directions = new[] { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };
                        
                        foreach (var dir in directions)
                        {
                            int nr = r + dir[0];
                            int nc = c + dir[1];
                            if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && grid[nr][nc] != '#')
                                neighbors++;
                        }
                        
                        if (neighbors >= 3)
                            junctions.Add((r, c));
                    }
                }
            }
            
            // Build edges between junctions
            foreach (var junction in junctions)
            {
                graph[junction] = new List<((int, int), int)>();
                FindAdjacentJunctions(grid, junction, junctions, graph);
            }
            
            return graph;
        }

        private void FindAdjacentJunctions(char[][] grid, (int, int) start, HashSet<(int, int)> junctions, Dictionary<(int, int), List<((int, int), int)>> graph)
        {
            int rows = grid.Length;
            int cols = grid[0].Length;
            
            var visited = new HashSet<(int, int)> { start };
            var queue = new Queue<((int, int), int)>();
            
            // Start from all neighbors
            int[][] directions = new[] { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };
            
            foreach (var dir in directions)
            {
                int nr = start.Item1 + dir[0];
                int nc = start.Item2 + dir[1];
                
                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && grid[nr][nc] != '#')
                {
                    queue.Enqueue(((nr, nc), 1));
                    visited.Add((nr, nc));
                }
            }
            
            // BFS to find connected junctions
            while (queue.Count > 0)
            {
                var (pos, dist) = queue.Dequeue();
                
                if (junctions.Contains(pos) && pos != start)
                {
                    graph[start].Add((pos, dist));
                    continue;
                }
                
                foreach (var dir in directions)
                {
                    int nr = pos.Item1 + dir[0];
                    int nc = pos.Item2 + dir[1];
                    
                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && 
                        grid[nr][nc] != '#' && !visited.Contains((nr, nc)))
                    {
                        visited.Add((nr, nc));
                        queue.Enqueue(((nr, nc), dist + 1));
                    }
                }
            }
        }

        private long DFSGraph(Dictionary<(int, int), List<((int, int), int)>> graph, (int, int) current, (int, int) end, HashSet<(int, int)> visited)
        {
            if (current == end)
                return 0;
            
            visited.Add(current);
            long maxSteps = -1;
            
            if (graph.ContainsKey(current))
            {
                foreach (var (neighbor, distance) in graph[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        long steps = DFSGraph(graph, neighbor, end, visited);
                        if (steps >= 0)
                            maxSteps = Math.Max(maxSteps, steps + distance);
                    }
                }
            }
            
            visited.Remove(current);
            return maxSteps;
        }
    }
}
