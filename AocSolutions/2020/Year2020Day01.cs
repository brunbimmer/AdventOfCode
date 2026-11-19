using System;
using System.Collections.Generic;
using System.Diagnostics;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 1)]
    public class Year2020Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day01()
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

            int[] entries = FileIOHelper.getInstance().ReadDataToIntArray(file);
            
            _SW.Restart();
            int product = Part1(entries);
            _SW.Stop();

            output.WritePart(1, product, trackTime ? _SW.Elapsed : null, "Product of the two entries that sum to 2020");


            _SW.Restart();
            int tripleProduct = Part2(entries);
            _SW.Stop();

            output.WritePart(2, tripleProduct, trackTime ? _SW.Elapsed : null, "Product of the three entries that sum to 2020");

        }

        private int Part1(int[] entries)
        {
            HashSet<int> seen = new HashSet<int>();

            foreach (int entry in entries)
            {
                int complement = 2020 - entry;
                if (seen.Contains(complement))
                    return entry * complement;

                seen.Add(entry);
            }

            throw new InvalidOperationException("No two expense report entries sum to 2020.");
        }

        private int Part2(int[] entries)
        {
            for (int i = 0; i < entries.Length - 2; i++)
            {
                HashSet<int> seen = new HashSet<int>();

                for (int j = i + 1; j < entries.Length; j++)
                {
                    int complement = 2020 - entries[i] - entries[j];
                    if (seen.Contains(complement))
                        return entries[i] * entries[j] * complement;

                    seen.Add(entries[j]);
                }
            }

            throw new InvalidOperationException("No three expense report entries sum to 2020.");
        }
    }
}
