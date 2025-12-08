using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2025, Day = 8)]
    public class Year2025Day08 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        class Circuit
        {
            public int Count { get; set; }
            public HashSet<int> Boxes { get; set; }

            public Circuit(int count, HashSet<int> boxes)
            {
                Count = count;
                Boxes = boxes;
            }
        }

        public Year2025Day08()
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
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            long part1 = Solve(input);

            _SW.Stop();

            Console.WriteLine($"  Part 1: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part2 = Solve(input, true);

            _SW.Stop();

            Console.WriteLine($"  Part 2: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private long Solve(string[] lines, bool solvePart2 = false)
        {
            // Parse junction boxes to Coordinate3D
            var boxes = ParseBoxes(lines);
            
            // Generate all pairs with EuclideanDistance
            var edges = new List<(double distance, int a, int b)>();
            for (int i = 0; i < boxes.Count; i++)
            {
                for (int j = i + 1; j < boxes.Count; j++)
                {
                    double dist = EuclideanDistance(boxes[i], boxes[j]);
                    edges.Add((dist, i, j));
                }
            }
            
            // Sort by distance (shortest to longest)
            edges.Sort((a, b) => a.distance.CompareTo(b.distance));
                       
            // Dictionary to track circuits: Value is circuit info
            var circuits = new HashSet<Circuit>();

            long lastConnectionX1 = 0;
            long lastConnectionX2 = 0;


            //Circuit Counter
            int MaxConnections = 10;

            int counter = 0;

            bool _bDone = false;

            foreach (var (distance, a, b) in edges)
            {
                if (!solvePart2) counter++; //count 1000 connections only for part 1

                if (_bDone) break;

                var circuit = circuits.Where(c => c.Boxes.Contains(a) || c.Boxes.Contains(b)).ToList();

                if (circuit.Count() != 0)
                {
                    if (circuit.Count() > 1)
                    {
                        // Merge circuits if both boxes are in different circuits
                        var circuitsList = circuit.ToList();
                        var firstCircuit = circuitsList[0];
                        for (int i = 1; i < circuitsList.Count; i++)
                        {
                            var toMerge = circuitsList[i];
                            firstCircuit.Count += toMerge.Count;
                            foreach (var box in toMerge.Boxes)
                            {
                                firstCircuit.Boxes.Add(box);
                            }
                            circuits.Remove(toMerge);

                    
                            // If all boxes are now in one circuit, we're done
                            if (circuits.Count == 1)
                            {
                                // Track this as the last connection
                                lastConnectionX1 = boxes[a].X;
                                lastConnectionX2 = boxes[b].X;

                                if (circuits.First().Boxes.Count() == boxes.Count) 
                                {
                                    _bDone = true;
                                }
                            }
                        }                        
                    }
                    else 
                    {
                        if (circuit.First().Boxes.Contains(a) && circuit.First().Boxes.Contains(b))
                        {
                            // Both boxes already in the same circuit, skip to avoid cycle
                            continue;
                        }

                        circuit.First().Boxes.Add(a);    //Gets added if not present
                        circuit.First().Boxes.Add(b);    //Gets added if not present
                        circuit.First().Count += 1;      //Increase circuit size

                        if (circuits.First().Boxes.Count() == boxes.Count) 
                        {
                            // Track this as the last connection
                            lastConnectionX1 = boxes[a].X;
                            lastConnectionX2 = boxes[b].X;
                            _bDone = true;
                        }
                    }
                }
                else 
                {
                    Circuit newCircuit = new Circuit(2, new HashSet<int> { a, b });
                    circuits.Add(newCircuit);
                }

                if (counter >= MaxConnections) break;
            }
            
            if (solvePart2)
            {
                // For part 2, return the size of the largest circuit
                return lastConnectionX1 * lastConnectionX2;
            }
            else
            {
                // Get the 3 largest circuit sizes and multiply
                var sizes = circuits.Select(c => c.Count).OrderByDescending(s => s).Take(3);
                return sizes.Aggregate(1L, (acc, size) => acc * size);
            }
        }

        private List<Coordinate3D> ParseBoxes(string[] lines)
        {
            var boxes = new List<Coordinate3D>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var parts = line.Split(',');
                if (parts.Length != 3)
                    continue;
                long x = long.Parse(parts[0].Trim());
                long y = long.Parse(parts[1].Trim());
                long z = long.Parse(parts[2].Trim());
                boxes.Add(new Coordinate3D((int)x, (int)y, (int)z));
            }
            return boxes;
        }

        private double EuclideanDistance(Coordinate3D a, Coordinate3D b)
        {
            long dx = a.X - b.X;
            long dy = a.Y - b.Y;
            long dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
