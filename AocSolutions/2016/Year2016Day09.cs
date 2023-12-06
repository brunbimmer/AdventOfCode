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
using MoreLinq.Extensions;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 9)]
    public class Year2016Day09 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2016Day09()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string data = FileIOHelper.getInstance().ReadDataAsString(file);

            SW.Start();

            int length = DecompressInput(data);

            
            SW.Stop();

            Console.WriteLine("Part 1: Decompressed Length {0}, Execution Time: {1}", length, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            long newLength = CalculateDecompressedLength(data.Trim());

            SW.Stop();

            Console.WriteLine("Part 2: New Decompressed Length {0}, Execution Time: {1}", newLength, StopwatchUtil.getInstance().GetTimestamp(SW));


        }

        int DecompressInput(string data)
        {
            string currentString = data;

            int markerPos = 0;

            while (markerPos < currentString.Length - 1)
            {
                try
                {

                    int markerIndex = currentString.IndexOf('(', markerPos);
                    int markerEnd = currentString.IndexOf(')', markerIndex);


                    string marker = currentString.Substring(markerPos + 1, markerEnd - (markerPos + 1));
                    int chCount = int.Parse(marker.Split("x")[0]);
                    int repeat = int.Parse(marker.Split("x")[1]);

                    string strToRepeat = currentString.Substring(markerEnd + 1, chCount);

                    var result = strToRepeat.Repeat(repeat);

                    string newSequence = string.Join("", result);

                    string _temp = currentString.Remove(markerPos, marker.Length + 2 + chCount);
                    string _newString = _temp.Insert(markerPos, newSequence);

                    markerPos = markerPos + newSequence.Length;

                    currentString = _newString;
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            }

            return currentString.Trim().Length;
        }

        long CalculateDecompressedLength(string data)
        {
            if (!data.Contains('('))
                return data.Length;
            long fullcount = 0;
            int i = 0;
            while (i < data.Length)
            {
                if (data[i] != '(') { i++; fullcount++; continue; }

                int length = Convert.ToInt32(data.Substring(i + 1, data.IndexOf('x', i) - i - 1));
                int count = Convert.ToInt32(data.Substring(data.IndexOf('x', i) + 1, data.IndexOf(')', i) - data.IndexOf('x', i) - 1));
                int clength = 3 + count.ToString().Length + length.ToString().Length;
                string part = data.Substring(i + clength, length);
                fullcount += CalculateDecompressedLength(part) * count;
                i += clength + length;
            }
            return fullcount;
        }
    }
}
