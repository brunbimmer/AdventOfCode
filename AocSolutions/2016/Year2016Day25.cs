using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 25)]
    public class Year2016Day25 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private Dictionary<string, int> registers = new Dictionary<string, int>()
        {
            {"a", 7 },
            {"b", 0 },
            {"c", 0 },
            {"d", 0 }
        };

        public Year2016Day25()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            int lowestPositiveInteger = FindLowestPositiveInteger(instructions);

            _SW.Stop();

            Console.WriteLine("Part 1 - Lowest Positive Integer for Clock Signal: {0}, Execution Time: {1}", lowestPositiveInteger, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

           
            
            _SW.Stop();

            //Console.WriteLine("Part 2: {0}, Execution Time: {1}", result2, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       

        int FindLowestPositiveInteger(string[] instructions)
        {
            int a = 1;
            
            while (true)
            {
                // Reset registers and test with current value of a
                var testRegisters = new Dictionary<string, int>()
                {
                    {"a", a },
                    {"b", 0 },
                    {"c", 0 },
                    {"d", 0 }
                };
                
                List<int> output = ExecuteProgram(instructions.ToList(), testRegisters);
                
                // Check if output is alternating 0, 1, 0, 1...
                if (IsValidClockSignal(output))
                {
                    return a;
                }
                
                a++;
            }
        }

        List<int> ExecuteProgram(List<string> instructions, Dictionary<string, int> registers)
        {
            List<int> output = new List<int>();
            int instructionPointer = 0;
            int maxOutputs = 100; // Check first 100 outputs to verify pattern
            int iterations = 0;
            int maxIterations = 1000000; // Prevent infinite loops
            
            while (instructionPointer < instructions.Count && output.Count < maxOutputs && iterations < maxIterations)
            {
                iterations++;
                string[] parts = instructions[instructionPointer].Split(' ');
                
                switch (parts[0])
                {
                    case "cpy":
                        // Skip if destination is not a register
                        if (int.TryParse(parts[2], out _))
                        {
                            instructionPointer++;
                            break;
                        }
                        int value = int.TryParse(parts[1], out int val) ? val : registers[parts[1]];
                        registers[parts[2]] = value;
                        instructionPointer++;
                        break;
                    
                    case "inc":
                        if (registers.ContainsKey(parts[1]))
                            registers[parts[1]]++;
                        instructionPointer++;
                        break;
                    
                    case "dec":
                        if (registers.ContainsKey(parts[1]))
                            registers[parts[1]]--;
                        instructionPointer++;
                        break;
                    
                    case "jnz":
                        try
                        {
                            int checkValue = int.TryParse(parts[1], out int chkVal) ? chkVal : registers[parts[1]];
                            if (checkValue != 0)
                            {
                                int jumpValue = int.TryParse(parts[2], out int jmpVal) ? jmpVal : registers[parts[2]];
                                instructionPointer += jumpValue;
                            }
                            else
                            {
                                instructionPointer++;
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            instructionPointer++;
                        }
                        break;
                    
                    case "out":
                        int outValue = int.TryParse(parts[1], out int outVal) ? outVal : registers[parts[1]];
                        output.Add(outValue);
                        instructionPointer++;
                        break;
                    
                    case "tgl":
                        int tglOffset = int.TryParse(parts[1], out int tglVal) ? tglVal : registers[parts[1]];
                        int targetIndex = instructionPointer + tglOffset;
                        if (targetIndex >= 0 && targetIndex < instructions.Count)
                        {
                            string targetInstruction = instructions[targetIndex];
                            string[] targetParts = targetInstruction.Split(' ');

                            if (targetParts.Length == 2)
                            {
                                if (targetParts[0] == "inc")
                                    targetParts[0] = "dec";
                                else
                                    targetParts[0] = "inc";
                            }
                            else if (targetParts.Length == 3)
                            {
                                if (targetParts[0] == "jnz")
                                    targetParts[0] = "cpy";
                                else
                                    targetParts[0] = "jnz";
                            }

                            instructions[targetIndex] = string.Join(" ", targetParts);
                        }
                        instructionPointer++;
                        break;
                    
                    default:
                        instructionPointer++;
                        break;
                }
            }
            
            return output;
        }

        bool IsValidClockSignal(List<int> output)
        {
            // Must have enough outputs to verify the pattern
            if (output.Count < 10)
                return false;
            
            // Check if alternating 0, 1, 0, 1...
            for (int i = 0; i < output.Count; i++)
            {
                int expected = i % 2;
                if (output[i] != expected)
                    return false;
            }
            
            return true;
        }
    
    }
}
