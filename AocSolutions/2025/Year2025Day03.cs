using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2025, Day = 3)]
    public class Year2025Day03: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day03()
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

            string[] input  = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            long totalJoltage = FindTotalOutputVoltagePairs(input);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Total Joltage : {totalJoltage}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long totalJoltage2 = FindTotalOutputVoltage12Batteries(input);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Total Joltage : {totalJoltage2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        public long FindTotalOutputVoltagePairs(string[] input)
        {
            long sum = 0;
             
            foreach (string line in input)
            {
                //Break up line into characters and convert to integer array
                int[] batteryBanks = line.Select(c => int.Parse(c.ToString())).ToArray();

                List<int> pairs = new List<int>();

                for (int i = 0; i < batteryBanks.Length - 1; i++)
                {
                    for (int j = i + 1; j < batteryBanks.Length; j++)
                    {
                        int pair = int.Parse($"{batteryBanks[i]}{batteryBanks[j]}");
                        pairs.Add(pair);
                    }
                }

                //find in-order largest battery sequence.
                long largestPair = pairs.Max();
                
                sum += largestPair;
            }

            return sum;
        }

        public long FindTotalOutputVoltage12Batteries(string[] input)
        {
            long sum = 0;
            foreach (string line in input)
            {
                int targetLength = 12;
                        
                // Use a greedy approach: at each position, pick the largest digit 
                // that still allows us to get enough remaining digits
                string result = "";
                int currentIndex = 0;
                
                for (int position = 0; position < targetLength; position++)
                {
                    // How many more digits do we need after this one?
                    int remaining = targetLength - position;
                    
                    // We need to find the largest digit in the range where we can 
                    // still get enough digits after it
                    int maxIndex = line.Length - remaining;
                    
                    // Find the largest digit from currentIndex to maxIndex
                    int bestIndex = currentIndex;
                    for (int i = currentIndex; i <= maxIndex; i++)
                    {
                        if (line[i] > line[bestIndex])
                        {
                            bestIndex = i;
                        }
                    }
                    
                    result += line[bestIndex];
                    currentIndex = bestIndex + 1;
                }
                sum += long.Parse(result);
            }

            return sum;
        }
    }
}
