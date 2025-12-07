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
    [AdventOfCode(Year = 2022, Day = 23)]
    public class Year2022Day23 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day23()
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

            _SW.Start();
            // Parse elves from input
            var elves = ParseElves(lines);          
            long part1 = SolvePart1(elves);

            _SW.Stop();

            Console.WriteLine($"  Part 1: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            // Re-parse for Part 2
            elves = ParseElves(lines);
            long part2 = SolvePart2(elves);

            _SW.Stop();

            Console.WriteLine($"  Part 2: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private HashSet<Coordinate2D> ParseElves(string[] lines)
        {
            var elves = new HashSet<Coordinate2D>();
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        elves.Add(new Coordinate2D(x, y));
                    }
                }
            }
            return elves;
        }

        private long SolvePart1(HashSet<Coordinate2D> elves)
        {
            var directions = new List<(Coordinate2D[], Coordinate2D)>
            {
                // North: NW, N, NE -> move N
                (new[] { new Coordinate2D(-1, -1), new Coordinate2D(0, -1), new Coordinate2D(1, -1) }, new Coordinate2D(0, -1)),
                // South: SW, S, SE -> move S
                (new[] { new Coordinate2D(-1, 1), new Coordinate2D(0, 1), new Coordinate2D(1, 1) }, new Coordinate2D(0, 1)),
                // West: NW, W, SW -> move W
                (new[] { new Coordinate2D(-1, -1), new Coordinate2D(-1, 0), new Coordinate2D(-1, 1) }, new Coordinate2D(-1, 0)),
                // East: NE, E, SE -> move E
                (new[] { new Coordinate2D(1, -1), new Coordinate2D(1, 0), new Coordinate2D(1, 1) }, new Coordinate2D(1, 0))
            };

            for (int round = 0; round < 10; round++)
            {
                SimulateRound(elves, directions);
                directions.Add(directions[0]);
                directions.RemoveAt(0);
            }

            return CountEmptyTiles(elves);
        }

        private long SolvePart2(HashSet<Coordinate2D> elves)
        {
            var directions = new List<(Coordinate2D[], Coordinate2D)>
            {
                (new[] { new Coordinate2D(-1, -1), new Coordinate2D(0, -1), new Coordinate2D(1, -1) }, new Coordinate2D(0, -1)),
                (new[] { new Coordinate2D(-1, 1), new Coordinate2D(0, 1), new Coordinate2D(1, 1) }, new Coordinate2D(0, 1)),
                (new[] { new Coordinate2D(-1, -1), new Coordinate2D(-1, 0), new Coordinate2D(-1, 1) }, new Coordinate2D(-1, 0)),
                (new[] { new Coordinate2D(1, -1), new Coordinate2D(1, 0), new Coordinate2D(1, 1) }, new Coordinate2D(1, 0))
            };

            int round = 0;
            while (true)
            {
                bool anyMoved = SimulateRound(elves, directions);
                round++;
                
                if (!anyMoved)
                    return round;

                directions.Add(directions[0]);
                directions.RemoveAt(0);
            }
        }

        private bool SimulateRound(HashSet<Coordinate2D> elves, List<(Coordinate2D[], Coordinate2D)> directions)
        {
            // Phase 1: Propose moves
            var proposals = new Dictionary<Coordinate2D, Coordinate2D>();
            var proposalCounts = new Dictionary<Coordinate2D, int>();

            foreach (var elf in elves)
            {
                // Check if elf has any neighbors
                if (!HasNeighbors(elf, elves))
                    continue;

                // Check directions in order
                foreach (var (checkPositions, moveDir) in directions)
                {
                    if (checkPositions.All(offset => !elves.Contains(elf + offset)))
                    {
                        var proposedPos = elf + moveDir;
                        proposals[elf] = proposedPos;
                        proposalCounts[proposedPos] = proposalCounts.GetValueOrDefault(proposedPos) + 1;
                        break;
                    }
                }
            }

            // Phase 2: Execute valid moves
            bool anyMoved = false;
            var newElves = new HashSet<Coordinate2D>(elves);

            foreach (var (elf, proposedPos) in proposals)
            {
                if (proposalCounts[proposedPos] == 1)
                {
                    newElves.Remove(elf);
                    newElves.Add(proposedPos);
                    anyMoved = true;
                }
            }

            elves.Clear();
            foreach (var pos in newElves)
                elves.Add(pos);

            return anyMoved;
        }

        private bool HasNeighbors(Coordinate2D elf, HashSet<Coordinate2D> elves)
        {
            // Check all 8 adjacent positions
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    if (elves.Contains(elf + new Coordinate2D(dx, dy)))
                        return true;
                }
            }
            return false;
        }

        private long CountEmptyTiles(HashSet<Coordinate2D> elves)
        {
            int minX = elves.Min(e => e.X);
            int maxX = elves.Max(e => e.X);
            int minY = elves.Min(e => e.Y);
            int maxY = elves.Max(e => e.Y);

            long totalTiles = (long)(maxX - minX + 1) * (maxY - minY + 1);
            return totalTiles - elves.Count;
        }
    }
}
