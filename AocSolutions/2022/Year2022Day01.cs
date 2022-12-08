using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 1)]
    public class Year2022Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2022Day01()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
           
            SW.Start();
            
            ParseInput(lines);

            int maxElfSum = calorieCollection.Max(x => x.Sum());

            SW.Stop();

            Console.WriteLine("  Part 1: Total calories carried by an elf: {0}", maxElfSum);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));
            
            SW.Restart();

            var orderedSumList = calorieCollection.OrderByDescending(x => x.Sum()).ToList().Take(3).ToList();

            int topThreeElfSum = orderedSumList.Sum(x => x.Sum());
            
            SW.Stop();

            Console.WriteLine("  Part 1: Total calories carried by three elves: {0}", topThreeElfSum);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


        }

        private void ParseInput(string[] data)
        {
            List<int> calorieInput = new List<int>();
            foreach(string line in data)
            {
                if (line.Trim() == "")                
                {
                    calorieCollection.Add(calorieInput);
                    calorieInput = new List<int>();
                }
                else
                {
                    calorieInput.Add(Convert.ToInt32(line));
                }
            }
        }
    }
}
