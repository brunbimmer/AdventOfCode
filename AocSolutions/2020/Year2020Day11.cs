using System;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 11)]
    public class Year2020Day11 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day11()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] layout = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            int occupied = Part1(layout);
            _SW.Stop();

            output.WritePart(1, occupied, trackTime ? _SW.Elapsed : null, "Occupied seats after stabilization");


            _SW.Restart();
            occupied = Part2(layout);
            _SW.Stop();

            output.WritePart(2, occupied, trackTime ? _SW.Elapsed : null, "Occupied seats after stabilization");

        }

        private int Part1(string[] layout)
        {
            return Simulate(layout, useVisibility: false, occupiedThreshold: 4);
        }

        private int Part2(string[] layout)
        {
            return Simulate(layout, useVisibility: true, occupiedThreshold: 5);
        }

        private int Simulate(string[] layout, bool useVisibility, int occupiedThreshold)
        {
            char[][] current = layout.Select(row => row.ToCharArray()).ToArray();
            char[][] next = layout.Select(row => new char[row.Length]).ToArray();

            while (true)
            {
                bool changed = false;
                int occupied = 0;

                for (int row = 0; row < current.Length; row++)
                {
                    for (int column = 0; column < current[row].Length; column++)
                    {
                        char seat = current[row][column];
                        if (seat == '.')
                        {
                            next[row][column] = '.';
                            continue;
                        }

                        int neighbors = CountOccupiedNeighbors(current, row, column, useVisibility);
                        char nextSeat = seat;
                        if (seat == 'L' && neighbors == 0)
                            nextSeat = '#';
                        else if (seat == '#' && neighbors >= occupiedThreshold)
                            nextSeat = 'L';

                        next[row][column] = nextSeat;
                        changed |= nextSeat != seat;
                        if (nextSeat == '#')
                            occupied++;
                    }
                }

                if (!changed)
                    return occupied;

                (current, next) = (next, current);
            }
        }

        private int CountOccupiedNeighbors(char[][] layout, int row, int column, bool useVisibility)
        {
            int occupied = 0;
            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
                {
                    if (rowOffset == 0 && columnOffset == 0)
                        continue;

                    int neighborRow = row + rowOffset;
                    int neighborColumn = column + columnOffset;
                    while (neighborRow >= 0 && neighborRow < layout.Length
                        && neighborColumn >= 0 && neighborColumn < layout[neighborRow].Length)
                    {
                        char seat = layout[neighborRow][neighborColumn];
                        if (seat == '#')
                            occupied++;

                        // Both empty and occupied seats block the view beyond them.
                        if (!useVisibility || seat != '.')
                            break;

                        neighborRow += rowOffset;
                        neighborColumn += columnOffset;
                    }
                }
            }

            return occupied;
        }
    }
}
