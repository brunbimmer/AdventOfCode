using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 17)]
    public class Year2020Day17 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day17()
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
            string[] layout = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            int activeCubes = Part1(layout);
            _SW.Stop();

            output.WritePart(1, activeCubes, trackTime ? _SW.Elapsed : null, "Active cubes after six cycles");


            _SW.Restart();
            activeCubes = Part2(layout);
            _SW.Stop();

            output.WritePart(2, activeCubes, trackTime ? _SW.Elapsed : null, "Active hypercubes after six cycles");

        }

        private int Part1(string[] layout, int cycles = 6)
        {
            return Simulate(layout, cycles, useFourDimensions: false);
        }

        private int Part2(string[] layout, int cycles = 6)
        {
            return Simulate(layout, cycles, useFourDimensions: true);
        }

        private int Simulate(string[] layout, int cycles, bool useFourDimensions)
        {
            if (cycles < 0)
                throw new ArgumentOutOfRangeException(nameof(cycles));

            var active = new HashSet<(int X, int Y, int Z, int W)>();
            for (int row = 0; row < layout.Length; row++)
            {
                for (int column = 0; column < layout[row].Length; column++)
                {
                    if (layout[row][column] == '#')
                        active.Add((column, row, 0, 0));
                }
            }

            int wRadius = useFourDimensions ? 1 : 0;
            var offsets = (
                from x in Enumerable.Range(-1, 3)
                from y in Enumerable.Range(-1, 3)
                from z in Enumerable.Range(-1, 3)
                from w in Enumerable.Range(-wRadius, 2 * wRadius + 1)
                where x != 0 || y != 0 || z != 0 || w != 0
                select (X: x, Y: y, Z: z, W: w)).ToArray();

            var neighborCounts = new Dictionary<(int X, int Y, int Z, int W), int>();
            for (int cycle = 0; cycle < cycles; cycle++)
            {
                neighborCounts.Clear();
                foreach (var cube in active)
                {
                    foreach (var offset in offsets)
                    {
                        var neighbor = (cube.X + offset.X, cube.Y + offset.Y, cube.Z + offset.Z, cube.W + offset.W);
                        neighborCounts[neighbor] = neighborCounts.GetValueOrDefault(neighbor) + 1;
                    }
                }

                // Cubes with no active neighbors are omitted and become inactive.
                active = neighborCounts
                    .Where(pair => pair.Value == 3 || (pair.Value == 2 && active.Contains(pair.Key)))
                    .Select(pair => pair.Key).ToHashSet();
            }

            return active.Count;
        }
    }
}
