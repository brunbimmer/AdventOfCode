using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2025, Day = 2)]
    public class Year2025Day02: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day02()
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

            string input  = FileIOHelper.getInstance().ReadDataAsString(file);

            _SW.Start();

            var (part1, part2) = FindInvalidIDs(input);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Sum of Invalid IDs: {part1}");
            Console.WriteLine($"  Part 2: Sum of Invalid IDs (Repeated Pattern): {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
        }

        (long part1, long part2) FindInvalidIDs(string input)
        {
            long sum1 = 0;
            long sum2 = 0;
            
            // Input is a single long line with all ranges
            string[] ranges = input.Split(',');
            
            foreach (string range in ranges)
            {
                string[] parts = range.Trim().Split('-');
                long start = long.Parse(parts[0]);
                long end = long.Parse(parts[1]);
                
                for (long id = start; id <= end; id++)
                {
                    if (IsInvalidIDPart1(id))
                    {
                        sum1 += id;
                    }
                    
                    if (IsInvalidIDPart2(id))
                    {
                        sum2 += id;
                    }
                }
            }
            
            return (sum1, sum2);
        }

        bool IsInvalidIDPart1(long id)
        {
            string idStr = id.ToString();
            int len = idStr.Length;
            
            // Invalid ID must be made of a pattern repeated exactly twice
            // So length must be even
            if (len % 2 != 0)
                return false;
            
            int halfLen = len / 2;
            string firstHalf = idStr.Substring(0, halfLen);
            string secondHalf = idStr.Substring(halfLen);
            
            return firstHalf == secondHalf;
        }

        bool IsInvalidIDPart2(long id)
        {
            string idStr = id.ToString();
            int len = idStr.Length;
            
            // Invalid ID has a sequence of digits repeated at least twice
            // Try all possible pattern lengths from 1 to len/2
            for (int patternLen = 1; patternLen <= len / 2; patternLen++)
            {
                // Check if this pattern length divides evenly into the ID length
                if (len % patternLen == 0)
                {
                    string pattern = idStr.Substring(0, patternLen);
                    bool isValid = true;
                    
                    // Check if the entire ID is the pattern repeated
                    for (int i = 0; i < len; i += patternLen)
                    {
                        string chunk = idStr.Substring(i, patternLen);
                        if (chunk != pattern)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    
                    // If we found a repeating pattern of at least 2 repetitions
                    if (isValid && len >= 2 * patternLen)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}
