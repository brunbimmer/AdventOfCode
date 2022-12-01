using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 1)]
    public class Year2017Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2017Day01()
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
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            int[] measurements = FileIOHelper.getInstance().ReadDataToIntArray(file);
            
            if (trackTime) SW.Start();
            int increases = Part1(measurements);
            if (trackTime) SW.Stop();



            Console.WriteLine("  Part 1: Number of increases (Actual Measurements): {0}", increases);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));
            
            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            increases = Part2(measurements);
            if (trackTime) SW.Stop();

            Console.WriteLine("  Part 1: Number of increases (Sliding Measurements): {0}", increases);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        private int Part1(int[] measurements)
        {
            return 0;
        }

        private int Part2(int[] measurements)
        {
            return 0;
        }
    }
}
