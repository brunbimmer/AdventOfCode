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
    [AdventOfCode(Year = 2017, Day = 24)]
    public class Year2017Day24 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day24()
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

            //Dictionary<(int, int), int> octopusGrid = FileIOHelper.getInstance().GetDataAsMap(file);

            _SW.Start();                       



            
            _SW.Stop();

            //Console.WriteLine("Part 1: {0}, Execution Time: {1}", result1, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

           
            
            _SW.Stop();

            //Console.WriteLine("Part 2: {0}, Execution Time: {1}", result2, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       
    }
}
