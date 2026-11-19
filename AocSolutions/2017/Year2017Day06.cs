using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 6)]
    public class Year2017Day06 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day06()
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
            int[] banks = FileIOHelper.getInstance().ReadDataAsString(file)
                .Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            if (banks.Length == 0 || banks.Any(value => value < 0))
                throw new FormatException("Memory banks must contain non-negative block counts.");
            _SW.Restart();
            var result = FindRepeat(banks);
            _SW.Stop();
            output.WritePart(1, result.Cycles, trackTime ? _SW.Elapsed : null, "Redistributions before a repeated configuration");
            output.WritePart(2, result.LoopLength, description: "Redistributions in the repeating loop");
        }

        private (int Cycles, int LoopLength) FindRepeat(int[] original)
        {
            int[] banks = (int[])original.Clone();
            var seen = new Dictionary<string, int>(StringComparer.Ordinal);
            int cycles = 0;
            while (true)
            {
                string state = string.Join(",", banks);
                if (seen.TryGetValue(state, out int firstSeen))
                    return (cycles, cycles - firstSeen);
                seen.Add(state, cycles);
                int largest = Array.IndexOf(banks, banks.Max());
                int blocks = banks[largest];
                banks[largest] = 0;
                int each = blocks / banks.Length;
                int remainder = blocks % banks.Length;
                for (int index = 0; index < banks.Length; index++)
                    banks[index] = checked(banks[index] + each);
                for (int offset = 1; offset <= remainder; offset++)
                    banks[(largest + offset) % banks.Length]++;
                cycles++;
            }
        }
    }
}
