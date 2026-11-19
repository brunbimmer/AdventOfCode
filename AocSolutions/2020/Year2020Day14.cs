using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 14)]
    public class Year2020Day14 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day14()
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
            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            long sum = Part1(instructions);
            _SW.Stop();

            output.WritePart(1, sum, trackTime ? _SW.Elapsed : null, "Sum of values left in memory");


            _SW.Restart();
            BigInteger addressSum = Part2(instructions);
            _SW.Stop();

            output.WritePart(2, addressSum, trackTime ? _SW.Elapsed : null, "Sum of values left in memory");

        }

        private long Part1(string[] instructions)
        {
            Dictionary<long, long> memory = new Dictionary<long, long>();
            long andMask = (1L << 36) - 1;
            long orMask = 0;

            foreach (string instruction in instructions)
            {
                string[] parts = instruction.Split(" = ", 2);
                if (parts[0] == "mask")
                {
                    andMask = Convert.ToInt64(parts[1].Replace('X', '1'), 2);
                    orMask = Convert.ToInt64(parts[1].Replace('X', '0'), 2);
                }
                else
                {
                    long address = long.Parse(parts[0].AsSpan(4, parts[0].Length - 5));
                    long value = long.Parse(parts[1]);
                    memory[address] = (value & andMask) | orMask;
                }
            }

            return memory.Values.Sum();
        }

        private BigInteger Part2(string[] instructions)
        {
            const long allBits = (1L << 36) - 1;
            var writes = new List<((long Mask, long Bits) Region, long Value)>();
            long fixedMask = allBits;
            long orMask = 0;

            foreach (string instruction in instructions)
            {
                string[] parts = instruction.Split(" = ", 2);
                if (parts[0] == "mask")
                {
                    orMask = Convert.ToInt64(parts[1].Replace('X', '0'), 2);
                    long floatingMask = Convert.ToInt64(parts[1].Replace('1', '0').Replace('X', '1'), 2);
                    fixedMask = allBits & ~floatingMask;
                }
                else
                {
                    long address = long.Parse(parts[0].AsSpan(4, parts[0].Length - 5));
                    long value = long.Parse(parts[1]);
                    writes.Add(((fixedMask, (address | orMask) & fixedMask), value));
                }
            }

            // Signed intersections describe the union of addresses already overwritten.
            var covered = new Dictionary<(long Mask, long Bits), BigInteger>();
            BigInteger total = 0;
            for (int index = writes.Count - 1; index >= 0; index--)
            {
                var write = writes[index];
                long addressCount = CountAddresses(write.Region);
                BigInteger overwritten = 0;
                var changes = new Dictionary<(long Mask, long Bits), BigInteger>();

                foreach (var region in covered)
                {
                    var intersection = IntersectRegions(write.Region, region.Key);
                    if (intersection.HasValue)
                    {
                        overwritten += region.Value * CountAddresses(intersection.Value);
                        changes[intersection.Value] = changes.GetValueOrDefault(intersection.Value) - region.Value;
                    }
                }

                total += (addressCount - overwritten) * write.Value;
                changes[write.Region] = changes.GetValueOrDefault(write.Region) + 1;
                foreach (var change in changes)
                {
                    BigInteger coefficient = covered.GetValueOrDefault(change.Key) + change.Value;
                    if (coefficient == 0)
                        covered.Remove(change.Key);
                    else
                        covered[change.Key] = coefficient;
                }
            }

            return total;
        }

        private (long Mask, long Bits)? IntersectRegions((long Mask, long Bits) left, (long Mask, long Bits) right)
        {
            if (((left.Bits ^ right.Bits) & left.Mask & right.Mask) != 0)
                return null;

            return (left.Mask | right.Mask, left.Bits | right.Bits);
        }

        private long CountAddresses((long Mask, long Bits) region)
        {
            return 1L << (36 - BitOperations.PopCount((ulong)region.Mask));
        }
    }
}
