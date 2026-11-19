using System;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 5)]
    public class Year2020Day05 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day05()
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
            string[] boardingPasses = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            int highestSeatId = Part1(boardingPasses);
            _SW.Stop();

            output.WritePart(1, highestSeatId, trackTime ? _SW.Elapsed : null, "Highest seat ID");


            _SW.Restart();
            int missingSeatId = Part2(boardingPasses);
            _SW.Stop();

            output.WritePart(2, missingSeatId, trackTime ? _SW.Elapsed : null, "Your seat ID");

        }

        private int Part1(string[] boardingPasses)
        {
            return boardingPasses.Select(GetSeatId).Max();
        }

        private int Part2(string[] boardingPasses)
        {
            bool[] occupiedSeats = new bool[128 * 8];
            foreach (string boardingPass in boardingPasses)
                occupiedSeats[GetSeatId(boardingPass)] = true;

            for (int seatId = 1; seatId < occupiedSeats.Length - 1; seatId++)
            {
                if (!occupiedSeats[seatId] && occupiedSeats[seatId - 1] && occupiedSeats[seatId + 1])
                    return seatId;
            }

            throw new InvalidOperationException("No missing seat with both neighboring seats occupied was found.");
        }

        private int GetSeatId(string boardingPass)
        {
            int seatId = 0;

            // The seven row bits followed by three column bits encode row * 8 + column.
            foreach (char character in boardingPass)
            {
                seatId <<= 1;
                if (character is 'B' or 'R')
                    seatId |= 1;
            }

            return seatId;
        }
    }
}
