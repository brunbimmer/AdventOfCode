using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 9)]
    public class Year2017Day09 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day09()
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
            _SW.Restart();
            var result = Analyze(FileIOHelper.getInstance().ReadDataAsString(file));
            _SW.Stop();
            output.WritePart(1, result.Score, trackTime ? _SW.Elapsed : null, "Total group score");
            output.WritePart(2, result.Garbage, description: "Non-canceled characters inside garbage");
        }

        private (long Score, long Garbage) Analyze(string stream)
        {
            int depth = 0;
            long score = 0;
            long garbageCount = 0;
            bool garbage = false;
            bool canceled = false;
            foreach (char character in stream)
            {
                if (canceled)
                {
                    canceled = false;
                    continue;
                }
                if (garbage)
                {
                    if (character == '!') canceled = true;
                    else if (character == '>') garbage = false;
                    else garbageCount++;
                    continue;
                }
                switch (character)
                {
                    case '{': score += ++depth; break;
                    case '}':
                        if (--depth < 0) throw new FormatException("A group closes before it opens.");
                        break;
                    case '<': garbage = true; break;
                    case ',': break;
                    default:
                        if (!char.IsWhiteSpace(character)) throw new FormatException("Unexpected character outside garbage.");
                        break;
                }
            }
            if (depth != 0 || garbage || canceled)
                throw new FormatException("The stream contains an unfinished group or garbage section.");
            return (score, garbageCount);
        }
    }
}
