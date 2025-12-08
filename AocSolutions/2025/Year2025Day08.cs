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

            // Prepare boxes and edges once
            var (boxes, edges) = PrepareData(input);

            _SW.Start();

            long part1 = SolvePart1(boxes, edges);

            _SW.Stop();

            Console.WriteLine($"  Part 1: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part2 = SolvePart2(boxes, edges);

            _SW.Stop();

            Console.WriteLine($"  Part 2: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private (List<Coordinate3D> boxes, List<(double distance, int a, int b)> edges) PrepareData(string[] lines)
        {
            var boxes = ParseBoxes(lines);
            
            // Generate all pairs with Euclidean distance
            var edges = new List<(double distance, int a, int b)>();
            for (int i = 0; i < boxes.Count; i++)
            {
                for (int j = i + 1; j < boxes.Count; j++)
                {
                    edges.Add((EuclideanDistance(boxes[i], boxes[j]), i, j));
                }
            }
            
            // Sort by distance
            edges.Sort((a, b) => a.distance.CompareTo(b.distance));
            
            return (boxes, edges);
        }

        private long SolvePart1(List<Coordinate3D> boxes, List<(double distance, int a, int b)> edges)
        {
            return Solve(boxes, edges, solvePart2: false);
        }

        private long SolvePart2(List<Coordinate3D> boxes, List<(double distance, int a, int b)> edges)
        {
            return Solve(boxes, edges, solvePart2: true);
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

        private long Solve(List<Coordinate3D> boxes, List<(double distance, int a, int b)> edges, bool solvePart2)
        {
            var circuits = new HashSet<Circuit>();
            long lastConnectionX1 = 0;
            long lastConnectionX2 = 0;
            int connectionCount = 0;
            const int MAX_CONNECTIONS_PART1 = 1000;

            foreach (var (distance, a, b) in edges)
            {
                // Stop after 1000 connections for Part 1
                if (!solvePart2) connectionCount++;

                var matchingCircuits = circuits.Where(c => c.Boxes.Contains(a) || c.Boxes.Contains(b)).ToList();

                if (matchingCircuits.Count == 0)
                {
                    // Both boxes are new - create new circuit
                    circuits.Add(new Circuit(2, new HashSet<int> { a, b }));
                }
                else if (matchingCircuits.Count == 1)
                {
                    // One box is in a circuit
                    var circuit = matchingCircuits[0];
                    if (!circuit.Boxes.Contains(a) || !circuit.Boxes.Contains(b))
                    {
                        circuit.Boxes.Add(a);
                        circuit.Boxes.Add(b);
                        circuit.Count++;
                        
                        if (circuit.Boxes.Count == boxes.Count)
                        {
                            lastConnectionX1 = boxes[a].X;
                            lastConnectionX2 = boxes[b].X;
                            if (solvePart2) break;
                        }
                    }
                }
                else
                {
                    // Both boxes are in different circuits - merge them
                    var firstCircuit = matchingCircuits[0];
                    for (int i = 1; i < matchingCircuits.Count; i++)
                    {
                        var toMerge = matchingCircuits[i];
                        firstCircuit.Count += toMerge.Count;
                        foreach (var box in toMerge.Boxes)
                        {
                            firstCircuit.Boxes.Add(box);
                        }
                        circuits.Remove(toMerge);
                    }

                    if (firstCircuit.Boxes.Count == boxes.Count)
                    {
                        lastConnectionX1 = boxes[a].X;
                        lastConnectionX2 = boxes[b].X;
                        if (solvePart2) break;
                    }
                }

                if (connectionCount >= MAX_CONNECTIONS_PART1)
                    break;
            }

            if (solvePart2)
            {
                return lastConnectionX1 * lastConnectionX2;
            }
            else
            {
                var sizes = circuits.Select(c => c.Count).OrderByDescending(s => s).Take(3);
                return sizes.Aggregate(1L, (acc, size) => acc * size);
            }
        }
    }
}
