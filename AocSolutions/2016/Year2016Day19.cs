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
    [AdventOfCode(Year = 2016, Day = 19)]
    public class Year2016Day19 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day19()
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
 

            int numberOfElves = 3001330; // Hardcoded input for Day 19

            _SW.Start();                       

            double elfWithAllPresents = SolvePart1(numberOfElves);

            
            _SW.Stop();

            Console.WriteLine("Part 1 - Elf all the presents: {0}, Execution Time: {1}", elfWithAllPresents, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            double elfWithPresentsPart2 = SolvePart2(numberOfElves);
            
            _SW.Stop();

            Console.WriteLine("Part 2 - Elf all the presents: {0}, Execution Time: {1}", elfWithPresentsPart2, StopwatchUtil.getInstance().GetTimestamp(_SW));

        }       

        double SolvePart1(int n)
        {
            int msb = (int)Math.Log2(n);
            return ((n ^ (1 << msb)) << 1) + 1;
        }

        double SolvePart2(int n)
        {
            int l = (int)Math.Floor(Math.Log(n, 3));
            int k = n - (int)Math.Pow(3, l);
            
            if (k == 0)
            {
                return n; // is a power of 3
            }
            
            if (l == 1 || k <= Math.Pow(3, l))
            {
                return k;
            }
            else
            {
                return Math.Pow(3, l) + 2 * (k - Math.Pow(3, l));
            }
        }
    }
}
