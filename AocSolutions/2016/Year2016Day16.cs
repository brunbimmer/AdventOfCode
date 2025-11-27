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
    [AdventOfCode(Year = 2016, Day = 16)]
    public class Year2016Day16 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day16()
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
 
            string inputString = "00111101111101000";
            int size = 272;
            int sizePart2 = 35651584;

            _SW.Start();                       

            string checkSum = GetCheckSum(inputString, size);

            _SW.Stop();

            Console.WriteLine("Part 1 - Checksum Result: {0}, Execution Time: {1}", checkSum, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            string checkSumPart2 = GetCheckSum(inputString, sizePart2);
            
            _SW.Stop();

            Console.WriteLine("Part 2 - Checksum Result: {0}, Execution Time: {1}", checkSumPart2, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }    

        string GetCheckSum(string input, int size)
        {
            StringBuilder sb = new StringBuilder(input);

            while (sb.Length < size)
            {
                StringBuilder bRev = new StringBuilder(sb.Length + 1);
                bRev.Append('0');
                for (int i = sb.Length - 1; i >= 0; i--)
                {
                    bRev.Append(sb[i] == '0' ? '1' : '0');
                }
                sb.Append(bRev.ToString());
            }

            // Trim to size
            sb.Length = size;

            // Compute checksum
            StringBuilder checksum = new StringBuilder();
            while (sb.Length % 2 == 0)
            {
                checksum.Clear();
                for (int i = 0; i < sb.Length; i += 2)
                {
                    checksum.Append(sb[i] == sb[i + 1] ? '1' : '0');
                }
                sb = new StringBuilder(checksum.ToString());
            }

            return sb.ToString();
        }
    }
}
