using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 8)]
    public class Year2020Day08 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day08()
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
            var instructions = lines.Select(line =>
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return (Operation: parts[0], Argument: int.Parse(parts[1]));
            }).ToArray();

            _SW.Restart();
            int accumulator = Part1(instructions);
            _SW.Stop();

            output.WritePart(1, accumulator, trackTime ? _SW.Elapsed : null, "Accumulator before an instruction repeats");


            _SW.Restart();
            accumulator = Part2(instructions);
            _SW.Stop();

            output.WritePart(2, accumulator, trackTime ? _SW.Elapsed : null, "Accumulator after repairing the program");

        }

        private int Part1((string Operation, int Argument)[] instructions)
        {
            return RunProgram(instructions).Accumulator;
        }

        private int Part2((string Operation, int Argument)[] instructions)
        {
            var predecessors = new List<int>[instructions.Length + 1];
            for (int position = 0; position < predecessors.Length; position++)
                predecessors[position] = new List<int>();

            for (int position = 0; position < instructions.Length; position++)
            {
                var instruction = instructions[position];
                int next = instruction.Operation == "jmp" ? position + instruction.Argument : position + 1;
                if (next >= 0 && next <= instructions.Length)
                    predecessors[next].Add(position);
            }

            // Work backward from the only valid exit to find terminating suffixes.
            bool[] reachesExit = new bool[instructions.Length + 1];
            Queue<int> pending = new Queue<int>();
            reachesExit[instructions.Length] = true;
            pending.Enqueue(instructions.Length);

            while (pending.Count > 0)
            {
                foreach (int predecessor in predecessors[pending.Dequeue()])
                {
                    if (!reachesExit[predecessor])
                    {
                        reachesExit[predecessor] = true;
                        pending.Enqueue(predecessor);
                    }
                }
            }

            bool[] visited = new bool[instructions.Length];
            int current = 0;
            while (current >= 0 && current < instructions.Length && !visited[current])
            {
                visited[current] = true;
                var instruction = instructions[current];
                if (instruction.Operation is "jmp" or "nop")
                {
                    int alternate = instruction.Operation == "jmp" ? current + 1 : current + instruction.Argument;
                    if (alternate >= 0 && alternate <= instructions.Length && reachesExit[alternate])
                    {
                        var result = RunProgram(instructions, current);
                        if (result.Terminated)
                            return result.Accumulator;
                    }
                }

                current = instruction.Operation == "jmp" ? current + instruction.Argument : current + 1;
            }

            throw new InvalidOperationException("No single jmp/nop change makes the program terminate.");
        }

        private (int Accumulator, bool Terminated) RunProgram(
            (string Operation, int Argument)[] instructions, int swapPosition = -1)
        {
            bool[] visited = new bool[instructions.Length];
            int position = 0;
            int accumulator = 0;

            while (position != instructions.Length)
            {
                if (position < 0 || position > instructions.Length)
                    throw new InvalidOperationException("Instruction position is outside the program.");

                if (visited[position])
                    return (accumulator, false);

                visited[position] = true;
                var instruction = instructions[position];
                string operation = instruction.Operation;
                if (position == swapPosition)
                    operation = operation == "jmp" ? "nop" : "jmp";

                switch (operation)
                {
                    case "acc":
                        accumulator += instruction.Argument;
                        position++;
                        break;
                    case "jmp":
                        position += instruction.Argument;
                        break;
                    case "nop":
                        position++;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown operation: {instruction.Operation}");
                }
            }

            return (accumulator, true);
        }
    }
}
