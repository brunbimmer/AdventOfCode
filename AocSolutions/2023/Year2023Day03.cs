using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;
using Microsoft.Extensions.Primitives;
using MoreLinq;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 3)]
    public class Year2023Day03: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        int total = 0;
        private List<string> board = new List<string>();
        private Dictionary<(int, int), List<int>> gearNumbers = new Dictionary<(int, int), List<int>>();


        public Year2023Day03()
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

            board = FileIOHelper.getInstance().ReadDataAsLines(file).ToList();
           
            _SW.Start();

            Regex num_pattern = new Regex(@"\d+");

            for (int row = 0; row < board.Count; row++)
            {
                foreach (Match match in num_pattern.Matches(board[row]))
                {
                    if (LookForNeighbourSymbols(row - 1, match.Index - 1, row + 1, match.Index + match.Length, int.Parse(match.Value)))
                    {
                        total += int.Parse(match.Value);
                    }
                }
            }

            _SW.Stop();


            Console.WriteLine($"  Part 1: Sum of Value Part Numbers: {total}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


            _SW.Restart();

            int ratioTotal = 0;
            foreach (var pairs in gearNumbers)
            {
                if (pairs.Value.Count == 2)
                {
                    ratioTotal += pairs.Value[0] * pairs.Value[1];
                }
            }
            _SW.Stop();

            Console.WriteLine($"  Part 2: Sum of Gear Ratios Numbers: {ratioTotal}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }
        

        bool LookForNeighbourSymbols(int start_y, int start_x, int end_y, int end_x, int num)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                for (int x = start_x; x <= end_x; x++)
                {
                    if (y >= 0 && y < board.Count && x >= 0 && x < board[y].Length)
                    {
                        if (!"0123456789.".Contains(board[y][x]))
                        {
                            //Part 2, track gears separately
                            if (board[y][x] == '*')
                            {
                                if (!gearNumbers.ContainsKey((y, x)))
                                    gearNumbers.Add((y, x), new List<int>());

                                gearNumbers[(y, x)].Add(num);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
