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
    [AdventOfCode(Year = 2022, Day = 24)]
    public class Year2022Day24 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }


        public Year2022Day24()
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

            var input = FileIOHelper.getInstance().ReadDataAsLines(file);

            var blizzards = ParseBlizzards(input);
            int width = input[0].Length - 2; // Exclude walls
            int height = input.Length - 2;   // Exclude walls
            var start = new Coordinate2D(0, -1);
            var end = new Coordinate2D(width - 1, height);
            
            int cycleTime = LCM(width, height);
            
            // Pre-compute blizzard positions for all times in the cycle
            var blizzardStates = PrecomputeBlizzardStates(blizzards, cycleTime, width, height);

            _SW.Start();
            
            int part1 = FindPath(blizzardStates, start, end, width, height, 0, cycleTime);

            _SW.Stop();

            Console.WriteLine($"  Part 1 - Fewest minutes to reach end goal: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            // Part 2: Go to end, back to start, then to end again
            int timeAfterFirstLeg = FindPath(blizzardStates, start, end, width, height, 0, cycleTime);
            int timeAfterSecondLeg = FindPath(blizzardStates, end, start, width, height, timeAfterFirstLeg, cycleTime);
            int part2 = FindPath(blizzardStates, start, end, width, height, timeAfterSecondLeg, cycleTime);

            _SW.Stop();

            Console.WriteLine($"  Part 2: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private List<(Coordinate2D pos, Coordinate2D dir)> ParseBlizzards(string[] input)
        {
            var blizzards = new List<(Coordinate2D, Coordinate2D)>();
            
            for (int y = 1; y < input.Length - 1; y++)
            {
                for (int x = 1; x < input[y].Length - 1; x++)
                {
                    char c = input[y][x];
                    Coordinate2D dir = c switch
                    {
                        '^' => new Coordinate2D(0, -1),
                        'v' => new Coordinate2D(0, 1),
                        '<' => new Coordinate2D(-1, 0),
                        '>' => new Coordinate2D(1, 0),
                        _ => null
                    };
                    
                    if (dir != null)
                    {
                        // Convert to internal coordinates (0-indexed, excluding walls)
                        blizzards.Add((new Coordinate2D(x - 1, y - 1), dir));
                    }
                }
            }
            
            return blizzards;
        }

        private List<HashSet<Coordinate2D>> PrecomputeBlizzardStates(List<(Coordinate2D pos, Coordinate2D dir)> blizzards, int cycleTime, int width, int height)
        {
            var states = new List<HashSet<Coordinate2D>>(cycleTime);
            
            for (int time = 0; time < cycleTime; time++)
            {
                var positions = new HashSet<Coordinate2D>();
                
                foreach (var (pos, dir) in blizzards)
                {
                    int newX = (pos.X + dir.X * time) % width;
                    if (newX < 0) newX += width;
                    
                    int newY = (pos.Y + dir.Y * time) % height;
                    if (newY < 0) newY += height;
                    
                    positions.Add(new Coordinate2D(newX, newY));
                }
                
                states.Add(positions);
            }
            
            return states;
        }

        private int FindPath(List<HashSet<Coordinate2D>> blizzardStates, Coordinate2D start, Coordinate2D end, int width, int height, int startTime, int cycleTime)
        {
            var queue = new Queue<(Coordinate2D pos, int time)>();
            var visited = new HashSet<(Coordinate2D, int)>();
            
            queue.Enqueue((start, startTime));
            visited.Add((start, startTime % cycleTime));
            
            var directions = new[] { 
                new Coordinate2D(0, 0),  // Wait
                new Coordinate2D(1, 0),  // Right
                new Coordinate2D(-1, 0), // Left
                new Coordinate2D(0, 1),  // Down
                new Coordinate2D(0, -1)  // Up
            };
            
            while (queue.Count > 0)
            {
                var (pos, time) = queue.Dequeue();
                
                if (pos.Equals(end))
                    return time;
                
                int nextTime = time + 1;
                int cycleIndex = nextTime % cycleTime;
                var blizzardPositions = blizzardStates[cycleIndex];
                
                foreach (var dir in directions)
                {
                    var nextPos = pos + dir;
                    
                    // Check bounds (allow start and end positions outside grid)
                    bool isValid = false;
                    if (nextPos.Equals(start) || nextPos.Equals(end))
                    {
                        isValid = true;
                    }
                    else if (nextPos.X >= 0 && nextPos.X < width && nextPos.Y >= 0 && nextPos.Y < height)
                    {
                        isValid = true;
                    }
                    
                    // Check if there's a blizzard at this position
                    if (isValid && !blizzardPositions.Contains(nextPos))
                    {
                        var state = (nextPos, cycleIndex);
                        if (!visited.Contains(state))
                        {
                            visited.Add(state);
                            queue.Enqueue((nextPos, nextTime));
                        }
                    }
                }
            }
            
            return -1; // Should never reach here
        }

        private int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private int LCM(int a, int b)
        {
            return (a / GCD(a, b)) * b;
        }
    }
}
