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
    [AdventOfCode(Year = 2021, Day = 6)]
    public class Year2021Day6 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2021Day6()
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

            string input = FileIOHelper.getInstance().ReadDataAsString(file);

            long[] fishInput = Array.ConvertAll(input.Split(','), s => long.Parse(s));

            _SW.Start();            
            long fish = CountFish(fishInput, 80);
            _SW.Stop();



            Console.WriteLine("Part 1: Number of Fish after 50 days: {0}", fish);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long fish2 = CountFish(fishInput, 256);           
            _SW.Stop();

            Console.WriteLine("Part 2: Number of Fish after 256 days: {0}", fish2);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }        

        private long CountFish(long[] fishInput, int days)
        {
            long[] fishCounter = new long[9];

            for(int i = 0; i <=8; i++)
                fishCounter[i] = fishInput.Count(x => x == i);

            for(int i = 0; i < days; i++) {
                long newFish = fishCounter[0];

                for(int d = 0; d < 8; d++) {
                    fishCounter[d] = fishCounter[d + 1];
                }
                
                fishCounter[6] += newFish;
                fishCounter[8] = newFish;
            }
            
            return fishCounter.Sum();
        }
    }
}
