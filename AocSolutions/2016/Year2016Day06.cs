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
using MoreLinq;
using MoreLinq.Extensions;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 6)]
    public class Year2016Day06 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2016Day06()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();

            string[][] elements = lines.Select(line => line.Select(c => c.ToString()).ToArray()).ToArray();

            // Transpose the elements using LINQ
            string[][] transposed = elements[0]
                                        .Select((_, index) => elements.Select(row => row[index]).ToArray())
                                        .ToArray();

            // Join the transposed elements into lines
            string[] transposedLines = transposed.Select(row => string.Join("", row)).ToArray();


            var code1 = new StringBuilder();
            var code2 = new StringBuilder();

            foreach (string line in transposedLines)
            {
                code1.Append(line.GroupBy(c => c).OrderByDescending(c => c.Count()).Select(c => c.Key).First());
                code2.Append(line.GroupBy(c => c).OrderBy(c => c.Count()).Select(c => c.Key).First());
            }

            SW.Stop();

            Console.WriteLine($"Part 1: Code based on most common letter per column: {code1}");
            Console.WriteLine($"Part 2: Code based on least common letter per column: {code2}");
            Console.WriteLine($"    Execution Time: {StopwatchUtil.getInstance().GetTimestamp(SW)}");


        }       
    }
}
