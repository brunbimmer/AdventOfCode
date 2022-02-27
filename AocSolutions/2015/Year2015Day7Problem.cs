using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 7)]
    public class Year2015Day7Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        Dictionary<string, string> instructionSet;
        Dictionary<string, int> cachedValues;


        public Year2015Day7Problem()
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

            //no need to retrieve input from AOC. The input is a simple string
            string filename = _OverrideFile ?? path;

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            instructionSet = BuildInstructions(lines);
            cachedValues = new Dictionary<string, int>();

            if (trackTime) SW.Start();
          
            int value = RunSimulation();

            if (trackTime) SW.Stop();

            Console.WriteLine("  Part 1: Signal that is sent to Wire A: {0}", value);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            cachedValues.Clear();

            //For part 2, we override the value at signal "b" with the result of Part 1 and re-run the simulation
            instructionSet["b"] = value.ToString();

            int newValue = RunSimulation();

            Console.WriteLine("  Part 2: Update signal B to {0}. New value at Wire A: {1}", value, newValue);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        private Dictionary<string, string> BuildInstructions(string[] lines)
        {
            Dictionary<string, string> instructions = new Dictionary<string, string>();

            foreach (string line in lines)
            {
                string[] operands = line.Split("->");
                instructions.Add(operands[1].Trim(), operands[0].Trim());
            }

            return instructions;
        }


        public int RunSimulation()
        {
            int ValueAtA = ComputeResult(instructionSet.GetValueOrDefault("a"));

            return ValueAtA;
            
        }

        public int ComputeResult(string operation)
        {
            int numericValue;
            if (int.TryParse(operation, out numericValue))
                return numericValue;
            else
            {
                string[] operationElements = operation.Split(' ');

                if (operationElements.Length == 1)
                {
                    return GetValue(operationElements[0]);
                }
                else if (operationElements.Length == 2)
                {
                    //the only 2 argument operation is a NOT operation
                    
                    int rightValue;
                    if (int.TryParse(operationElements[1], out rightValue) == false)
                        rightValue = GetValue(operationElements[1]);

                    return ~rightValue;
                }
                else
                {
                    //this implies a bitwise operation between an expression or value.

                    int leftValue;

                    if (int.TryParse(operationElements[0], out leftValue) == false)
                        leftValue = GetValue(operationElements[0]);
                        
                    int rightValue;

                    if (int.TryParse(operationElements[2], out rightValue) == false)
                        rightValue = GetValue(operationElements[2]);                      

                    switch (operationElements[1])
                    {
                        case "AND":
                            return leftValue & rightValue;
                        case "OR":
                            return leftValue | rightValue;
                        case "LSHIFT":
                            return leftValue << rightValue;
                        case "RSHIFT":
                            return leftValue >> rightValue;
                    }
                }
            }
            //this will not happen, but return a default value of 0;
            return 0;
        }       

        private int GetValue(string signal)
        {
            int value;
            if (!cachedValues.ContainsKey(signal))
            {
                value = ComputeResult(instructionSet.GetValueOrDefault(signal));
                cachedValues.Add(signal, value);
            }
            else
                value = cachedValues[signal];

            return value;
        }

    }
}
