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
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 8)]
    public class Year2024Day08: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2024Day08()
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

            // Parse the grid to find all antennas by frequency
            var antennasByFrequency = ParseAntennas(lines);
            int maxY = lines.Length;
            int maxX = lines[0].Length;

            _SW.Start();

            long part1 = CountAntinodes(antennasByFrequency, maxX, maxY, false);

            _SW.Stop();

            Console.WriteLine($"  Part 1 (Antinodes from harmonic resonance): {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            long part2 = CountAntinodes(antennasByFrequency, maxX, maxY, true);

            _SW.Stop();

            Console.WriteLine($"  Part 2 (Antinodes along full collinear paths): {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        private sealed record Position(int X, int Y);

        private Dictionary<char, List<Position>> ParseAntennas(string[] lines)
        {
            var result = new Dictionary<char, List<Position>>();
            
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    char c = lines[y][x];
                    if (c != '.')
                    {
                        if (!result.ContainsKey(c))
                            result[c] = new List<Position>();
                        result[c].Add(new Position(x, y));
                    }
                }
            }
            
            return result;
        }

        private long CountAntinodes(Dictionary<char, List<Position>> antennasByFrequency, 
            int maxX, int maxY, bool isPartTwo)
        {
            var antinodes = new HashSet<Position>();

            // For each frequency, find all pairs and calculate their antinodes
            foreach (var frequency in antennasByFrequency.Keys)
            {
                var positions = antennasByFrequency[frequency];
                
                // Generate all pairs of antennas with the same frequency
                for (int i = 0; i < positions.Count; i++)
                {
                    for (int j = i + 1; j < positions.Count; j++)
                    {
                        var a = positions[i];
                        var b = positions[j];
                        
                        // Calculate the difference vector
                        int dx = b.X - a.X;
                        int dy = b.Y - a.Y;
                        
                        if (isPartTwo)
                        {
                            // Part 2: Add all points along the line in both directions
                            // From a going backwards (subtracting the difference)
                            int x = a.X - dx;
                            int y = a.Y - dy;
                            while (x >= 0 && x < maxX && y >= 0 && y < maxY)
                            {
                                antinodes.Add(new Position(x, y));
                                x -= dx;
                                y -= dy;
                            }
                            
                            // From b going forwards (adding the difference)
                            x = b.X + dx;
                            y = b.Y + dy;
                            while (x >= 0 && x < maxX && y >= 0 && y < maxY)
                            {
                                antinodes.Add(new Position(x, y));
                                x += dx;
                                y += dy;
                            }
                            
                            // Part 2 also includes the antenna positions themselves
                            antinodes.Add(a);
                            antinodes.Add(b);
                        }
                        else
                        {
                            // Part 1: Add the two antinodes (one on each side)
                            // One antinode at position b + difference (beyond b)
                            int x1 = b.X + dx;
                            int y1 = b.Y + dy;
                            if (x1 >= 0 && x1 < maxX && y1 >= 0 && y1 < maxY)
                                antinodes.Add(new Position(x1, y1));
                            
                            // One antinode at position a - difference (beyond a in opposite direction)
                            int x2 = a.X - dx;
                            int y2 = a.Y - dy;
                            if (x2 >= 0 && x2 < maxX && y2 >= 0 && y2 < maxY)
                                antinodes.Add(new Position(x2, y2));
                        }
                    }
                }
            }
            
            return antinodes.Count;
        }
    }
}
