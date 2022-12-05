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
    [AdventOfCode(Year = 2022, Day = 02)]
    public class Year2022Day02 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2022Day02()
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

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            int totalScore = 0;            
            foreach (string instruction in instructions)
            {
                string[] play = instruction.Split(" ");
                totalScore += PlayRound1(play);
            }
            
            SW.Stop();

            Console.WriteLine("Part 1: Total Score for all rounds {0}, Execution Time: {1}", totalScore, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();
            totalScore = 0;

            foreach (string instruction in instructions)
            {
                string[] play = instruction.Split(" ");
                totalScore += PlayRound2(play);                
            }           
            
            SW.Stop();

            Console.WriteLine("Part 2: Total Score after all rounds {0}, Execution Time: {1}", totalScore, StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("\n===========================================\n");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }       

        int PlayRound1(string[] play)
        {
            if (play[1] == "X" && play[0] == "C")
                return 6 + 1;
            
            if (play[1] == "X" && play[0] == "B")
                return 0 + 1;

            if (play[1] == "X" && play[0] == "A")
                return 3 + 1;

            if (play[1] == "Y" && play[0] == "C")
                return 0 + 2;
            
            if (play[1] == "Y" && play[0] == "B")
                return 3 + 2;

            if (play[1] == "Y" && play[0] == "A")
                return 6 + 2;

            if (play[1] == "Z" && play[0] == "C")
                return 3 + 3;
            
            if (play[1] == "Z" && play[0] == "B")
                return 6 + 3;

            if (play[1] == "Z" && play[0] == "A")
                return 0 + 3;

            return 0;
        }

        int PlayRound2(string[] play)
        {
            if (play[1] == "X" && play[0] == "C")
                return 0 + 2;

            if (play[1] == "X" && play[0] == "B")
                return 0 + 1;

            if (play[1] == "X" && play[0] == "A")
                return 0 + 3;

            if (play[1] == "Y" && play[0] == "C")
                return 3 + 3;
            
            if (play[1] == "Y" && play[0] == "B")
                return 3 + 2;

            if (play[1] == "Y" && play[0] == "A")
                return 3 + 1;

            if (play[1] == "Z" && play[0] == "C")
                return 6 + 1;
            
            if (play[1] == "Z" && play[0] == "B")
                return 6 + 3;

            if (play[1] == "Z" && play[0] == "A")
                return 6 + 2;

            return 0;
        }

    }
}
