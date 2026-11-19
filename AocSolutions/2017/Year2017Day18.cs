using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 18)]
    public class Year2017Day18 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day18()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        private readonly record struct Instruction(string Op, string X, string Y);

        private sealed class Process
        {
            public readonly Dictionary<string, long> Registers = new(StringComparer.Ordinal);
            public readonly Queue<long> Inbox = new();
            public long Position;
            public long Sent;

            public Process(int id) { Registers["p"] = id; }
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            Instruction[] program = Parse(FileIOHelper.getInstance().ReadDataAsLines(file));
            _SW.Restart();
            long frequency = Recover(program);
            _SW.Stop();
            output.WritePart(1, frequency, trackTime ? _SW.Elapsed : null, "Recovered sound frequency");
            _SW.Restart();
            long sent = CountSends(program);
            _SW.Stop();
            output.WritePart(2, sent, trackTime ? _SW.Elapsed : null, "Messages sent by program one");
        }

        private Instruction[] Parse(string[] lines)
        {
            return lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line =>
            {
                string[] parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                int operands = parts[0] switch
                {
                    "snd" or "rcv" => 1,
                    "set" or "add" or "mul" or "mod" or "jgz" => 2,
                    _ => throw new FormatException("Unknown duet instruction.")
                };
                if (parts.Length != operands + 1) throw new FormatException("Invalid duet instruction operands.");
                return new Instruction(parts[0], parts[1], operands == 2 ? parts[2] : null);
            }).ToArray();
        }

        private long Value(string operand, Dictionary<string, long> registers)
        {
            if (long.TryParse(operand, out long literal)) return literal;
            return registers.GetValueOrDefault(operand);
        }

        private void Execute(Instruction instruction, Dictionary<string, long> registers, ref long position)
        {
            long right = Value(instruction.Y, registers);
            long left = Value(instruction.X, registers);
            switch (instruction.Op)
            {
                case "set": registers[instruction.X] = right; break;
                case "add": registers[instruction.X] = left + right; break;
                case "mul": registers[instruction.X] = left * right; break;
                case "mod": registers[instruction.X] = left % right; break;
                case "jgz":
                    position += left > 0 ? right : 1;
                    return;
                default: throw new FormatException("Unexpected duet arithmetic instruction.");
            }
            position++;
        }

        private long Recover(Instruction[] program)
        {
            var registers = new Dictionary<string, long>(StringComparer.Ordinal);
            long? sound = null;
            long position = 0;
            while (position >= 0 && position < program.Length)
            {
                Instruction instruction = program[(int)position];
                if (instruction.Op == "snd")
                {
                    sound = Value(instruction.X, registers);
                    position++;
                }
                else if (instruction.Op == "rcv")
                {
                    if (Value(instruction.X, registers) != 0)
                        return sound ?? throw new InvalidOperationException("No sound was played before recovery.");
                    position++;
                }
                else Execute(instruction, registers, ref position);
            }
            throw new InvalidOperationException("The duet program terminated without recovering a sound.");
        }

        private long CountSends(Instruction[] program)
        {
            var first = new Process(0);
            var second = new Process(1);
            // Both processes get a turn even when the first one is blocked or terminated.
            while (Step(program, first, second) | Step(program, second, first)) { }
            return second.Sent;
        }

        private bool Step(Instruction[] program, Process process, Process other)
        {
            if (process.Position < 0 || process.Position >= program.Length) return false;
            Instruction instruction = program[(int)process.Position];
            if (instruction.Op == "snd")
            {
                other.Inbox.Enqueue(Value(instruction.X, process.Registers));
                process.Sent++;
                process.Position++;
            }
            else if (instruction.Op == "rcv")
            {
                if (process.Inbox.Count == 0) return false;
                process.Registers[instruction.X] = process.Inbox.Dequeue();
                process.Position++;
            }
            else Execute(instruction, process.Registers, ref process.Position);
            return true;
        }
    }
}
