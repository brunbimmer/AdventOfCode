using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 24)]
    public class Year2020Day24 : IAdventOfCode
    {
        private static readonly Dictionary<string, (int Q, int R)> Directions = new(StringComparer.Ordinal)
        {
            ["e"] = (1, 0),
            ["se"] = (0, 1),
            ["sw"] = (-1, 1),
            ["w"] = (-1, 0),
            ["nw"] = (0, -1),
            ["ne"] = (1, -1)
        };

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day24()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            var blackTiles = new HashSet<(int Q, int R)>();
            foreach (string instruction in instructions.Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                var tile = FollowPath(instruction.Trim());
                if (!blackTiles.Add(tile))
                    blackTiles.Remove(tile);
            }
            _SW.Stop();

            output.WritePart(1, blackTiles.Count, trackTime ? _SW.Elapsed : null, "Number of tiles with the black side up");

            _SW.Restart();
            for (int day = 0; day < 100; day++)
                blackTiles = NextDay(blackTiles);
            _SW.Stop();

            output.WritePart(2, blackTiles.Count, trackTime ? _SW.Elapsed : null, "Black tiles after 100 days");
        }

        private HashSet<(int Q, int R)> NextDay(HashSet<(int Q, int R)> blackTiles)
        {
            var neighborCounts = new Dictionary<(int Q, int R), int>();
            foreach (var tile in blackTiles)
            {
                foreach (var direction in Directions.Values)
                {
                    var neighbor = (tile.Q + direction.Q, tile.R + direction.R);
                    neighborCounts.TryGetValue(neighbor, out int count);
                    neighborCounts[neighbor] = count + 1;
                }
            }

            return neighborCounts
                .Where(entry => entry.Value == 2 || (entry.Value == 1 && blackTiles.Contains(entry.Key)))
                .Select(entry => entry.Key)
                .ToHashSet();
        }

        private (int Q, int R) FollowPath(string path)
        {
            // Axial coordinates give each tile six integer-coordinate neighbors.
            int q = 0;
            int r = 0;
            for (int index = 0; index < path.Length;)
            {
                int length = path[index] == 'n' || path[index] == 's' ? 2 : 1;
                if (index + length > path.Length)
                    throw new FormatException("A north or south direction must be followed by east or west.");

                string direction = path.Substring(index, length);
                if (!Directions.TryGetValue(direction, out var offset))
                    throw new FormatException($"Invalid hex direction '{direction}'.");

                q += offset.Q;
                r += offset.R;
                index += length;
            }

            return (q, r);
        }
    }
}
