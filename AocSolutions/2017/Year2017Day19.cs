using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using System.Text;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 19)]
    public class Year2017Day19 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day19()
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
            _SW.Restart();
            var result = Follow(FileIOHelper.getInstance().ReadDataAsLines(file));
            _SW.Stop();
            output.WritePart(1, result.Letters, trackTime ? _SW.Elapsed : null, "Letters collected along the tube path");
            output.WritePart(2, result.Steps, description: "Steps taken along the tube path");
        }

        private (string Letters, int Steps) Follow(string[] diagram)
        {
            if (diagram.Length == 0) throw new FormatException("The routing diagram is empty.");
            int col = diagram[0].IndexOf('|');
            if (col < 0) throw new FormatException("The top row must contain a tube entrance.");
            char Cell(int row, int column) => row >= 0 && row < diagram.Length && column >= 0 && column < diagram[row].Length
                ? diagram[row][column] : ' ';
            int row = 0;
            int dr = 1;
            int dc = 0;
            int steps = 0;
            var letters = new StringBuilder();
            while (Cell(row, col) != ' ')
            {
                char cell = Cell(row, col);
                steps++;
                if (char.IsLetter(cell)) letters.Append(cell);
                if (cell == '+')
                {
                    var candidates = dr == 0 ? new[] { (-1, 0), (1, 0) } : new[] { (0, -1), (0, 1) };
                    var turns = candidates.Where(direction => Cell(row + direction.Item1, col + direction.Item2) != ' ').ToArray();
                    if (turns.Length != 1) throw new FormatException("A junction must have exactly one onward turn.");
                    (dr, dc) = turns[0];
                }
                row += dr;
                col += dc;
            }
            return (letters.ToString(), steps);
        }
    }
}
