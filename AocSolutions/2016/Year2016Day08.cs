using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 8)]
    public class Year2016Day08 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        char[,] map = new char[6, 50];

        public Year2016Day08()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 



            // Initialize the map with empty entries
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 50; j++)
                    map[i, j] = ' ';

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            ParseDisplayInstructions(instructions);

            int  count = map.Cast<char>().Count(c => c == '#');

            SW.Stop();

            Console.WriteLine($"Part 1 - Number of pixels that are lit: {count}, Execution Time: {StopwatchUtil.getInstance().GetTimestamp(SW)}");

            SW.Restart();

            for (int a = 0; a < map.GetLength(0); a++)
            {
                for (int b = 0; b < map.GetLength(1); b++)
                {
                    Console.Write(map[a, b]);
                }
                Console.WriteLine();
            }
                


            SW.Stop();

            //Console.WriteLine("Part 2: {0}, Execution Time: {1}", result2, StopwatchUtil.getInstance().GetTimestamp(SW));


        }

        private void ParseDisplayInstructions(string[] instructions)
        {
            foreach (string instruction in instructions)
            {
                if (instruction.Contains("rect"))
                {
                    Rectangle(instruction);
                }
                else if (instruction.Contains("rotate row"))
                {
                    RotateRow(instruction);
                }
                else if (instruction.Contains("rotate column"))
                {
                    RotateColumn(instruction);
                }
            }
        }

        private void Rectangle(string instruction)
        {
            string grid = instruction.Replace("rect ", "");

            var values = grid.Split("x").Select(int.Parse).ToArray();

            for (int a = 0; a < values[0]; a++)
                for (int b = 0; b < values[1]; b++)
                    map[b, a] = '#';
        }

        private void RotateRow(string instruction)
        {
            string rotate = instruction.Replace("rotate row y=", "");

            var values = rotate.Split(" by ");
            var row = int.Parse(values[0]);
            var shift = int.Parse(values[1]);

            int rowLength = map.GetLength(1);       //get length of the row

            var hashLocations = Enumerable.Range(0, rowLength)
                                                        .Where(colIndex => map[row, colIndex] == '#')
                                                        .Select(colIndex => new Tuple<int, int>(row, colIndex))
                                                        .ToList();

            //clear row 
            for (int col = 0; col < rowLength; col++)
            {
                map[row, col] = ' ';
            }

            foreach (var hashLocation in hashLocations)
            {
                int newPos = (hashLocation.Item2 + shift) % rowLength;
                map[row, newPos] = '#';
            }
        }

        private void RotateColumn(string instruction)
        {
            string rotate = instruction.Replace("rotate column x=", "");

            var values = rotate.Split(" by ");
            var column = int.Parse(values[0]);
            var shift = int.Parse(values[1]);

            int columnHeight = map.GetLength(0);       //get height of column

            var hashLocations = Enumerable.Range(0, columnHeight)
                .Where(rowIndex => map[rowIndex, column] == '#')
                .Select(rowIndex => new Tuple<int, int>(rowIndex, column))
                .ToList();


            //clear column 
            for (int row = 0; row < columnHeight; row++)
            {
                map[row, column] = ' ';
            }

            foreach (var hashLocation in hashLocations)
            {
                int newPos = (hashLocation.Item1 + shift) % columnHeight;
                map[newPos, column] = '#';
            }
        }
    }
}
