using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 25)]
    public class Year2017Day25 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day25()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        private readonly record struct Transition(bool WriteOne, int Move, string Next);

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            _SW.Restart();
            int checksum = Checksum(FileIOHelper.getInstance().ReadDataAsLines(file));
            _SW.Stop();
            output.WritePart(1, checksum, trackTime ? _SW.Elapsed : null, "Turing-machine diagnostic checksum");
        }

        private int Checksum(string[] source)
        {
            string[] lines = source.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => line.Trim()).ToArray();
            if (lines.Length < 2 || (lines.Length - 2) % 9 != 0)
                throw new FormatException("The Turing blueprint contains incomplete state rules.");
            string Capture(string text, string pattern)
            {
                Match match = Regex.Match(text, pattern);
                return match.Success ? match.Groups[1].Value : throw new FormatException($"Invalid blueprint line '{text}'.");
            }
            string state = Capture(lines[0], @"^Begin in state (\w+)\.$");
            long steps = long.Parse(Capture(lines[1], @"^Perform a diagnostic checksum after (\d+) steps\.$"));
            var transitions = new Dictionary<(string State, bool One), Transition>();
            for (int start = 2; start < lines.Length; start += 9)
            {
                string name = Capture(lines[start], @"^In state (\w+):$");
                for (int branch = 0; branch < 2; branch++)
                {
                    int offset = start + 1 + branch * 4;
                    bool one = Capture(lines[offset], @"^If the current value is ([01]):$") == "1";
                    bool write = Capture(lines[offset + 1], @"^- Write the value ([01])\.$") == "1";
                    int move = Capture(lines[offset + 2], @"^- Move one slot to the (left|right)\.$") == "right" ? 1 : -1;
                    string next = Capture(lines[offset + 3], @"^- Continue with state (\w+)\.$");
                    if (!transitions.TryAdd((name, one), new Transition(write, move, next)))
                        throw new FormatException("The blueprint defines a state/value transition more than once.");
                }
            }
            if (!transitions.ContainsKey((state, false)) || transitions.Values.Any(transition =>
                !transitions.ContainsKey((transition.Next, false)) || !transitions.ContainsKey((transition.Next, true))))
                throw new FormatException("The blueprint references an undefined state.");
            var ones = new HashSet<long>();
            long cursor = 0;
            for (long step = 0; step < steps; step++)
            {
                Transition transition = transitions[(state, ones.Contains(cursor))];
                if (transition.WriteOne) ones.Add(cursor);
                else ones.Remove(cursor);
                cursor += transition.Move;
                state = transition.Next;
            }
            return ones.Count;
        }
    }
}
