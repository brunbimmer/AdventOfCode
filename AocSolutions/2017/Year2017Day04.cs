using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 4)]
    public class Year2017Day04 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day04()
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
            int first = CountValid(lines, false);
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Passphrases without duplicate words");
            _SW.Restart();
            int second = CountValid(lines, true);
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Passphrases without anagrams");
        }

        private int CountValid(string[] lines, bool rejectAnagrams)
        {
            int count = 0;
            foreach (string line in lines.Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                string[] words = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<string> keys = rejectAnagrams
                    ? words.Select(word => new string(word.OrderBy(letter => letter).ToArray()))
                    : words;
                if (keys.Distinct(StringComparer.Ordinal).Count() == words.Length)
                    count++;
            }
            return count;
        }
    }
}
