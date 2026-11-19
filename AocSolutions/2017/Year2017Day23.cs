using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 23)]
    public class Year2017Day23 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day23()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        private readonly record struct Instruction(string Op, string X, string Y);

        private static readonly Instruction[] FactorLoop =
        {
            new("set", "d", "2"), new("set", "e", "2"), new("set", "g", "d"),
            new("mul", "g", "e"), new("sub", "g", "b"), new("jnz", "g", "2"),
            new("set", "f", "0"), new("sub", "e", "-1"), new("set", "g", "e"),
            new("sub", "g", "b"), new("jnz", "g", "-8"), new("sub", "d", "-1"),
            new("set", "g", "d"), new("sub", "g", "b"), new("jnz", "g", "-13")
        };

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            Instruction[] program = Parse(FileIOHelper.getInstance().ReadDataAsLines(file));
            _SW.Restart();
            long first = Run(program, 0, false).Multiplications;
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Multiplication instructions executed");
            _SW.Restart();
            long second = Run(program, 1, true).H;
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Register h after disabling debug mode");
        }

        private Instruction[] Parse(string[] lines)
        {
            return lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line =>
            {
                string[] parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3 || (parts[0] != "set" && parts[0] != "sub" && parts[0] != "mul" && parts[0] != "jnz"))
                    throw new FormatException("Invalid coprocessor instruction.");
                return new Instruction(parts[0], parts[1], parts[2]);
            }).ToArray();
        }

        private long Value(string operand, Dictionary<string, long> registers)
        {
            return long.TryParse(operand, out long literal) ? literal : registers.GetValueOrDefault(operand);
        }

        private (long Multiplications, long H) Run(Instruction[] program, long initialA, bool optimize)
        {
            var registers = new Dictionary<string, long>(StringComparer.Ordinal) { ["a"] = initialA };
            long multiplications = 0;
            long position = 0;
            while (position >= 0 && position < program.Length)
            {
                Instruction instruction = program[(int)position];
                long b = registers.GetValueOrDefault("b");
                if (optimize && b >= 3 && instruction == FactorLoop[0] && position + FactorLoop.Length <= program.Length
                    && program.AsSpan((int)position, FactorLoop.Length).SequenceEqual(FactorLoop))
                {
                    // The nested d/e loops test whether b has a proper factor.
                    if (IsComposite(b)) registers["f"] = 0;
                    registers["d"] = b;
                    registers["e"] = b;
                    registers["g"] = 0;
                    multiplications = checked(multiplications + checked((b - 2) * (b - 2)));
                    position += FactorLoop.Length;
                    continue;
                }
                long right = Value(instruction.Y, registers);
                switch (instruction.Op)
                {
                    case "set": registers[instruction.X] = right; break;
                    case "sub": registers[instruction.X] = Value(instruction.X, registers) - right; break;
                    case "mul":
                        registers[instruction.X] = Value(instruction.X, registers) * right;
                        multiplications++;
                        break;
                    case "jnz":
                        position += Value(instruction.X, registers) != 0 ? right : 1;
                        continue;
                }
                position++;
            }
            return (multiplications, registers.GetValueOrDefault("h"));
        }

        private bool IsComposite(long value)
        {
            for (long divisor = 2; divisor <= value / divisor; divisor++)
                if (value % divisor == 0) return true;
            return false;
        }
    }
}
