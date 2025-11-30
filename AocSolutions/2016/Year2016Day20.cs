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
    [AdventOfCode(Year = 2016, Day = 20)]
    public class Year2016Day20 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day20()
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

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       


            int result1 = FindLowestAllowedIP(input);

            
            _SW.Stop();

            Console.WriteLine("Part 1 - Lowest Allowed IP: {0}, Execution Time: {1}", result1, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            int totalAllowed = FindAllAllowedIPs(input);
            
            _SW.Stop();

            Console.WriteLine("Part 2 - Total Allowed IPs: {0}, Execution Time: {1}", totalAllowed, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }      

        int FindLowestAllowedIP(string[] input)
        {
            List<(long, long)> blockedRanges = new List<(long, long)>();

            foreach (string line in input)
            {
                string[] parts = line.Split('-');
                long start = long.Parse(parts[0]);
                long end = long.Parse(parts[1]);
                blockedRanges.Add((start, end));
            }

            blockedRanges = blockedRanges.OrderBy(r => r.Item1).ToList();

            long currentIP = 0;

            foreach (var range in blockedRanges)
            {
                if (currentIP < range.Item1)
                {
                    // Found a gap
                    return (int)currentIP;
                }
                else if (currentIP <= range.Item2)
                {
                    // Move currentIP to the end of the blocked range
                    currentIP = range.Item2 + 1;
                }
            }

            return (int)currentIP;
        } 

        int FindAllAllowedIPs(string[] input)
        {
            List<(long, long)> blockedRanges = new List<(long, long)>();

            foreach (string line in input)
            {
                string[] parts = line.Split('-');
                long start = long.Parse(parts[0]);
                long end = long.Parse(parts[1]);
                blockedRanges.Add((start, end));
            }

            blockedRanges = blockedRanges.OrderBy(r => r.Item1).ToList();

            long currentIP = 0;
            long allowedCount = 0;

            foreach (var range in blockedRanges)
            {
                if (currentIP < range.Item1)
                {
                    // Count allowed IPs in the gap
                    allowedCount += range.Item1 - currentIP;
                    currentIP = range.Item2 + 1;
                }
                else if (currentIP <= range.Item2)
                {
                    // Move currentIP to the end of the blocked range
                    currentIP = range.Item2 + 1;
                }
            }

            // Count any remaining allowed IPs up to 4294967295
            if (currentIP <= 4294967295)
            {
                allowedCount += 4294967295 - currentIP + 1;
            }

            return (int)allowedCount;
        }
    }
}
