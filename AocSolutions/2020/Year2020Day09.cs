using System;
using System.Collections.Generic;
using System.Diagnostics;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 9)]
    public class Year2020Day09 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day09()
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
            long[] numbers = FileIOHelper.getInstance().ReadDataToLongArray(file);

            _SW.Restart();
            long invalidNumber = Part1(numbers);
            _SW.Stop();

            output.WritePart(1, invalidNumber, trackTime ? _SW.Elapsed : null, "First invalid number");


            _SW.Restart();
            long weakness = Part2(numbers, invalidNumber);
            _SW.Stop();

            output.WritePart(2, weakness, trackTime ? _SW.Elapsed : null, "Encryption weakness");

        }

        private long Part1(long[] numbers, int preambleLength = 25)
        {
            if (preambleLength < 2)
                throw new ArgumentOutOfRangeException(nameof(preambleLength), "The preamble must contain at least two numbers.");

            HashSet<long> seen = new HashSet<long>();
            for (int index = preambleLength; index < numbers.Length; index++)
            {
                seen.Clear();
                bool valid = false;

                for (int previous = index - preambleLength; previous < index; previous++)
                {
                    long number = numbers[previous];
                    long complement = numbers[index] - number;
                    if (complement != number && seen.Contains(complement))
                    {
                        valid = true;
                        break;
                    }

                    seen.Add(number);
                }

                if (!valid)
                    return numbers[index];
            }

            throw new InvalidOperationException("No invalid number was found after the preamble.");
        }

        private long Part2(long[] numbers, long invalidNumber)
        {
            Dictionary<long, int> prefixPositions = new Dictionary<long, int> { [0] = 0 };
            long sum = 0;

            for (int end = 1; end <= numbers.Length; end++)
            {
                sum += numbers[end - 1];

                // Subtracting two prefix sums gives the sum of the intervening range.
                if (prefixPositions.TryGetValue(sum - invalidNumber, out int start) && end - start >= 2)
                {
                    long minimum = numbers[start];
                    long maximum = numbers[start];
                    for (int index = start + 1; index < end; index++)
                    {
                        minimum = Math.Min(minimum, numbers[index]);
                        maximum = Math.Max(maximum, numbers[index]);
                    }

                    return minimum + maximum;
                }

                // Keep the earliest position so repeated sums allow the longest range.
                prefixPositions.TryAdd(sum, end);
            }

            throw new InvalidOperationException("No contiguous range of at least two numbers sums to the invalid number.");
        }
    }
}
