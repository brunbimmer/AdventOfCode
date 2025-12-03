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

namespace AdventOfCode
{
    [AdventOfCode(Year = 2025, Day = 1)]
    public class Year2025Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day01()
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

            var (part1, part2) = AnalyzeDial(lines);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Password (Zero Landings): {part1}");
            Console.WriteLine($"  Part 2: Times Passing Through Zero: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

        }

        (int landings, int passes) AnalyzeDial(string[] rotations)
        {
            int position = 50;
            int landings = 0;
            int passes = 0;
            
            foreach (string rotationLine in rotations)
            {
                string rotation = rotationLine.Trim();
                
                char direction = rotation[0]; // 'L' or 'R'
                int distance = int.Parse(rotation.Substring(1));
                
                if (direction == 'L')
                {
                    // Moving left: check if we cross 0
                    int startPos = position;
                    for (int i = 1; i <= distance; i++)
                    {
                        int newPos = (startPos - i) % 100;
                        if (newPos < 0) newPos += 100;
                        if (newPos == 0)
                        {
                            passes++;
                        }
                    }
                    position = (startPos - distance) % 100;
                    if (position < 0) position += 100;
                }
                else // 'R'
                {
                    // Moving right: check if we cross 0
                    int startPos = position;
                    for (int i = 1; i <= distance; i++)
                    {
                        int newPos = (startPos + i) % 100;
                        if (newPos == 0)
                        {
                            passes++;
                        }
                    }
                    position = (startPos + distance) % 100;
                }
                
                // Part 1: Count landings (final position after each rotation)
                if (position == 0)
                {
                    landings++;
                }
            }
            
            return (landings, passes);
        }
    }
}
