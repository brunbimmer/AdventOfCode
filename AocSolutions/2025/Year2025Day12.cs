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
    [AdventOfCode(Year = 2025, Day = 12)]
    public class Year2025Day12 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day12()
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

            var (shapes, regions) = ParseInput(input);

            _SW.Stop();
            Console.WriteLine($"  Parsed {shapes.Count} shapes and {regions.Count} regions");
            Console.WriteLine("  Execution Time to Prepare Data: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part1 = SolvePart1(shapes, regions);

            _SW.Stop();
            Console.WriteLine($"  Part 1 - Regions that fit all presents: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

        }

        private class Shape
        {
            public int Index { get; set; }
            public int Area { get; set; }  // Number of cells in the shape
        }

        private class Region
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public List<int> Needs { get; set; }
        }

        private (List<Shape>, List<Region>) ParseInput(string[] lines)
        {
            var shapes = new List<Shape>();
            var regions = new List<Region>();

            int i = 0;

            // Parse shapes section
            while (i < lines.Length)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    i++;
                    continue;
                }

                // Check for shape definition: "0:", "1:", etc.
                if (line.EndsWith(":") && !line.Contains("x") && int.TryParse(line.TrimEnd(':'), out int shapeIdx))
                {
                    var shapeGrid = new List<string>();
                    i++;

                    while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]) && !lines[i].Contains(":"))
                    {
                        shapeGrid.Add(lines[i]);
                        i++;
                    }

                    if (shapeGrid.Count > 0)
                    {
                        // Count the cells in the shape
                        int area = 0;
                        foreach (var row in shapeGrid)
                        {
                            area += row.Count(c => c == '#');
                        }
                        shapes.Add(new Shape { Index = shapeIdx, Area = area });
                    }
                    continue;
                }

                // Check for region definition: "4x4:", "12x5:", etc.
                if (line.Contains("x") && line.Contains(":"))
                {
                    var colonIdx = line.IndexOf(':');
                    var dimStr = line.Substring(0, colonIdx);
                    var dims = dimStr.Split('x');

                    if (int.TryParse(dims[0], out int w) && int.TryParse(dims[1], out int h))
                    {
                        var needsStr = line.Substring(colonIdx + 1).Trim();
                        var needs = needsStr.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList();

                        regions.Add(new Region { Width = w, Height = h, Needs = needs });
                    }
                }

                i++;
            }

            return (shapes, regions);
        }

        private long SolvePart1(List<Shape> shapes, List<Region> regions)
        {
            int count = 0;
            foreach (var region in regions)
            {
                int totalArea = region.Height * region.Width;
                int neededArea = 0;

                for (int i = 0; i < region.Needs.Count && i < shapes.Count; i++)
                {
                    neededArea += shapes[i].Area * region.Needs[i];
                }

                // Heuristic: area must be sufficient and packing efficiency must be reasonable (≤ 85%)
                double efficiency = (double)neededArea / totalArea;
                if (totalArea >= neededArea && efficiency <= 0.85)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
