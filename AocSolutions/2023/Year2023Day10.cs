using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AdventFileIO;
using Common;

using static Common.Utilities;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 10)]
    public class Year2023Day10 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day10()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

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
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            int length = Solve(input);

            Console.WriteLine($"  Part 1: Length of longest path: {length}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Stop();

            int tiles = SolvePart2(input);

            Console.WriteLine($"  Part 2: Number of Tiles enclosed by loop: {tiles}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");
        }

        public int Solve(string[] lines)
        {
            var (start, sqs) = FetchMaze(lines);
            return FollowTunnels(start, sqs);
        }

        private ((int, int), Dictionary<(int, int), ((int, int), (int, int))>) FetchMaze(string[] lines)
        {
            var sqs = new Dictionary<(int, int), ((int, int), (int, int))>();
            
            (int, int) start = (-1, -1);

            for (int r = 0; r < lines.Length; r++)
            {
                for (int c = 0; c < lines[r].Length; c++)
                {
                    if (lines[r][c] == '.')
                        continue;

                    if (lines[r][c] == 'S')
                        start = (r, c);
                    else
                    {
                        var exits = lines[r][c] switch
                        {
                            '|' => ((-1, 0), (1, 0)),
                            '-' => ((0, -1), (0, 1)),
                            'L' => ((-1, 0), (0, 1)),
                            'J' => ((-1, 0), (0, -1)),
                            '7' => ((1, 0), (0, -1)),
                            'F' => ((1, 0), (0, 1)),
                            _ => throw new Exception("Hmm this shouldn't happen")
                        };
                        sqs.Add((r, c), ((r + exits.Item1.Item1, c + exits.Item1.Item2), (r + exits.Item2.Item1, c + exits.Item2.Item2)));
                    }
                }
            }

            // Add the connections for the start square, ie the two squares that connect to it
            var startAdj = sqs.Keys.Where(k => sqs[k].Item1 == start || sqs[k].Item2 == start).ToList();
            sqs.Add(start, (startAdj[0], startAdj[1]));

            return (start, sqs);
        }

        private int FollowTunnels((int, int) start, Dictionary<(int, int), ((int, int), (int, int))> sqs)
        {
            int steps = 1;
            (int, int) a = sqs[start].Item1;
            (int, int) b = sqs[start].Item2;
            (int, int) prevA = start, prevB = start;

            do
            {
                var nextA = sqs[a].Item1 == prevA ? sqs[a].Item2 : sqs[a].Item1;
                var nextB = sqs[b].Item1 == prevB ? sqs[b].Item2 : sqs[b].Item1;
                prevA = a;
                prevB = b;
                a = nextA;
                b = nextB;
                ++steps;
            } while (a != b);

            return steps;
        }

        public int SolvePart2(string[] lines)
        {
            var maze = FetchMazePart2(lines);
            FloodFill(maze);
            return CountInner(maze);
        }


        private void Scale(int row, int col, char ch, char[,] maze)
        {
            (int, int, char)[] pattern = ch switch
            {
                '.' => [ (0, 0, '.'), (0, 1, '.'), (0, 2, '.'), (1, 0, '.'), (1, 1, '.'), (1, 2, '.'), (2, 0, '.'), (2, 1, '.'), (2, 2, '.') ],
                'S' => [ (0, 0, '.'), (0, 1, 'S'), (0, 2, '.'), (1, 0, 'S'), (1, 1, 'S'), (1, 2, 'S'), (2, 0, '.'), (2, 1, 'S'), (2, 2, '.') ],
                '|' => [ (0, 0, '.'), (0, 1, '|'), (0, 2, '.'), (1, 0, '.'), (1, 1, '|'), (1, 2, '.'), (2, 0, '.'), (2, 1, '|'), (2, 2, '.') ],
                '-' => [ (0, 0, '.'), (0, 1, '.'), (0, 2, '.'), (1, 0, '-'), (1, 1, '-'), (1, 2, '-'), (2, 0, '.'), (2, 1, '.'), (2, 2, '.') ],
                'L' => [ (0, 0, '.'), (0, 1, '|'), (0, 2, '.'), (1, 0, '.'), (1, 1, '+'), (1, 2, '-'), (2, 0, '.'), (2, 1, '.'), (2, 2, '.') ],
                'J' => [ (0, 0, '.'), (0, 1, '|'), (0, 2, '.'), (1, 0, '-'), (1, 1, '+'), (1, 2, '.'), (2, 0, '.'), (2, 1, '.'), (2, 2, '.') ],
                '7' => [ (0, 0, '.'), (0, 1, '.'), (0, 2, '.'), (1, 0, '-'), (1, 1, '+'), (1, 2, '.'), (2, 0, '.'), (2, 1, '|'), (2, 2, '.') ],
                'F' => [ (0, 0, '.'), (0, 1, '.'), (0, 2, '.'), (1, 0, '.'), (1, 1, '+'), (1, 2, '-'), (2, 0, '.'), (2, 1, '|'), (2, 2, '.') ],
                _ => throw new Exception("hmm this shouldn't happen")
            };

            foreach (var p in pattern)
                maze[row + p.Item1, col + p.Item2] = p.Item3;
        }

        private char[,] FetchMazePart2(string[] lines)
        {
            int length = lines.Length;
            int width = lines[0].Length;

            // I'm going to put a border of .s around the actual maze to make 
            // the flood fill simpler (if longer)
            char[,] maze = new char[(length + 1) * 3, (width + 1) * 3];
            for (int r = 0; r < maze.GetLength(0) - 1; r++)
            {
                for (int c = 0; c < maze.GetLength(1) - 1; c++)
                {
                    maze[r, c] = '.';
                }
            }

            for (int r = 0; r < length; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    Scale(r * 3 + 1, c * 3 + 1, lines[r][c], maze);
                }
            }

            return maze;
        }

        private void FloodFill(char[,] maze)
        {
            var neighbours = new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
            var length = maze.GetLength(0);
            var width = maze.GetLength(1);
            var locs = new Queue<(int, int)>();
            locs.Enqueue((0, 0));
            var visited = new HashSet<(int, int)>();

            do
            {
                var loc = locs.Dequeue();
                if (visited.Contains(loc))
                    continue;
                maze[loc.Item1, loc.Item2] = 'o';
                visited.Add((loc.Item1, loc.Item2));

                foreach (var n in neighbours)
                {
                    var nr = loc.Item1 + n.Item1;
                    var nc = loc.Item2 + n.Item2;
                    if (nr < 0 || nr >= length || nc < 0 || nc >= width || visited.Contains((nr, nc)))
                        continue;
                    if (maze[nr, nc] == '.')
                        locs.Enqueue((nr, nc));
                }
            }
            while (locs.Count > 0);
        }

        private int CountInner(char[,] maze)
        {
            var length = maze.GetLength(0) - 1;
            var width = maze.GetLength(1) - 1;
            int count = 0;
            var pixels = new (int, int)[] { (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 2) };

            for (int r = 1; r < length; r += 3)
            {
                for (int c = 1; c < width; c += 3)
                {
                    bool isInner = true;
                    foreach (var p in pixels)
                    {
                        if (maze[r + p.Item1, c + p.Item2] == 'o')
                        {
                            isInner = false;
                            break;
                        }
                    }
                    if (isInner) ++count;
                }
            }

            return count;
        }



    }

}
