using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using CommonAlgorithms;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 10)]
    public class Year2015Day10Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        
        public Year2015Day10Problem()
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

            //simple input, file access not required

            SW.Start();

            string sequence = "3113322113";
                
            foreach (int i in Enumerable.Range(1, 40))
            {
                sequence = LookAndSay(sequence);
            }
                           

            SW.Stop();            

            Console.WriteLine("  Part 1: When applying the Look And Say Sequence 40 times, the length of the final number is: {0}", sequence.Length);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            sequence = "3113322113";

            foreach (int i in Enumerable.Range(1, 50))
            {
                sequence = LookAndSay(sequence);
            }

            SW.Stop();

            Console.WriteLine("  Part 1: When applying the Look And Say Sequence 50 times, the length of the final number is: {0}", sequence.Length);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));
            

            Console.WriteLine("\n===========================================\n");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        private string LookAndSay(string input)
        {
            StringBuilder result = new StringBuilder();

            char repeat = input[0];
            input = input.Substring(1, input.Length-1)+" ";
            int times = 1;
      
            foreach (char actual in input)
            {
                if (actual != repeat)
                {
                    result.Append(Convert.ToString(times)+repeat);
                    times = 1;
                    repeat = actual;
                }
                else
                {
                    times += 1;
                }
            }
            return result.ToString();
        }

    }
}
