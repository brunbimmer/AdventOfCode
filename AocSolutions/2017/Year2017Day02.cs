using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 02)]
    public class Year2017Day02 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day02()
        {
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
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();
            long part1 = SolvePart1(lines);
            _SW.Stop();
            output.WritePart(1, part1, trackTime ? _SW.Elapsed : null, "Spreadsheet Checksum");

            _SW.Restart();
            long part2 = SolvePart2(lines);
            _SW.Stop();
            output.WritePart(2, part2, trackTime ? _SW.Elapsed : null, "Sum of the Evenly Divisible Values");
        }

        // Part 1: checksum = sum over rows of (max - min)
        private long SolvePart1(string[] lines)
        {
            long checksum = 0;
            foreach (var row in ParseSpreadsheet(lines))
            {
                checksum += row.Max() - row.Min();
            }
            return checksum;
        }

        // Part 2: per row find the only divisible pair (a % b == 0), add a/b
        private long SolvePart2(string[] lines)
        {
            long sum = 0;
            foreach (var row in ParseSpreadsheet(lines))
            {
                sum += FindEvenDivisionResult(row);
            }
            return sum;
        }

        private IEnumerable<int[]> ParseSpreadsheet(string[] lines)
        {
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // AoC input is tab-separated, but accept spaces too.
                var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    continue;

                yield return parts.Select(int.Parse).ToArray();
            }
        }

        private int FindEvenDivisionResult(int[] row)
        {
            // The puzzle guarantees exactly one such pair per row.
            for (int i = 0; i < row.Length; i++)
            {
                for (int j = 0; j < row.Length; j++)
                {
                    if (i == j)
                        continue;

                    int a = row[i];
                    int b = row[j];
                    if (a % b == 0)
                        return a / b;
                }
            }

            throw new InvalidOperationException("No evenly divisible pair found in row: " + string.Join(" ", row));
        }
    }
}
