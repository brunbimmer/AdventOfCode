using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 4)]
    public class Year2020Day04 : IAdventOfCode
    {
        private static readonly string[] RequiredFields = { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day04()
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
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            int validPassports = Part1(lines);
            _SW.Stop();

            output.WritePart(1, validPassports, trackTime ? _SW.Elapsed : null, "Number of valid passports");


            _SW.Restart();
            validPassports = Part2(lines);
            _SW.Stop();

            output.WritePart(2, validPassports, trackTime ? _SW.Elapsed : null, "Number of valid passports");

        }

        private int Part1(string[] lines)
        {
            return ParsePassports(lines).Count(passport => RequiredFields.All(passport.ContainsKey));
        }

        private int Part2(string[] lines)
        {
            return ParsePassports(lines).Count(passport => RequiredFields.All(field =>
                passport.TryGetValue(field, out string value) && IsValidField(field, value)));
        }

        private IEnumerable<Dictionary<string, string>> ParsePassports(string[] lines)
        {
            Dictionary<string, string> passport = new Dictionary<string, string>();

            // A final separator ensures the last passport is checked at end of file.
            foreach (string line in lines.Append(string.Empty))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    yield return passport;
                    passport = new Dictionary<string, string>();
                    continue;
                }

                foreach (string field in line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] parts = field.Split(':', 2);
                    passport[parts[0]] = parts[1];
                }
            }

        }

        private bool IsValidField(string field, string value)
        {
            return field switch
            {
                "byr" => value.Length == 4 && IsNumberInRange(value, 1920, 2002),
                "iyr" => value.Length == 4 && IsNumberInRange(value, 2010, 2020),
                "eyr" => value.Length == 4 && IsNumberInRange(value, 2020, 2030),
                "hgt" => (value.EndsWith("cm", StringComparison.Ordinal) && IsNumberInRange(value[..^2], 150, 193))
                    || (value.EndsWith("in", StringComparison.Ordinal) && IsNumberInRange(value[..^2], 59, 76)),
                "hcl" => Regex.IsMatch(value, @"\A#[0-9a-f]{6}\z"),
                "ecl" => value is "amb" or "blu" or "brn" or "gry" or "grn" or "hzl" or "oth",
                "pid" => Regex.IsMatch(value, @"\A[0-9]{9}\z"),
                _ => true
            };
        }

        private bool IsNumberInRange(string value, int minimum, int maximum)
        {
            return value.All(char.IsAsciiDigit) && int.TryParse(value, out int number)
                && number >= minimum && number <= maximum;
        }
    }
}
