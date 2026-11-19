using System;
using System.Diagnostics;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 02)]
    public class Year2020Day02 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day02()
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
            string[] entries = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            int validPasswords = Part1(entries);
            _SW.Stop();

            output.WritePart(1, validPasswords, trackTime ? _SW.Elapsed : null, "Number of valid passwords");


            _SW.Restart();
            validPasswords = Part2(entries);
            _SW.Stop();

            output.WritePart(2, validPasswords, trackTime ? _SW.Elapsed : null, "Number of valid passwords");

        }

        private int Part1(string[] entries)
        {
            int validPasswords = 0;

            foreach (string entry in entries)
            {
                var (minimum, maximum, letter, password) = ParseEntry(entry);

                int occurrences = 0;
                foreach (char character in password)
                {
                    if (character == letter)
                        occurrences++;
                }

                if (occurrences >= minimum && occurrences <= maximum)
                    validPasswords++;
            }

            return validPasswords;
        }

        private int Part2(string[] entries)
        {
            int validPasswords = 0;

            foreach (string entry in entries)
            {
                var (firstPosition, secondPosition, letter, password) = ParseEntry(entry);
                bool firstMatches = firstPosition > 0 && firstPosition <= password.Length
                    && password[firstPosition - 1] == letter;
                bool secondMatches = secondPosition > 0 && secondPosition <= password.Length
                    && password[secondPosition - 1] == letter;

                if (firstMatches ^ secondMatches)
                    validPasswords++;
            }

            return validPasswords;
        }

        private (int First, int Second, char Letter, string Password) ParseEntry(string entry)
        {
            string[] parts = entry.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] bounds = parts[0].Split('-');
            return (int.Parse(bounds[0]), int.Parse(bounds[1]), parts[1][0], parts[2]);
        }
    }
}
