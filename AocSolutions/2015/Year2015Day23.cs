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
    [AdventOfCode(Year = 2015, Day = 23)]
    public class Year2015Day23 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        Dictionary<string, int> registers = new Dictionary<string, int>();

        public Stopwatch SW { get; set; }

        public Year2015Day23()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 

            registers.Add("a", 0);
            registers.Add("b", 0);

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();

            ProcessInstructionsPart1(instructions);

            SW.Stop();

            Console.WriteLine("Part 1, value of Register B: {0}, Execution Time: {1}", registers["b"], StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            registers["a"] = 1;
            registers["b"] = 0;
            ProcessInstructionsPart1(instructions);
            
            SW.Stop();

            Console.WriteLine("Part 2, value of Register B: {0}, Execution Time: {1}", registers["b"], StopwatchUtil.getInstance().GetTimestamp(SW));
        }

        private void ProcessInstructionsPart1(string[] instructions)
        {
            int instructionIndex = 0;

            while (instructionIndex < instructions.Length && instructionIndex >= 0)
            {
                var pair = instructions[instructionIndex].Split(" ");
                string r;

                switch (pair[0])
                {
                    case "hlf":
                        registers[pair[1]] = registers[pair[1]] / 2; 

                        instructionIndex += 1;
                        break;
                    case "tpl":
                        registers[pair[1]] = registers[pair[1]] * 3; 

                        instructionIndex += 1;
                        break;
                    case "inc":
                        registers[pair[1]] += 1;
                        instructionIndex += 1;
                        break;

                    case "jmp":
                        instructionIndex += Convert.ToInt32(pair[1]);
                        break;
                    
                    case "jie":
                        r = pair[1].Split(",")[0];
                        if (   registers[r] % 2 == 0)
                            instructionIndex += Convert.ToInt32(pair[2]);
                        else
                            instructionIndex += 1;
                        break;

                    case "jio":
                        r = pair[1].Split(",")[0];
                        if (registers[r] == 1)
                            instructionIndex += Convert.ToInt32(pair[2]);                 
                        else
                            instructionIndex += 1;

                        break;

                }
            }
        }
    }
}
