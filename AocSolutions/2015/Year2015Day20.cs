using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 20)]
    public class Year2015Day20 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2015Day20()
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

            //No file...input in the puzzle.

            int presents = 29000000;

            _SW.Start();                
            
            int min = int.MaxValue;
	        int[] houses = new int[200000000];
	        for(int i = 1; i < presents / 10; ++i)
		        for(int j = i; j > 0 && j < houses.Length && j < min; j = unchecked(j + i))
			        if((houses[j] += i * 10) >= presents)
			            min = Math.Min(min, j);
            
            _SW.Stop();

            Console.WriteLine("Part 1: Smallest House # {0}, Execution Time: {1}", min, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

           	int min2 = int.MaxValue;
	        int[] houses2 = new int[200000000];
	        for(int i = 1; i < presents / 10; ++i)
		        for(int j = i, c = 0; c < 50 && j < houses2.Length && j < min2; j = unchecked(j + i), ++c)
			        if((houses2[j] += i * 11) >= presents)
				        min2 = Math.Min(min2, j);
            
            _SW.Stop();

            Console.WriteLine("Part 2: Smallest House # {0}, Execution Time: {1}", min2, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       
    }
}
