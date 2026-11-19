using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 8)]
    public class Year2017Day08 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day08()
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
            var result = Execute(FileIOHelper.getInstance().ReadDataAsLines(file));
            _SW.Stop();
            output.WritePart(1, result.FinalMaximum, trackTime ? _SW.Elapsed : null, "Largest register value after execution");
            output.WritePart(2, result.HighestEver, description: "Highest register value during execution");
        }

        private (long FinalMaximum, long HighestEver) Execute(string[] lines)
        {
            string[][] instructions = lines.Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)).ToArray();
            var registers = new Dictionary<string, long>(StringComparer.Ordinal);
            foreach (string[] instruction in instructions)
            {
                if (instruction.Length != 7 || instruction[3] != "if")
                    throw new FormatException("Invalid conditional register instruction.");
                registers.TryAdd(instruction[0], 0);
                registers.TryAdd(instruction[4], 0);
            }
            long highest = 0;
            foreach (string[] instruction in instructions)
            {
                long left = registers[instruction[4]];
                long right = long.Parse(instruction[6]);
                bool condition = instruction[5] switch
                {
                    ">" => left > right, "<" => left < right,
                    ">=" => left >= right, "<=" => left <= right,
                    "==" => left == right, "!=" => left != right,
                    _ => throw new FormatException("Unknown register comparison.")
                };
                long change = instruction[1] switch
                {
                    "inc" => long.Parse(instruction[2]),
                    "dec" => -long.Parse(instruction[2]),
                    _ => throw new FormatException("Unknown register operation.")
                };
                if (condition)
                {
                    registers[instruction[0]] = checked(registers[instruction[0]] + change);
                    highest = Math.Max(highest, registers[instruction[0]]);
                }
            }
            return (registers.Values.DefaultIfEmpty(0).Max(), highest);
        }
    }
}
