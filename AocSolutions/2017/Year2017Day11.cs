using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 11)]
    public class Year2017Day11 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day11()
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
            _SW.Restart();
            var result = Walk(FileIOHelper.getInstance().ReadDataAsString(file));
            _SW.Stop();
            output.WritePart(1, result.Final, trackTime ? _SW.Elapsed : null, "Hex steps from the starting point");
            output.WritePart(2, result.Furthest, description: "Furthest distance reached along the path");
        }

        private (int Final, int Furthest) Walk(string input)
        {
            int q = 0;
            int r = 0;
            int furthest = 0;
            foreach (string direction in input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                (int dq, int dr) = direction switch
                {
                    "n" => (0, -1), "ne" => (1, -1), "se" => (1, 0),
                    "s" => (0, 1), "sw" => (-1, 1), "nw" => (-1, 0),
                    _ => throw new FormatException($"Unknown hex direction '{direction}'.")
                };
                q += dq;
                r += dr;
                furthest = Math.Max(furthest, Math.Max(Math.Abs(q), Math.Max(Math.Abs(r), Math.Abs(q + r))));
            }
            return (Math.Max(Math.Abs(q), Math.Max(Math.Abs(r), Math.Abs(q + r))), furthest);
        }
    }
}
