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
    [AdventOfCode(Year = 2023, Day = 21)]
    public class Year2023Day21: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day21()
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

            _SW.Start();

            long part1 = SolvePart1(lines);

            _SW.Stop();

            output.WritePart(1, part1, trackTime ? _SW.Elapsed : null, "");

            
            _SW.Restart();

            long part2 = SolvePart2(lines);

            _SW.Stop();

            output.WritePart(2, part2, trackTime ? _SW.Elapsed : null, "");

        }

        private long SolvePart1(string[] lines)
        {
            int gridSize = lines.Length;
            var visited = BFS(lines);
            int distanceToEdge = gridSize / 2;  // 65
            
            // Count squares with even parity and distance < 65
            return visited.Values.Count(v => v < distanceToEdge && v % 2 == 0);
        }

        private long SolvePart2(string[] lines)
        {
            int gridSize = lines.Length;
            var visited = BFS(lines);
            long distanceToEdge = gridSize / 2;  // 65
            long n = (26501365 - distanceToEdge) / gridSize;  // 202300
            
            // Count all points by parity
            long oddParityCount = visited.Values.Count(v => v % 2 == 1);
            long evenParityCount = visited.Values.Count(v => v % 2 == 0);
            
            // Count corners (points farther than distance_to_edge with specific parity)
            long oddCorners = visited.Values.Count(v => v > distanceToEdge && v % 2 == 1);
            long evenCorners = visited.Values.Count(v => v > distanceToEdge && v % 2 == 0);
            
            // Number of each type of tile
            long numOddTiles = (n + 1) * (n + 1);
            long numEvenTiles = n * n;
            
            // Apply formula
            long result = numOddTiles * oddParityCount
                        + numEvenTiles * evenParityCount
                        - (n + 1) * oddCorners
                        + n * evenCorners;
            
            return result;
        }

        // BFS to find shortest distance from start to all reachable cells within the single grid
        private Dictionary<(int, int), int> BFS(string[] lines)
        {
            int gridSize = lines.Length;
            var visited = new Dictionary<(int, int), int>();
            var queue = new Queue<((int, int), int)>();
            
            // Find starting position
            (int, int) start = (0, 0);
            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < lines[r].Length; c++)
                {
                    if (lines[r][c] == 'S')
                    {
                        start = (r, c);
                        break;
                    }
                }
            }
            
            queue.Enqueue((start, 0));
            visited[start] = 0;
            
            int[][] directions = new[] { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };
            
            while (queue.Count > 0)
            {
                var ((r, c), distance) = queue.Dequeue();
                
                foreach (var dir in directions)
                {
                    int nr = r + dir[0];
                    int nc = c + dir[1];
                    
                    if (nr < 0 || nr >= gridSize || nc < 0 || nc >= lines[nr].Length)
                        continue;
                    
                    if (lines[nr][nc] == '#')
                        continue;
                    
                    if (visited.ContainsKey((nr, nc)))
                        continue;
                    
                    visited[(nr, nc)] = distance + 1;
                    queue.Enqueue(((nr, nc), distance + 1));
                }
            }
            
            return visited;
        }
    }
}
