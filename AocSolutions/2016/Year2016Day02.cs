using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [AdventOfCode(Year = 2016, Day = 02)]
    public class Year2016Day02 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2016Day02()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        private int[,] keypad = { { 1, 2, 3 }, 
                                  { 4, 5, 6 }, 
                                  { 7, 8, 9 } };
        
        private string[,] realKeypad =
        {
            { "", "", "1", "", "" },
            { "", "2", "3", "4", "" }, 
            { "5", "6", "7", "8", "9" },
            { "", "A", "B", "C", "" },
            { "", "", "D", "", "" }
        };

        private Coordinate2D coord = new Coordinate2D(1, 1);
        private Coordinate2D realCoord = new Coordinate2D(0, 2);
        private string code = string.Empty;
        private string realCode = string.Empty;


        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();

            string code = SolvePart1(instructions);

            
            SW.Stop();

            Console.WriteLine("Part 1: Bathroom Code: {0}, Execution Time: {1}", code, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            string realCode = SolvePart2(instructions);

            SW.Stop();

            Console.WriteLine("Part 2: Real Bathroom Code: {0}, Execution Time: {1}", realCode, StopwatchUtil.getInstance().GetTimestamp(SW));


        }

        string SolvePart1(string[] instructions)
        {
            string code = "";
            foreach (string instr in instructions)
            {
                foreach (char c in instr)
                {
                    coord = c switch
                    {
                        'U' => coord.Y - 1 >= 0 ? coord with { Y = coord.Y - 1 } : coord,
                        'D' => coord.Y + 1 <= 2 ? coord with { Y = coord.Y + 1 } : coord,
                        'R' => coord.X + 1 <= 2 ? coord with { X = coord.X + 1 } : coord,
                        _ => coord.X - 1 >= 0 ? coord with { X = coord.X - 1 } : coord
                    };
                }

                int row = coord.Y;
                int col = coord.X;
                code += keypad[row, col];
            }

            return code;
        }

        string SolvePart2(string[] instructions)
        {
            string code = "";
            foreach (string instr in instructions)
            {
                foreach (char c in instr)
                {
                    realCoord = c switch
                    {
                        'U' => realCoord.Y - 1 >= 0 && realKeypad[realCoord.Y - 1, realCoord.X] != String.Empty ? realCoord with { Y = realCoord.Y - 1 } : realCoord,
                        'D' => realCoord.Y + 1 <= 4 && realKeypad[realCoord.Y + 1, realCoord.X] != String.Empty ? realCoord with { Y = realCoord.Y + 1 } : realCoord,
                        'R' => realCoord.X + 1 <= 4 && realKeypad[realCoord.Y, realCoord.X + 1] != String.Empty ? realCoord with { X = realCoord.X + 1 } : realCoord,
                        _ => realCoord.X - 1 >= 0 && realKeypad[realCoord.Y, realCoord.X - 1] != String.Empty ? realCoord with { X = realCoord.X - 1 } : realCoord
                    };
                }

                int row = realCoord.Y;
                int col = realCoord.X;
                code += realKeypad[row, col];
            }

            return code;
        }
    }
}
