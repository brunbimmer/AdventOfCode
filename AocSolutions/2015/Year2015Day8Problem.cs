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
    [AdventOfCode(Year = 2015, Day = 8)]
    public class Year2015Day8Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }



        public Year2015Day8Problem()
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

            var words = FileIOHelper.getInstance().ReadDataAsLines(file);

         
            if (trackTime) SW.Start();
          
            
            //simple regular expression replacements will accurately and conveniently provide expected results.
            int value = words.Sum(w => w.Length - Regex.Replace(w.Trim('"').Replace("\\\"", "A").Replace("\\\\", "B"), "\\\\x[a-f0-9]{2}", "C").Length);


            if (trackTime) SW.Stop();

            Console.WriteLine("  Part 1: Difference between string literals and # of actual characters: {0}", value);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();


            //simple regular expressions to simulate encoding without performing the actual encoding.

            int newValue = words.Sum(w => w.Replace("\\", "AA").Replace("\"", "BB").Length + 2 - w.Length);

            Console.WriteLine("  Part 2: Difference between encoded strings and oroginal # of string literals: {0}", newValue);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }
    }
}
