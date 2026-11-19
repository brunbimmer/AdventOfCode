using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 5)]
    public class Year2017Day05 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day05()
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
            int[] offsets = FileIOHelper.getInstance().ReadDataAsLines(file)
                .Where(line => !string.IsNullOrWhiteSpace(line)).Select(int.Parse).ToArray();
            _SW.Restart();
            long first = CountSteps(offsets, false);
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Steps to escape the jump list");
            _SW.Restart();
            long second = CountSteps(offsets, true);
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Steps with decreasing large offsets");
        }

        private long CountSteps(int[] original, bool decreaseLargeOffsets)
        {
            int[] offsets = (int[])original.Clone();
            long position = 0;
            long steps = 0;
            while (position >= 0 && position < offsets.Length)
            {
                int index = (int)position;
                int jump = offsets[index];
                offsets[index] = checked(jump + (decreaseLargeOffsets && jump >= 3 ? -1 : 1));
                position += jump;
                steps++;
            }
            return steps;
        }
    }
}
