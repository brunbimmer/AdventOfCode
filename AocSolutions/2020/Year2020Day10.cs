using System;
using System.Collections.Generic;
using System.Diagnostics;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 10)]
    public class Year2020Day10 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day10()
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
            int[] adapters = FileIOHelper.getInstance().ReadDataToIntArray(file);

            _SW.Restart();
            int product = Part1(adapters);
            _SW.Stop();

            output.WritePart(1, product, trackTime ? _SW.Elapsed : null, "Product of 1-jolt and 3-jolt differences");


            _SW.Restart();
            long arrangements = Part2(adapters);
            _SW.Stop();

            output.WritePart(2, arrangements, trackTime ? _SW.Elapsed : null, "Number of distinct adapter arrangements");

        }

        private int Part1(int[] adapters)
        {
            int[] sortedAdapters = (int[])adapters.Clone();
            Array.Sort(sortedAdapters);

            int previous = 0;
            int oneJoltDifferences = 0;
            // The device is always three jolts above the highest adapter.
            int threeJoltDifferences = 1;

            foreach (int adapter in sortedAdapters)
            {
                int difference = adapter - previous;
                if (difference < 1 || difference > 3)
                    throw new InvalidOperationException("The adapters cannot form a chain using every adapter.");

                if (difference == 1)
                    oneJoltDifferences++;
                else if (difference == 3)
                    threeJoltDifferences++;

                previous = adapter;
            }

            return oneJoltDifferences * threeJoltDifferences;
        }

        private long Part2(int[] adapters)
        {
            int[] sortedAdapters = (int[])adapters.Clone();
            Array.Sort(sortedAdapters);

            Dictionary<int, long> ways = new Dictionary<int, long> { [0] = 1 };
            long arrangements = 1;

            foreach (int adapter in sortedAdapters)
            {
                arrangements = 0;
                for (int difference = 1; difference <= 3; difference++)
                {
                    if (ways.TryGetValue(adapter - difference, out long previousWays))
                        arrangements = checked(arrangements + previousWays);
                }

                ways.Add(adapter, arrangements);
            }

            // Only the highest adapter can reach the device, which is three jolts higher.
            return arrangements;
        }
    }
}
