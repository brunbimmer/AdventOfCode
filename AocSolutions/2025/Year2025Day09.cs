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
    [AdventOfCode(Year = 2025, Day = 9)]
    public class Year2025Day09 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day09()
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

            var tiles = ParseTiles(input);

            _SW.Stop();
            Console.WriteLine($"  Parsed {tiles.Count} tiles");
            Console.WriteLine("  Execution Time to Prepare Data: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part1 = SolvePart1(tiles);

            _SW.Stop();
            Console.WriteLine($"  Part 1 - Largest Area between two coordinates: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part2 = SolvePart2(tiles);

            _SW.Stop();
            Console.WriteLine($"  Part 2 - Largest Filled Red/Green Inclusive Area: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private List<(int col, int row)> ParseTiles(string[] lines)
        {
            var tiles = new List<(int, int)>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');
                if (parts.Length != 2)
                    continue;

                int col = int.Parse(parts[0].Trim());
                int row = int.Parse(parts[1].Trim());
                tiles.Add((col, row));
            }

            return tiles;
        }

        private long SolvePart1(List<(int col, int row)> tiles)
        {
            long maxArea = 0;

            // Try all pairs of red tiles as opposite corners
            for (int i = 0; i < tiles.Count; i++)
            {
                for (int j = i + 1; j < tiles.Count; j++)
                {
                    int col1 = tiles[i].col;
                    int row1 = tiles[i].row;
                    int col2 = tiles[j].col;
                    int row2 = tiles[j].row;

                    // Calculate rectangle area (width * height)
                    // Include the tiles themselves, so width = |col2 - col1| + 1
                    long width = Math.Abs(col2 - col1) + 1;
                    long height = Math.Abs(row2 - row1) + 1;
                    long area = width * height;

                    maxArea = Math.Max(maxArea, area);
                }
            }

            return maxArea;
        }

        private long SolvePart2(List<(int col, int row)> redTiles)
        {
            // Find horizontal and vertical edges
            var (horizontalEdges, verticalEdges) = FindEdges(redTiles);
            
            // Create coordinate set for faster lookup
            var coordSet = new HashSet<(int, int)>(redTiles);
            
            long maxArea = 0;

            // Try all pairs of red tiles as opposite corners
            for (int i = 0; i < redTiles.Count; i++)
            {
                for (int j = i + 1; j < redTiles.Count; j++)
                {
                    var X1 = redTiles[i];
                    var X2 = redTiles[j];

                    // Check if this rectangle is valid
                    if (IsRectangleValid(X1, X2, coordSet, verticalEdges, horizontalEdges))
                    {
                        long width = Math.Abs(X2.Item1 - X1.Item1) + 1;
                        long height = Math.Abs(X2.Item2 - X1.Item2) + 1;
                        long area = width * height;
                        maxArea = Math.Max(maxArea, area);
                    }
                }
            }

            return maxArea;
        }

        private (List<(int, int, int)>, List<(int, int, int)>) FindEdges(List<(int col, int row)> redTiles)
        {
            var horizontal = new List<(int, int, int)>(); // (row, minCol, maxCol)
            var vertical = new List<(int, int, int)>();   // (col, minRow, maxRow)

            int x0 = redTiles[0].Item1;
            int y0 = redTiles[0].Item2;

            foreach (var tile in redTiles.Skip(1).Append(redTiles[0]))
            {
                int x = tile.Item1;
                int y = tile.Item2;

                if (x == x0)
                {
                    // Vertical edge at x
                    int minY = Math.Min(y0, y);
                    int maxY = Math.Max(y0, y);
                    vertical.Add((x0, minY, maxY));
                }
                else
                {
                    // Horizontal edge at y
                    int minX = Math.Min(x0, x);
                    int maxX = Math.Max(x0, x);
                    horizontal.Add((y0, minX, maxX));
                }

                x0 = x;
                y0 = y;
            }

            return (horizontal, vertical);
        }

        private bool IsRectangleValid(
            (int, int) X1, (int, int) X2,
            HashSet<(int, int)> coordSet,
            List<(int, int, int)> verticalEdges,
            List<(int, int, int)> horizontalEdges)
        {
            int xMin = Math.Min(X1.Item1, X2.Item1);
            int xMax = Math.Max(X1.Item1, X2.Item1);
            int yMin = Math.Min(X1.Item2, X2.Item2);
            int yMax = Math.Max(X1.Item2, X2.Item2);

            // Check all corners
            var corners = new[] { (xMin, yMin), (xMin, yMax), (xMax, yMin), (xMax, yMax) };

            foreach (var corner in corners)
            {
                if (!coordSet.Contains(corner))
                {
                    // Corner is not a red tile, check if inside polygon
                    if (!PointInPolygon(corner, verticalEdges))
                    {
                        return false;
                    }
                }
            }

            // Check horizontal polygon edges
            foreach (var (yEdge, xStart, xEnd) in horizontalEdges)
            {
                if (yEdge > yMin && yEdge < yMax) // strictly between yMin and yMax
                {
                    if (!(xEnd <= xMin || xStart >= xMax)) // edge crosses rectangle
                    {
                        return false;
                    }
                }
            }

            // Check vertical polygon edges
            foreach (var (xEdge, yStart, yEnd) in verticalEdges)
            {
                if (xEdge > xMin && xEdge < xMax) // strictly between xMin and xMax
                {
                    if (!(yEnd <= yMin || yStart >= yMax)) // edge crosses rectangle
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool PointInPolygon((int, int) point, List<(int, int, int)> verticalEdges)
        {
            int x = point.Item1;
            int y = point.Item2;
            int crossings = 0;

            foreach (var (xEdge, yStart, yEnd) in verticalEdges)
            {
                if (yStart <= y && y < yEnd && xEdge < x)
                {
                    crossings++;
                }
            }

            return crossings % 2 == 1;
        }

    }
}