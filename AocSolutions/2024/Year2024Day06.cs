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
using static Common.Utilities;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 6)]
    public class Year2024Day06 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2024Day06()
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

            var dataMap = FileIOHelper.getInstance().GetDataAsDoubleCharJaggedArray(file);

            _SW.Start();

            int distinctPosition = CalculateNumberOfDistinctPositionVisited(dataMap);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Number of Distinct Positions Visited ==> {distinctPosition}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            int part2 = 0;

            _SW.Stop();

            Console.WriteLine($"  Part 2:  {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        int CalculateNumberOfDistinctPositionVisited(char[,] dataMap)
        {
            (int guardX, int guardY) = FindCharacterPosition(dataMap, '^');

            // Directions: Up, Right, Down, Left
            int[] dx = { -1, 0, 1,  0 };
            int[] dy = {  0, 1, 0, -1 };

            int currentDirection = 0; // Start facing up


            HashSet<(int, int)> visitedPositions = new HashSet<(int, int)>();
            visitedPositions.Add((guardX, guardY));

            while (true)
            {
                // Check how far the guard can move in the current direction
                int nextX = guardX;
                int nextY = guardY;

                while (IsInBounds(nextX + dx[currentDirection], nextY + dy[currentDirection], dataMap) &&
                       dataMap[nextX + dx[currentDirection], nextY + dy[currentDirection]] != '#')
                {
                    nextX += dx[currentDirection];
                    nextY += dy[currentDirection];
                    visitedPositions.Add((nextX, nextY));
                }

                // If the guard cannot move forward, turn right
                if (nextX == guardX && nextY == guardY)
                {
                    currentDirection = (currentDirection + 1) % 4; // Turn right
                }
                else
                {
                    // Update the guard's position
                    guardX = nextX;
                    guardY = nextY;
                }

                // Check if the guard has left the mapped area
                if (!IsInBounds(nextX + dx[currentDirection], nextY + dy[currentDirection], dataMap))
                {
                    break;
                }
            }

            return visitedPositions.Count();
        }

        public (int row, int col) FindCharacterPosition(char[,] charArray, char target)
        {
            // Get the dimensions of the array
            int rows = charArray.GetLength(0);
            int cols = charArray.GetLength(1);

            // Use LINQ to find the character
            var result = Enumerable.Range(0, rows)
                .SelectMany(rowIndex => Enumerable.Range(0, cols)
                    .Select(colIndex => new { rowIndex, colIndex, c = charArray[rowIndex, colIndex] }))
                .FirstOrDefault(x => x.c == target);

            // If found, return the position as a tuple; otherwise, return null
            return (result.rowIndex, result.colIndex);
        }

        bool IsInBounds(int x, int y, char[,] map)
        {
            return x >= 0 && x < map.GetLength(1) && y >= 0 && y < map.GetLength(0);
        }
    }
}
