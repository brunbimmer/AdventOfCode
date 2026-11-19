using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 16)]
    public class Year2017Day16 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day16()
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
            string input = FileIOHelper.getInstance().ReadDataAsString(file).Trim();
            _SW.Restart();
            string first = Dance(input, 16, 1);
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Program order after one dance");
            _SW.Restart();
            string second = Dance(input, 16, 1_000_000_000);
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Program order after one billion dances");
        }

        private string Dance(string input, int count, int repetitions)
        {
            char[] programs = Enumerable.Range(0, count).Select(index => (char)('a' + index)).ToArray();
            string[] moves = input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var seen = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int round = 0; round < repetitions; round++)
            {
                string state = new string(programs);
                if (seen.TryGetValue(state, out int previous))
                {
                    int cycle = round - previous;
                    int remaining = (repetitions - round) % cycle;
                    repetitions = round + remaining;
                    seen.Clear();
                    if (remaining == 0) break;
                }
                seen[state] = round;
                foreach (string move in moves)
                {
                    switch (move[0])
                    {
                        case 's':
                            int spin = int.Parse(move[1..]) % count;
                            if (spin < 0) throw new FormatException("Spin sizes must be non-negative.");
                            var rotated = new char[count];
                            for (int index = 0; index < count; index++)
                                rotated[(index + spin) % count] = programs[index];
                            programs = rotated;
                            break;
                        case 'x':
                            string[] positions = move[1..].Split('/');
                            if (positions.Length != 2) throw new FormatException("Invalid exchange move.");
                            int first = int.Parse(positions[0]);
                            int second = int.Parse(positions[1]);
                            if (first < 0 || first >= count || second < 0 || second >= count)
                                throw new FormatException("Exchange positions must fit the program line.");
                            (programs[first], programs[second]) = (programs[second], programs[first]);
                            break;
                        case 'p':
                            if (move.Length != 4 || move[2] != '/') throw new FormatException("Invalid partner move.");
                            int left = Array.IndexOf(programs, move[1]);
                            int right = Array.IndexOf(programs, move[3]);
                            if (left < 0 || right < 0) throw new FormatException("Unknown dance partner.");
                            (programs[left], programs[right]) = (programs[right], programs[left]);
                            break;
                        default: throw new FormatException("Unknown dance move.");
                    }
                }
            }
            return new string(programs);
        }
    }
}
