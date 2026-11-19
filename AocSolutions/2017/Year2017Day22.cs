using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 22)]
    public class Year2017Day22 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day22()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] grid = FileIOHelper.getInstance().ReadDataAsLines(file);
            _SW.Restart();
            int first = CountInfections(grid, 10_000, false);
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Infection bursts in ten thousand moves");
            _SW.Restart();
            int second = CountInfections(grid, 10_000_000, true);
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Infection bursts with evolved node states");
        }

        private int CountInfections(string[] grid, int bursts, bool evolved)
        {
            if (grid.Length == 0 || grid.Length % 2 == 0 || grid[0].Length % 2 == 0 || grid.Any(row => row.Length != grid[0].Length))
                throw new FormatException("The infection map must have odd, rectangular dimensions.");
            var states = new Dictionary<(int X, int Y), byte>();
            for (int row = 0; row < grid.Length; row++)
            {
                for (int col = 0; col < grid[row].Length; col++)
                {
                    if (grid[row][col] == '#') states[(col - grid[0].Length / 2, row - grid.Length / 2)] = 2;
                    else if (grid[row][col] != '.') throw new FormatException("Unknown initial node state.");
                }
            }
            var dx = new[] { 0, 1, 0, -1 };
            var dy = new[] { -1, 0, 1, 0 };
            int x = 0;
            int y = 0;
            int direction = 0;
            int infections = 0;
            for (int burst = 0; burst < bursts; burst++)
            {
                var position = (x, y);
                byte state = states.GetValueOrDefault(position);
                if (evolved)
                {
                    direction = (direction + (state switch { 0 => 3, 1 => 0, 2 => 1, 3 => 2, _ => 0 })) % 4;
                    state = (byte)((state + 1) % 4);
                }
                else
                {
                    direction = (direction + (state == 0 ? 3 : 1)) % 4;
                    state = state == 0 ? (byte)2 : (byte)0;
                }
                if (state == 2) infections++;
                if (state == 0) states.Remove(position);
                else states[position] = state;
                x += dx[direction];
                y += dy[direction];
            }
            return infections;
        }
    }
}
