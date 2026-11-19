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

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            //Build BasePath and retrieve input. 
 
            string inputString = "00111101111101000";
            int size = 272;
            int sizePart2 = 35651584;

            _SW.Start();                       

            string checkSum = GetCheckSum(inputString, size);

            _SW.Stop();

            output.WritePart(1, checkSum, trackTime ? _SW.Elapsed : null, "Checksum Result");

            _SW.Restart();

            string checkSumPart2 = GetCheckSum(inputString, sizePart2);
            
            _SW.Stop();

            output.WritePart(2, checkSumPart2, trackTime ? _SW.Elapsed : null, "Checksum Result");


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
