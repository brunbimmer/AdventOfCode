using System;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 15)]
    public class Year2020Day15 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day15()
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
            int[] startingNumbers = FileIOHelper.getInstance().ReadDataAsString(file)
                .Split(',').Select(int.Parse).ToArray();

            _SW.Restart();
            int number = Part1(startingNumbers);
            _SW.Stop();

            output.WritePart(1, number, trackTime ? _SW.Elapsed : null, "Number spoken on turn 2020");


            _SW.Restart();
            number = Part2(startingNumbers);
            _SW.Stop();

            output.WritePart(2, number, trackTime ? _SW.Elapsed : null, "Number spoken on turn 30000000");

        }

        private int Part1(int[] startingNumbers, int targetTurn = 2020)
        {
            return PlayGame(startingNumbers, targetTurn);
        }

        private int Part2(int[] startingNumbers)
        {
            return PlayGame(startingNumbers, 30000000);
        }

        private int PlayGame(int[] startingNumbers, int targetTurn)
        {
            if (startingNumbers.Length == 0)
                throw new ArgumentException("At least one starting number is required.", nameof(startingNumbers));
            if (targetTurn < 1)
                throw new ArgumentOutOfRangeException(nameof(targetTurn));
            if (targetTurn <= startingNumbers.Length)
                return startingNumbers[targetTurn - 1];

            // Generated numbers are smaller than the target turn, so they fit this table.
            int[] lastSpoken = new int[targetTurn];
            int number = startingNumbers[^1];
            int previousTurn = 0;
            for (int index = 0; index < startingNumbers.Length - 1; index++)
            {
                int startingNumber = startingNumbers[index];
                if (startingNumber == number)
                    previousTurn = index + 1;

                if (startingNumber >= 0 && startingNumber < lastSpoken.Length)
                    lastSpoken[startingNumber] = index + 1;
            }

            // Handle the final seed separately, including seeds outside the table's range.
            if (number >= 0 && number < lastSpoken.Length)
                lastSpoken[number] = startingNumbers.Length;
            number = previousTurn == 0 ? 0 : startingNumbers.Length - previousTurn;

            for (int turn = startingNumbers.Length + 1; turn < targetTurn; turn++)
            {
                previousTurn = lastSpoken[number];
                lastSpoken[number] = turn;
                number = previousTurn == 0 ? 0 : turn - previousTurn;
            }

            return number;
        }
    }
}
