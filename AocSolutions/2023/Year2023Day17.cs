using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventFileIO;
using Common;
using static Common.Utilities;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 17)]
    public class Year2023Day17 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day17()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            var map = FileIOHelper.getInstance().GetDataAsDoubleIntArray(file);

            _SW.Start();

            int result = Solve(map, 1, 3);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Lowest Heat loss: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Restart();
            
            _SW.Start();
            result = Solve(map, 4, 10);
            _SW.Stop();

            Console.WriteLine($"  Part 2: Lowest Heat loss: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        int Solve(int[][] map, int minSteps, int maxSteps)
        {
            PriorityQueue<(int y, int x, Direction direction, int directionMoves), int> queue = new();
            Dictionary<(Direction, int), int>[][] visited = new Dictionary<(Direction, int), int>[map.Length][];
            for (var y = 0; y < map.Length; y++)
            {
                visited[y] = new Dictionary<(Direction, int), int>[map[0].Length];
                for (var x = 0; x < map[0].Length; x++)
                    visited[y][x] = new Dictionary<(Direction, int), int>();
            }

            queue.Enqueue((0, 0, Direction.E, 0), 0);
            queue.Enqueue((0, 0, Direction.S, 0), 0);

            while (queue.Count > 0)
            {
                var (y, x, direction, directionMoves) = queue.Dequeue();

                var heat = visited[y][x].GetValueOrDefault((direction, directionMoves));

                if (directionMoves < maxSteps)
                    Move(y, x, direction, heat, directionMoves);

                if (directionMoves >= minSteps)
                {
                    Move(y, x, Left(direction), heat, 0);
                    Move(y, x, Right(direction), heat, 0);
                }
            }

            var maxY = map.Length - 1;
            var maxX = map[0].Length - 1;

            return visited[maxY][maxX].Min(x => x.Value);

            void Move(int y, int x, Direction direction, int heat, int directionMoves)
            {
                var dy = direction switch
                {
                    Direction.N => -1,
                    Direction.S => 1,
                    _ => 0
                };

                var dx = direction switch
                {
                    Direction.E => 1,
                    Direction.W => -1,
                    _ => 0
                };

                for (var i = 1; i <= maxSteps; i++)
                {
                    var newY = y + i * dy;
                    var newX = x + i * dx;
                    var newDirectionMoves = directionMoves + i;

                    if (newY < 0 || newY >= map.Length || newX < 0 || newX >= map[0].Length ||
                        newDirectionMoves > maxSteps)
                        return;

                    heat += map[newY][newX];

                    if (i < minSteps) continue;

                    var vlist = visited[newY][newX];

                    if (vlist.TryGetValue((direction, newDirectionMoves), out var visitedHeat))
                    {
                        if (visitedHeat <= heat)
                            return;
                    }

                    queue.Enqueue((newY, newX, direction, newDirectionMoves), heat);
                    vlist[(direction, newDirectionMoves)] = heat;
                }
            }
        }

        private Direction Left(Direction direction) => direction switch
        {
            Direction.N => Direction.W,
            Direction.W => Direction.S,
            Direction.S => Direction.E,
            Direction.E => Direction.N,
            _ => throw new UnreachableException()
        };
        private Direction Right(Direction direction) => direction switch
        {
            Direction.N => Direction.E,
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N,
            _ => throw new UnreachableException()
        };
    }
}