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

        public Stopwatch _SW { get; set; }

        public Year2017Day01()
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

            string input = FileIOHelper.getInstance().ReadDataAsString(file);
            
            _SW.Start();
            
            int sum = SolvePart1(input);

            _SW.Stop();

            Console.WriteLine("  Part 1 - Result: {0}. Execution Time: {1}", sum, StopwatchUtil.getInstance().GetTimestamp(_SW));            
            
            _SW.Restart();

            sum = SolvePart2(input);

            _SW.Stop();

            Console.WriteLine("  Part 2 - result: {0}. Execution Time: {1}", sum, StopwatchUtil.getInstance().GetTimestamp(_SW));            

        }

        int SolvePart1(string input)
        {
            string digits = input.Trim();
            int sum = 0;
            
            for (int i = 0; i < digits.Length; i++)
            {
                int nextIndex = (i + 1) % digits.Length; // Circular
                if (digits[i] == digits[nextIndex])
                {
                    sum += int.Parse(digits[i].ToString());
                }
            }
            
            return sum;
        }

        int SolvePart2(string input)
        {
            string digits = input.Trim();
            int sum = 0;
            int halfwayPoint = digits.Length / 2;
            
            for (int i = 0; i < digits.Length; i++)
            {
                int compareIndex = (i + halfwayPoint) % digits.Length;
                if (digits[i] == digits[compareIndex])
                {
                    sum += int.Parse(digits[i].ToString());
                }
            }
            
            return sum;
        }

    }
}
