using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 1)]
    public class Year2023Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2023Day01()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
           
            _SW.Start();


            int sum = CalculateSum(lines);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Calibration Value of input: {sum}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            int trueSum = CalculateSumWithWords(lines);
            
            _SW.Stop();

            Console.WriteLine($"  Part 2: True calibration value: {trueSum}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        public int CalculateSum(string[] lines)
        {
            int sum = 0;
            foreach (string line in lines)
            {
                var digits = line.Where(char.IsDigit).ToArray();

                if (digits.Length > 0)
                {
                    char firstDigit = digits.First();
                    char lastDigit = digits.Last();
                    sum += Convert.ToInt32($"{firstDigit}{lastDigit}");
                }
            }
            return sum;
        }

        public int CalculateSumWithWords(string[] lines)
        {
            string pattern = @"(?=(one|two|three|four|five|six|seven|eight|nine))";
            int sum = 0;

            foreach (string line in lines)
            {
                var digits = line.Where(char.IsDigit).ToArray();

                char firstDigit;
                char lastDigit;

                if (digits.Length > 0)
                {
                    firstDigit = digits.First();
                    lastDigit = digits.Last();
                }
                else
                {
                    firstDigit = lastDigit = ' ';
                }

                MatchCollection matches = Regex.Matches(line, pattern);

                if (matches.Count > 0)
                {

                    int firstDigitPosition = line.IndexOfAny("0123456789".ToCharArray());
                    int lastDigitPosition = line.LastIndexOfAny("0123456789".ToCharArray());


                    if (matches.Count == 1)
                    {
                        string word = matches[0].Groups[1].Value;

                        int wordPosition = line.IndexOf(word, StringComparison.OrdinalIgnoreCase);

                        if (firstDigitPosition == -1 && lastDigitPosition == -1)
                            sum += Convert.ToInt32($"{GetValue(word)}{GetValue(word)}");
                        else if (wordPosition < firstDigitPosition)
                            sum += Convert.ToInt32($"{GetValue(word)}{lastDigit}");
                        else if (wordPosition > lastDigitPosition)
                            sum += Convert.ToInt32($"{firstDigit}{GetValue(word)}");
                        else
                            sum += Convert.ToInt32($"{firstDigit}{lastDigit}");
                    }
                    else
                    {
                        string firstWord = matches.First().Groups[1].Value;
                        string lastWord = matches.Last().Groups[1].Value;

                        int firstWordPosition = line.IndexOf(firstWord, StringComparison.OrdinalIgnoreCase);
                        int lastWordPosition = line.LastIndexOf(lastWord, StringComparison.OrdinalIgnoreCase);

                        if (   (firstDigitPosition == -1 && lastDigitPosition == -1)
                            || (firstWordPosition < firstDigitPosition && lastWordPosition > lastDigitPosition))
                            sum += Convert.ToInt32($"{GetValue(firstWord)}{GetValue(lastWord)}");
                        else if (firstWordPosition < firstDigitPosition && lastDigitPosition > lastWordPosition)
                            sum += Convert.ToInt32($"{GetValue(firstWord)}{lastDigit}");
                        else if (firstDigitPosition < firstWordPosition && lastWordPosition > lastDigitPosition)
                            sum += Convert.ToInt32($"{firstDigit}{GetValue(lastWord)}");
                        else
                            sum += Convert.ToInt32($"{firstDigit}{lastDigit}");
                    }
                }
                else
                {
                    sum += Convert.ToInt32($"{firstDigit}{lastDigit}");
                }
            }
            return sum;
        }

        public int GetValue(string input)
        {
            return input switch
            {
                "one" => 1,
                "two" => 2,
                "three" => 3,
                "four" => 4,
                "five" => 5,
                "six" => 6,
                "seven" => 7,
                "eight" => 8,
                "nine" => 9,
                _ => 0,
            };
        }
    }
}
