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
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 3)]
    public class Year2024Day03: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2024Day03()
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

            string data = FileIOHelper.getInstance().ReadDataAsString(file);



            _SW.Start();

            long part1 = CalculateValidInstructions(data);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Sum of multiplications: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            string disablePattern = @"don't\(\)(.*?)(?=do\(\)|$)";

            Regex regex = new Regex(disablePattern);
            string cleanedData = data.Replace("\n", string.Empty);
            long part2 = CalculateValidInstructions(regex.Replace(cleanedData, string.Empty));

            _SW.Stop();

            Console.WriteLine($"  Part 2: Updated Enabled Multiplication Result: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        long CalculateValidInstructions(string data)
        {

            string pattern = @"mul\((\d{1,3}),(\d{1,3})\)";

            MatchCollection matches = Regex.Matches(data, pattern);

            long sum = 0;

            foreach(Match match in matches)
            {
                sum += long.Parse(match.Groups[1].Value) * long.Parse(match.Groups[2].Value);
            }

            return sum;
        }
    }
}
