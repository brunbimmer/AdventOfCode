using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 25)]
    public class Year2022Day25 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day25()
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

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            var input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();
            
            // Convert all SNAFU numbers to decimal and sum them
            long totalDecimal = input.Sum(line => SnafuToDecimal(line));
            
            // Convert the sum back to SNAFU
            string resultSnafu = DecimalToSnafu(totalDecimal);

            _SW.Stop();

            output.WritePart(1, resultSnafu, trackTime ? _SW.Elapsed : null, "SNAFU number for fuel");


            _SW.Restart();

            // Part 2 is just pressing a big red button, no additional computation needed
            string part2 = "Press the big red button!";

            _SW.Stop();

            output.WritePart(2, part2, trackTime ? _SW.Elapsed : null, "");

        }

        /// <summary>
        /// Convert a SNAFU number string to a decimal long
        /// SNAFU digits: 2=2, 1=1, 0=0, -=−1, ==−2
        /// </summary>
        private long SnafuToDecimal(string snafu)
        {
            long result = 0;
            long power = 1;
            
            // Process from right to left
            for (int i = snafu.Length - 1; i >= 0; i--)
            {
                char digit = snafu[i];
                long value = digit switch
                {
                    '2' => 2,
                    '1' => 1,
                    '0' => 0,
                    '-' => -1,
                    '=' => -2,
                    _ => throw new ArgumentException($"Invalid SNAFU digit: {digit}")
                };
                
                result += value * power;
                power *= 5;
            }
            
            return result;
        }

        /// <summary>
        /// Convert a decimal number to SNAFU representation
        /// Uses a greedy algorithm working from least significant digit
        /// </summary>
        private string DecimalToSnafu(long decimal_number)
        {
            var result = new StringBuilder();
            
            while (decimal_number > 0)
            {
                long remainder = decimal_number % 5;
                decimal_number /= 5;
                
                switch (remainder)
                {
                    case 0:
                        result.Insert(0, '0');
                        break;
                    case 1:
                        result.Insert(0, '1');
                        break;
                    case 2:
                        result.Insert(0, '2');
                        break;
                    case 3:
                        // 3 = 5 - 2, so represent as = and carry 1
                        result.Insert(0, '=');
                        decimal_number++;
                        break;
                    case 4:
                        // 4 = 5 - 1, so represent as - and carry 1
                        result.Insert(0, '-');
                        decimal_number++;
                        break;
                }
            }
            
            return result.Length == 0 ? "0" : result.ToString();
        }
    }
}
