using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 5)]
    public class Year2015Day5Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2015Day5Problem()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            int numNiceStrings = Part1(lines);

            _SW.Stop();

            output.WritePart(1, numNiceStrings, trackTime ? _SW.Elapsed : null, "The number of nice strings in the sequence input are");


            _SW.Restart();

            numNiceStrings = Part2(lines);

            output.WritePart(2, numNiceStrings, trackTime ? _SW.Elapsed : null, "The number of nice strings in the sequence input are");



        }

        public int Part1(string[] list)
        {
            int numNiceStrings = 0;

            var vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u' };

            foreach (string line in list)
            {
                //Step 1: Check for disallowed variables first. If found, skip to next string
                if (line.Contains("ab") || line.Contains("cd") || line.Contains("pq") || line.Contains("xy"))
                    continue;

                //Step 2: check vowel count
                int total = line.Count(c => vowels.Contains(c));

                if (total < 3)
                    continue;

                //Step 3: check for doubles
                var matches = Regex.Matches(line, @"(.)\1+");
                if (matches.Count > 0)
                    numNiceStrings += 1;
            }

            return numNiceStrings;
            
        }

        public int Part2(string[] list)
        {
            int numNiceStrings = 0;

            foreach (string line in list)
            {
                //find the first overlapping sequential ppair
                var matches = Regex.Matches(line, @"(\w{2}).*?(\1)");

                if (matches.Count == 0)
                    continue;

                //find a letter that repears with exactly one letter in between
                //if we find out, increase count and continue to next loop
                for (int i = 0; i < line.Length - 2; i++)
                {
                    if (line[i] == line[i + 2])
                    {
                        numNiceStrings++;
                        break;
                    }
                }

            }

            return numNiceStrings;
        }
    }
}
