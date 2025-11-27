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
    [AdventOfCode(Year = 2016, Day = 11)]
    public class Year2016Day11 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        // State: elevator floor (0-3) + item locations encoded as bitmask
        // Each item gets a pair of bits (generator, microchip)
        private record State(int ElevatorFloor, int[] ItemFloors)
        {
            public override string ToString()
            {
                return $"E{ElevatorFloor}:{string.Join(",", ItemFloors)}";
            }
        }

        public Year2016Day11()
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

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] lines = FileIOHelper.getInstance().ReadDataAsString(file).Split("\n", StringSplitOptions.RemoveEmptyEntries);

            _SW.Start();
            
            int[] itemFloors = ParseInput(lines);
            int part1 = FindMinSteps(itemFloors);
            
            _SW.Stop();
            Console.WriteLine("Part 1: {0}, Execution Time: {1}", part1, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            // Part 2: Add 4 more items (2 generators, 2 microchips) to floor 0
            int[] itemFloorsPart2 = new int[itemFloors.Length + 4];
            Array.Copy(itemFloors, itemFloorsPart2, itemFloors.Length);
            for (int i = itemFloors.Length; i < itemFloorsPart2.Length; i++)
            {
                itemFloorsPart2[i] = 0;
            }
            
            int part2 = FindMinSteps(itemFloorsPart2);
            
            _SW.Stop();
            Console.WriteLine("Part 2: {0}, Execution Time: {1}", part2, StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private int[] ParseInput(string[] lines)
        {
            // Map element names to indices
            Dictionary<string, int> elementIndex = new();
            int nextIndex = 0;
            List<int> itemFloors = new();

            for (int floorIdx = 0; floorIdx < lines.Length; floorIdx++)
            {
                string line = lines[floorIdx];
                
                // Extract generators
                MatchCollection genMatches = Regex.Matches(line, @"(\w+) generator");
                foreach (Match m in genMatches)
                {
                    string element = m.Groups[1].Value;
                    if (!elementIndex.ContainsKey(element))
                        elementIndex[element] = nextIndex++;
                    
                    // Ensure array is large enough
                    while (itemFloors.Count <= elementIndex[element] * 2)
                        itemFloors.Add(-1);
                    
                    itemFloors[elementIndex[element] * 2] = floorIdx;
                }

                // Extract microchips
                MatchCollection chipMatches = Regex.Matches(line, @"(\w+)-compatible microchip");
                foreach (Match m in chipMatches)
                {
                    string element = m.Groups[1].Value;
                    if (!elementIndex.ContainsKey(element))
                        elementIndex[element] = nextIndex++;
                    
                    // Ensure array is large enough
                    while (itemFloors.Count <= elementIndex[element] * 2 + 1)
                        itemFloors.Add(-1);
                    
                    itemFloors[elementIndex[element] * 2 + 1] = floorIdx;
                }
            }

            return itemFloors.ToArray();
        }

        private int FindMinSteps(int[] initialFloors)
        {
            State initial = new(0, initialFloors);
            Queue<(State, int)> queue = new();
            HashSet<string> visited = new();

            queue.Enqueue((initial, 0));
            visited.Add(CanonicalState(initial));

            while (queue.Count > 0)
            {
                var (state, steps) = queue.Dequeue();

                // Goal: all items on floor 3
                if (state.ItemFloors.All(f => f == 3))
                {
                    return steps;
                }

                // Get items on current floor
                List<int> itemsOnFloor = new();
                for (int i = 0; i < state.ItemFloors.Length; i++)
                {
                    if (state.ItemFloors[i] == state.ElevatorFloor)
                    {
                        itemsOnFloor.Add(i);
                    }
                }

                // Try moving 1 or 2 items to adjacent floors
                int[] nextFloors = { state.ElevatorFloor - 1, state.ElevatorFloor + 1 };

                foreach (int nextFloor in nextFloors)
                {
                    if (nextFloor < 0 || nextFloor > 3) continue;

                    // Try taking 1 item
                    foreach (int item in itemsOnFloor)
                    {
                        int[] newFloors = (int[])state.ItemFloors.Clone();
                        newFloors[item] = nextFloor;

                        if (IsValidState(newFloors))
                        {
                            State newState = new(nextFloor, newFloors);
                            string canonical = CanonicalState(newState);
                            if (!visited.Contains(canonical))
                            {
                                visited.Add(canonical);
                                queue.Enqueue((newState, steps + 1));
                            }
                        }
                    }

                    // Try taking 2 items
                    for (int i = 0; i < itemsOnFloor.Count; i++)
                    {
                        for (int j = i + 1; j < itemsOnFloor.Count; j++)
                        {
                            int[] newFloors = (int[])state.ItemFloors.Clone();
                            newFloors[itemsOnFloor[i]] = nextFloor;
                            newFloors[itemsOnFloor[j]] = nextFloor;

                            if (IsValidState(newFloors))
                            {
                                State newState = new(nextFloor, newFloors);
                                string canonical = CanonicalState(newState);
                                if (!visited.Contains(canonical))
                                {
                                    visited.Add(canonical);
                                    queue.Enqueue((newState, steps + 1));
                                }
                            }
                        }
                    }
                }
            }

            return -1; // No solution found
        }

        private bool IsValidState(int[] floors)
        {
            // Check each floor for safety
            for (int floor = 0; floor < 4; floor++)
            {
                List<int> generatorsOnFloor = new();
                List<int> chipsOnFloor = new();

                // Items are stored as pairs: even indices = generators, odd indices = microchips (of same element)
                for (int i = 0; i < floors.Length; i++)
                {
                    if (floors[i] == floor)
                    {
                        if (i % 2 == 0)
                            generatorsOnFloor.Add(i / 2);
                        else
                            chipsOnFloor.Add(i / 2);
                    }
                }

                // If there are generators on this floor, all microchips must have their generator
                if (generatorsOnFloor.Count > 0)
                {
                    foreach (int chipElement in chipsOnFloor)
                    {
                        if (!generatorsOnFloor.Contains(chipElement))
                        {
                            return false; // Chip fried by foreign generator
                        }
                    }
                }
            }

            return true;
        }

        private string CanonicalState(State state)
        {
            // Create canonical representation to avoid redundant states
            // Group items by their pairing pattern
            List<(int, int)> pairs = new();
            for (int i = 0; i < state.ItemFloors.Length; i += 2)
            {
                pairs.Add((state.ItemFloors[i], state.ItemFloors[i + 1]));
            }

            // Sort pairs by their floor pattern to get canonical form
            pairs.Sort();

            StringBuilder sb = new();
            sb.Append(state.ElevatorFloor);
            foreach (var (gen, chip) in pairs)
            {
                sb.Append($":{gen},{chip}");
            }

            return sb.ToString();
        }
    }
}
