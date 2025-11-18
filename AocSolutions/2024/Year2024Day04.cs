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
    [AdventOfCode(Year = 2024, Day = 4)]
    public class Year2024Day04: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2024Day04()
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

            var input = FileIOHelper.getInstance().GetDataAsRectangularArray(file);


            _SW.Start();

            int count = FindXmasInstances(input, "XMAS");

            _SW.Stop();

            Console.WriteLine($"  Part 1: Number of XMAS Instances ==> {count}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            int count2 = FindMasInstances(input, "MAS");

            _SW.Stop();

            Console.WriteLine($"  Part 2: Number of X-MAS Instances ==> {count2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        int FindXmasInstances(char[,] grid, string word)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int count = 0;

            // Check each cell in the grid
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // If the current cell matches the first letter of the word
                    if (grid[i, j] == word[0])
                    {
                        // Check all possible directions
                        if (SearchInDirection(grid, word, i, j, -1, -1)) count++; // Top-left
                        if (SearchInDirection(grid, word, i, j, -1, 0))  count++;  // Top
                        if (SearchInDirection(grid, word, i, j, -1, 1))  count++;  // Top-right
                        if (SearchInDirection(grid, word, i, j, 0, -1))  count++;  // Left
                        if (SearchInDirection(grid, word, i, j, 0, 1))   count++;   // Right
                        if (SearchInDirection(grid, word, i, j, 1, -1))  count++;  // Bottom-left
                        if (SearchInDirection(grid, word, i, j, 1, 0))   count++;   // Bottom
                        if (SearchInDirection(grid, word, i, j, 1, 1))   count++;   // Bottom-right
                    }
                }
            }

            return count; // Return the total count of found instances
        }

        int FindMasInstances(char[,] grid, string word)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int count = 0;

            // Check each cell in the grid
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Look for A's
                    if (grid[i, j] == word[0])
                    {
                        // Check all possible directions
                        if (SearchInDirection(grid, word, i, j, -1, -1))
                        {
                            //top-left
                            if (   (grid[i - 2, j] == 'M' && grid[i, j - 2] == 'S')
                                || (grid[i - 2, j] == 'S' && grid[i, j - 2] == 'M'))

                            count++;

                        }

                        if (SearchInDirection(grid, word, i, j, -1, 1))
                        {
                            // Top-right
                            if (   (grid[i - 2, j] == 'M' && grid[i, j + 2] == 'S')
                                || (grid[i - 2, j] == 'S' && grid[i, j + 2] == 'M'))

                                count++;
                        }

                        if (SearchInDirection(grid, word, i, j, 1, -1))
                        {   // Bottom-left

                            if (   (grid[i + 2, j] == 'M' && grid[i, j - 2] == 'S')
                                || (grid[i + 2, j] == 'S' && grid[i, j - 2] == 'M'))

                                count++;
                        }

                        if (SearchInDirection(grid, word, i, j, 1, 1))
                        {
                            // Bottom-right

                            if (   (grid[i + 2, j] == 'M' && grid[i, j + 2] == 'S')
                                || (grid[i + 2, j] == 'S' && grid[i, j + 2] == 'M'))

                                count++;

                        }
                    }
                }
            }

            // We need to divide this count by half since we would be counting doubles using the above algorithm
            return count / 2; 
        }



        bool SearchInDirection(char[,] grid, string word, int row, int col, int rowDir, int colDir)
        {
            int wordLength = word.Length;
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            // Check if the word fits in the grid in the given direction
            if (row + (wordLength - 1) * rowDir >= 0 && row + (wordLength - 1) * rowDir < rows &&
                col + (wordLength - 1) * colDir >= 0 && col + (wordLength - 1) * colDir < cols)
            {
                // Check if the word matches in the given direction
                for (int i = 1; i < wordLength; i++)
                {
                    int newRow = row + i * rowDir;
                    int newCol = col + i * colDir;

                    if (grid[newRow, newCol] != word[i])
                    {
                        return false; // Word doesn't match
                    }
                }

                return true; // Word found
            }

            return false; // Word doesn't fit in the grid in the given direction
        }



    }
}
