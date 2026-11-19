using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 15)]
    public class Year2017Day15 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day15()
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
            long[] seeds = FileIOHelper.getInstance().ReadDataAsLines(file).Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => long.Parse(line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Last())).ToArray();
            if (seeds.Length != 2 || seeds.Any(seed => seed <= 0 || seed >= 2147483647))
                throw new FormatException("Two generator seeds between 1 and 2147483646 are required.");
            _SW.Restart();
            int first = Judge(seeds[0], seeds[1], 40_000_000, 1, 1);
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Matches in forty million generator pairs");
            _SW.Restart();
            int second = Judge(seeds[0], seeds[1], 5_000_000, 4, 8);
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Matches in five million filtered pairs");
        }

        private int Judge(long a, long b, int pairs, int multipleA, int multipleB)
        {
            int matches = 0;
            for (int pair = 0; pair < pairs; pair++)
            {
                do { a = a * 16807 % 2147483647; } while (a % multipleA != 0);
                do { b = b * 48271 % 2147483647; } while (b % multipleB != 0);
                if ((a & 65535) == (b & 65535)) matches++;
            }
            return matches;
        }
    }
}
