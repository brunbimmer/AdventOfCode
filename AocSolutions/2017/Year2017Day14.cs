using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using CommonAlgorithms;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 14)]
    public class Year2017Day14 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day14()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string key = FileIOHelper.getInstance().ReadDataAsString(file).Trim();
            _SW.Restart();
            bool[,] grid = BuildGrid(key);
            int used = grid.Cast<bool>().Count(value => value);
            _SW.Stop();
            output.WritePart(1, used, trackTime ? _SW.Elapsed : null, "Used squares in the disk grid");
            _SW.Restart();
            int regions = CountRegions(grid);
            _SW.Stop();
            output.WritePart(2, regions, trackTime ? _SW.Elapsed : null, "Connected regions of used squares");
        }

        private bool[,] BuildGrid(string key)
        {
            var grid = new bool[128, 128];
            for (int row = 0; row < 128; row++)
            {
                byte[] hash = KnotHash.Compute($"{key}-{row}");
                for (int col = 0; col < 128; col++)
                    grid[row, col] = (hash[col / 8] & (128 >> (col % 8))) != 0;
            }
            return grid;
        }

        private int CountRegions(bool[,] original)
        {
            var grid = (bool[,])original.Clone();
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            var directions = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
            int regions = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (!grid[row, col]) continue;
                    regions++;
                    grid[row, col] = false;
                    var pending = new Queue<(int Row, int Col)>();
                    pending.Enqueue((row, col));
                    while (pending.Count > 0)
                    {
                        var cell = pending.Dequeue();
                        foreach (var direction in directions)
                        {
                            int nextRow = cell.Row + direction.Item1;
                            int nextCol = cell.Col + direction.Item2;
                            if (nextRow >= 0 && nextRow < rows && nextCol >= 0 && nextCol < cols && grid[nextRow, nextCol])
                            {
                                grid[nextRow, nextCol] = false;
                                pending.Enqueue((nextRow, nextCol));
                            }
                        }
                    }
                }
            }
            return regions;
        }
    }
}
