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

using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 16)]
    public class Year2023Day16 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day16()
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
            var input = FileIOHelper.getInstance().GetDataAsDoubleCharArray(file);



            _SW.Start();

            int result = CountEnergized(TraverseTiles(input));
            
            _SW.Stop();

            Console.WriteLine($"  Part 1: Energized Light: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Restart();
            
            _SW.Start(); 
            result = CountMaxEnergized(input);
            _SW.Stop();

            Console.WriteLine($"  Part 2: Max Energized Light: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        private int CountEnergized(List<Utilities.Direction>[][] tiles) => tiles.Sum(r => r.Count(b => b.Count > 0));

        private int CountMaxEnergized(char[][] mirrors)
        {
            var maxY = mirrors.Length;
            var maxX = mirrors[0].Length;
            var max = 0;

            for (var x = 0; x < maxX; x++)
            {
                FindMaxEnergized(0, x, Utilities.Direction.S);
                FindMaxEnergized(maxY - 1, x, Utilities.Direction.N);
            }
            for (var y = 0; y < maxY; y++)
            {
                FindMaxEnergized(y, 0, Utilities.Direction.E);
                FindMaxEnergized(y, maxX - 1, Utilities.Direction.W);
            }
            return max;

            void FindMaxEnergized(int y, int x, Utilities.Direction direction)
                => max = Math.Max(max, CountEnergized(TraverseTiles(mirrors, y, x, direction)));
        }

        private List<Utilities.Direction>[][] TraverseTiles(char[][] mirrors,
            int startY = 0, int startX = 0, Utilities.Direction startDirection = Utilities.Direction.E)
        {
            var maxY = mirrors.Length;
            var maxX = mirrors[0].Length;

            var tiles = new List<Utilities.Direction>[maxY][];
            for (var y = 0; y < maxY; y++)
            {
                tiles[y] = new List<Utilities.Direction>[maxX];
                for (var x = 0; x < maxX; x++) 
                    tiles[y][x] = new List<Utilities.Direction>();
            }

            var beams = new Queue<(int y, int x, Utilities.Direction direction)>();
            beams.Enqueue((startY, startX, startDirection));

            while (beams.Count > 0)
            {
                var (y, x, direction) = beams.Dequeue();

                var tile = tiles[y][x];

                // if beam already exists, skip
                if (tile.Contains(direction)) continue;

                // add beam to mirror tile
                tile.Add(direction);

                if (mirrors[y][x] == '/')
                    direction = direction switch
                    {
                        Utilities.Direction.N => Utilities.Direction.E,
                        Utilities.Direction.E => Utilities.Direction.N,
                        Utilities.Direction.S => Utilities.Direction.W,
                        Utilities.Direction.W => Utilities.Direction.S,
                        _ => direction
                    };
                else if (mirrors[y][x] == '\\')
                    direction = direction switch
                    {
                        Utilities.Direction.N => Utilities.Direction.W,
                        Utilities.Direction.E => Utilities.Direction.S,
                        Utilities.Direction.S => Utilities.Direction.E,
                        Utilities.Direction.W => Utilities.Direction.N,
                        _ => direction
                    };
                else if (mirrors[y][x] == '-')
                {
                    if (direction is Utilities.Direction.N or Utilities.Direction.S)
                    {
                        ContinueBeam(y, x, Utilities.Direction.W);
                        ContinueBeam(y, x, Utilities.Direction.E);
                        continue; // skip to next beam
                    }
                }
                else if (mirrors[y][x] == '|')
                    if (direction is Utilities.Direction.E or Utilities.Direction.W)
                    {
                        ContinueBeam(y, x, Utilities.Direction.N);
                        ContinueBeam(y, x, Utilities.Direction.S);
                        continue; // skip to next beam
                    }

                ContinueBeam(y, x, direction);
            }

            return tiles;

            void ContinueBeam(int y, int x, Utilities.Direction direction)
            {
                switch (direction)
                {
                    case Utilities.Direction.N:
                        y--;
                        break;
                    case Utilities.Direction.E:
                        x++;
                        break;
                    case Utilities.Direction.S:
                        y++;
                        break;
                    case Utilities.Direction.W:
                        x--;
                        break;
                }

                if (x >= 0 && y >= 0 && x < maxX && y < maxY)
                    beams.Enqueue((y, x, direction));
            }
        }


    }
}