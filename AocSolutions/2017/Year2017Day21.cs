using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 21)]
    public class Year2017Day21 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day21()
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
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
            _SW.Restart();
            int first = CountPixels(lines, 5);
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Pixels on after five enhancements");
            _SW.Restart();
            int second = CountPixels(lines, 18);
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Pixels on after eighteen enhancements");
        }

        private int CountPixels(string[] lines, int iterations)
        {
            var rules = new Dictionary<(int Size, int Bits), int>();
            foreach (string line in lines.Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                string[] parts = line.Split("=>", StringSplitOptions.TrimEntries);
                if (parts.Length != 2) throw new FormatException("Invalid enhancement rule.");
                string[] pattern = parts[0].Split('/');
                string[] output = parts[1].Split('/');
                if ((pattern.Length != 2 && pattern.Length != 3) || output.Length != pattern.Length + 1)
                    throw new FormatException("Enhancement rules must map 2x2 to 3x3 or 3x3 to 4x4.");
                Encode(pattern);
                int outputBits = Encode(output);
                for (int rotation = 0; rotation < 4; rotation++)
                {
                    string[] flipped = pattern.Select(row => new string(row.Reverse().ToArray())).ToArray();
                    foreach (string[] variant in new[] { pattern, flipped })
                    {
                        var key = (variant.Length, Encode(variant));
                        if (rules.TryGetValue(key, out int previous) && previous != outputBits)
                            throw new FormatException("Equivalent input patterns have conflicting enhancements.");
                        rules[key] = outputBits;
                    }
                    pattern = Rotate(pattern);
                }
            }

            int initial = Encode(new[] { ".#.", "..#", "###" });
            var grid = new bool[3, 3];
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    grid[row, col] = (initial & (1 << (row * 3 + col))) != 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                int size = grid.GetLength(0);
                int blockSize = size % 2 == 0 ? 2 : 3;
                int outputSize = blockSize + 1;
                int blocks = size / blockSize;
                var expanded = new bool[blocks * outputSize, blocks * outputSize];
                for (int blockRow = 0; blockRow < blocks; blockRow++)
                {
                    for (int blockCol = 0; blockCol < blocks; blockCol++)
                    {
                        int bits = 0;
                        for (int row = 0; row < blockSize; row++)
                            for (int col = 0; col < blockSize; col++)
                                if (grid[blockRow * blockSize + row, blockCol * blockSize + col])
                                    bits |= 1 << (row * blockSize + col);
                        if (!rules.TryGetValue((blockSize, bits), out int replacement))
                            throw new InvalidOperationException($"No enhancement rule matches a block at iteration {iteration + 1}.");
                        for (int row = 0; row < outputSize; row++)
                            for (int col = 0; col < outputSize; col++)
                                expanded[blockRow * outputSize + row, blockCol * outputSize + col] =
                                    (replacement & (1 << (row * outputSize + col))) != 0;
                    }
                }
                grid = expanded;
            }
            int count = 0;
            foreach (bool pixel in grid)
                if (pixel) count++;
            return count;
        }

        private int Encode(string[] pattern)
        {
            int bits = 0;
            for (int row = 0; row < pattern.Length; row++)
            {
                if (pattern[row].Length != pattern.Length) throw new FormatException("Pixel patterns must be square.");
                for (int col = 0; col < pattern.Length; col++)
                {
                    if (pattern[row][col] == '#') bits |= 1 << (row * pattern.Length + col);
                    else if (pattern[row][col] != '.') throw new FormatException("Unknown pixel value.");
                }
            }
            return bits;
        }

        private string[] Rotate(string[] pattern)
        {
            int size = pattern.Length;
            var rotated = new string[size];
            for (int row = 0; row < size; row++)
            {
                var pixels = new char[size];
                for (int col = 0; col < size; col++) pixels[col] = pattern[size - col - 1][row];
                rotated[row] = new string(pixels);
            }
            return rotated;
        }
    }
}
