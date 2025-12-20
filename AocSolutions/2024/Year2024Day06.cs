using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
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

            int part2 = CountObstructionPositionsThatLoop(dataMap);

            _SW.Stop();

            Console.WriteLine($"  Part 2 => Number of different positions for obstructions:  {part2}");
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

        int CountObstructionPositionsThatLoop(char[,] dataMap)
        {
            (int startX, int startY) = FindCharacterPosition(dataMap, '^');

            // Candidates: any position visited in the original walk (excluding start).
            // Placing an obstruction elsewhere can't affect the path.
            var visited = WalkVisitedPositions(dataMap, startX, startY);

            int count = 0;
            foreach (var (x, y) in visited)
            {
                if (x == startX && y == startY)
                    continue;

                if (dataMap[x, y] == '#')
                    continue;

                // Try placing obstruction here
                dataMap[x, y] = '#';
                bool loops = DoesGuardLoop(dataMap, startX, startY);
                dataMap[x, y] = '.'; // restore (input only has '.', '#', '^')

                if (loops)
                    count++;
            }

            // Restore start marker just in case
            dataMap[startX, startY] = '^';
            return count;
        }

        HashSet<(int x, int y)> WalkVisitedPositions(char[,] dataMap, int startX, int startY)
        {
            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            var visited = new HashSet<(int, int)>();

            int x = startX;
            int y = startY;
            int dir = 0;
            visited.Add((x, y));

            while (true)
            {
                int nx = x + dx[dir];
                int ny = y + dy[dir];

                if (!IsInBounds(nx, ny, dataMap))
                    break;

                if (dataMap[nx, ny] == '#')
                {
                    dir = (dir + 1) % 4;
                    continue;
                }

                x = nx;
                y = ny;
                visited.Add((x, y));
            }

            return visited;
        }

        bool DoesGuardLoop(char[,] dataMap, int startX, int startY)
        {
            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            int x = startX;
            int y = startY;
            int dir = 0;

            var seenStates = new HashSet<(int x, int y, int dir)>();

            while (true)
            {
                var state = (x, y, dir);
                if (!seenStates.Add(state))
                    return true; // repeated state => loop

                int nx = x + dx[dir];
                int ny = y + dy[dir];

                if (!IsInBounds(nx, ny, dataMap))
                    return false; // exited

                if (dataMap[nx, ny] == '#')
                {
                    dir = (dir + 1) % 4;
                    continue;
                }

                x = nx;
                y = ny;
            }
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
            // 2D array is [rows, cols] => x is row, y is col
            return x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1);
        }
    }
}
