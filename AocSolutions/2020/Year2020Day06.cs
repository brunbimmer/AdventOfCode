using System;
using System.Collections.Generic;
using System.Diagnostics;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 6)]
    public class Year2020Day06 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day06()
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

            _SW.Restart();
            int total = Part1(lines);
            _SW.Stop();

            output.WritePart(1, total, trackTime ? _SW.Elapsed : null, "Sum of questions anyone answered yes to");


            _SW.Restart();
            total = Part2(lines);
            _SW.Stop();

            output.WritePart(2, total, trackTime ? _SW.Elapsed : null, "Sum of questions everyone answered yes to");

        }

        private int Part1(string[] lines)
        {
            HashSet<char> answers = new HashSet<char>();
            int total = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    total += answers.Count;
                    answers.Clear();
                }
                else
                {
                    answers.UnionWith(line);
                }
            }

            // Include the final group even when the file has no trailing blank line.
            return total + answers.Count;
        }

        private int Part2(string[] lines)
        {
            HashSet<char> answers = null;
            int total = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    total += answers?.Count ?? 0;
                    answers = null;
                }
                else if (answers == null)
                {
                    answers = new HashSet<char>(line);
                }
                else
                {
                    answers.IntersectWith(line);
                }
            }

            return total + (answers?.Count ?? 0);
        }
    }
}
