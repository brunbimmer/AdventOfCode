using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using CommonAlgorithms;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 10)]
    public class Year2017Day10 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day10()
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
            string input = FileIOHelper.getInstance().ReadDataAsString(file).TrimEnd('\r', '\n');
            _SW.Restart();
            int first = SparseProduct(input, 256);
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Product of the first two sparse-hash values");
            _SW.Restart();
            string second = Convert.ToHexString(KnotHash.Compute(input)).ToLowerInvariant();
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Knot hash");
        }

        private int SparseProduct(string input, int size)
        {
            int[] lengths = input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToArray();
            int[] values = KnotHash.Twist(lengths, size, 1);
            if (values.Length < 2) throw new ArgumentOutOfRangeException(nameof(size));
            return values[0] * values[1];
        }
    }
}
