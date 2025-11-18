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
using MoreLinq.Extensions;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 2)]
    public class Year2024Day02: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2024Day02()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);


            _SW.Start();

            int safeReports = 0;
            int safeReportsWithDampening = 0;

            foreach (string line in lines)
            {
                List<int> numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
                                    .ToList();

                if (IsSafe(numbers))
                    safeReports++;

                if (IsSafeWithDampening(numbers))
                    safeReportsWithDampening++;
            }

            _SW.Stop();

            Console.WriteLine($"  Part 1: Number of Safe Reports:             {safeReports}");           
            Console.WriteLine($"  Part 2: Safe Reports with Problem Dampener: {safeReportsWithDampening}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

        }

        bool IsSafe(List<int> numbers)
        {
            bool isIncreasing = ((numbers[1] - numbers[0]) > 0) ? true: false;


            if (isIncreasing)
            {
                return numbers.Zip(numbers.Skip(1), (current, next) => next - current)
                  .All(difference => difference == 1 || difference == 2 || difference == 3);
            }
            else
            { 
                return numbers.Zip(numbers.Skip(1), (current, next) => current - next)
                  .All(difference => difference == 1 || difference == 2 || difference == 3);
            }

        }

        bool IsSafeWithDampening(List<int> numbers)
        {

            if (IsSafe(numbers))
                return true;
            else
            {
                for(int i = 0; i < numbers.Count(); i++)
                {
                    var numbersTest = numbers.ToList();
                    numbersTest.RemoveAt(i);
                    if (IsSafe(numbersTest))
                        return true;

                }

                return false;
            }
        }
    }
}
