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
    [AdventOfCode(Year = 2025, Day = 6)]
    public class Year2025Day06 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day06()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        private class Problem
        {
            public List<long> Numbers { get; set; }
            public char Operation { get; set; }
        }


        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);


            _SW.Start();

            var problems = ParseProblems(input);           
            long grandTotal = SolveProblems(problems);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Grand Total: {grandTotal}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            var problemsRightToLeft = RotateGridRightToLeft(input);
            long grandTotalPart2 = SolveProblems(problemsRightToLeft);            

            _SW.Stop();

            Console.WriteLine($"  Part 2: Grand Total (Right-to-Left): {grandTotalPart2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }


        private List<Problem> ParseProblems(string[] input)
        {
            int maxWidth = input.Max(l => l.Length);
            int dataRows = input.Length - 1;
            var problems = new List<Problem>();
            var columnGroups = new List<List<int>>();
            var currentGroup = new List<int>();
            
            // Identify column groups separated by spaces
            for (int col = 0; col < maxWidth; col++)
            {
                if (Enumerable.Range(0, dataRows).Any(row => col < input[row].Length && input[row][col] != ' '))
                {
                    currentGroup.Add(col);
                }
                else if (currentGroup.Count > 0)
                {
                    columnGroups.Add(currentGroup);
                    currentGroup = new List<int>();
                }
            }
            if (currentGroup.Count > 0)
                columnGroups.Add(currentGroup);

            // Parse each group into a problem
            foreach (var group in columnGroups)
            {
                int minCol = group.Min();
                int maxCol = group.Max();
                
                var numbers = new List<long>();
                for (int lineIdx = 0; lineIdx < dataRows; lineIdx++)
                {
                    string groupText = new string(Enumerable.Range(minCol, maxCol - minCol + 1)
                        .Select(col => col < input[lineIdx].Length ? input[lineIdx][col] : ' ')
                        .ToArray());
                    
                    numbers.AddRange(groupText.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                        .Where(part => long.TryParse(part, out _))
                        .Select(long.Parse));
                }

                char operation = minCol < input[input.Length - 1].Length ? input[input.Length - 1][minCol] : ' ';
                
                if (numbers.Count > 0)
                    problems.Add(new Problem { Numbers = numbers, Operation = operation });
            }

            return problems;
        }

        private List<Problem> RotateGridRightToLeft(string[] input)
        {
            var problems = new List<Problem>();

            // Read columns from right to left, each column becomes a row (top to bottom)
            // Exclude the last row (operation row) from rotation
            int maxWidth = input.Max(l => l.Length);
            
            // Create a new grid where each row represents a problem (column from original)
            List<string> rotated = new List<string>();
            
            // Iterate columns from right to left
            for (int col = maxWidth - 1; col >= 0; col--)
            {
                StringBuilder sb = new StringBuilder();

                bool _bUniqueRotatedColumnComplete = false;
                
                // Read this column from top to bottom (excluding last row)
                for (int row = 0; row < input.Length; row++)
                {
                    if (input[row][col] == '*' || input[row][col] == '+')
                    {
                        rotated.Add(sb.ToString());  //add the most recent concatenated string to the rotated list
                        
                        Problem problem = new Problem() {
                            Numbers = rotated.Select(s => long.Parse(s)).ToList(),
                            Operation = input[row][col] // Operation is in the last row
                        };

                        problems.Add(problem);
                        rotated.Clear();
                        _bUniqueRotatedColumnComplete = true;
                    }
                    else
                    {
                        if (col < input[row].Length)
                            sb.Append(input[row][col]);
                        else
                            sb.Append(' ');
                    }
                }

                if (!_bUniqueRotatedColumnComplete) {
                    
                    //do not add a completely empty column to rotated list
                    if (sb.ToString().Trim() != "")
                        rotated.Add(sb.ToString());
                }                                              
            }
            
            return problems;
        }

        private long SolveProblems(List<Problem> problems)
        {
            long total = 0;
            
            foreach (var problem in problems)
            {
                long result = problem.Numbers[0];
                
                for (int i = 1; i < problem.Numbers.Count; i++)
                {
                    if (problem.Operation == '+')
                        result += problem.Numbers[i];
                    else if (problem.Operation == '*')
                        result *= problem.Numbers[i];
                }
                
                total += result;
            }
            
            return total;
        }   


    }
}
