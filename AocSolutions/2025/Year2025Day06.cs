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
            int dataRows = input.Length - 1; // Exclude operation row
            
            // Identify column groups (separated by empty columns)
            List<List<int>> columnGroups = new List<List<int>>();
            List<int> currentGroup = new List<int>();
            
            for (int col = 0; col < maxWidth; col++)
            {
                bool hasNonSpace = false;
                
                // Check if this column has any non-space characters (excluding last row)
                for (int row = 0; row < dataRows; row++)
                {
                    if (col < input[row].Length && input[row][col] != ' ')
                    {
                        hasNonSpace = true;
                        break;
                    }
                }

                if (hasNonSpace)
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

            // Parse problems from column groups
            var problems = new List<Problem>();

            foreach (var group in columnGroups)
            {
                var numbers = new List<long>();
                char operation = ' ';

                // Read each line horizontally (except last which is operation)
                for (int lineIdx = 0; lineIdx < dataRows; lineIdx++)
                {
                    int minCol = group.Min();
                    int maxCol = group.Max();
                    
                    // Extract substring for this group
                    string groupText = "";
                    for (int col = minCol; col <= maxCol; col++)
                    {
                        if (col < input[lineIdx].Length)
                            groupText += input[lineIdx][col];
                    }
                    
                    // Parse numbers from this line (split by spaces)
                    var parts = groupText.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (long.TryParse(part, out var num))
                            numbers.Add(num);
                    }
                }

                // Last line has the operation (read from first column in group)
                int minOpCol = group.Min();
                if (minOpCol < input[input.Length - 1].Length)
                {
                    operation = input[input.Length - 1][minOpCol];
                }

                if (numbers.Count > 0)
                {
                    problems.Add(new Problem 
                    { 
                        Numbers = numbers, 
                        Operation = operation 
                    });
                }
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
