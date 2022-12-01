using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 1)]
    public class Year2015Day1Problem: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day1Problem()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute) Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        

        public virtual void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string line = FileIOHelper.getInstance().ReadDataAsString(file);

            //Part 1

            SW.Start();

            int countUp = CommonAlgorithms.StringUtils.CountCharacters(line, '(');
            int countDown = CommonAlgorithms.StringUtils.CountCharacters(line, ')');

            SW.Stop();

            Console.WriteLine("  Part 1: What floor is Santa on: {0}", countUp - countDown);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));
            //Part 2
            SW.Restart();
            SW.Start();

            int floor = 0;
            int pos = 1;
            foreach (char c in line.ToCharArray())
            {
                if (c == '(')
                    floor++;
                else
                    floor--;

                if (floor == -1) break;

                pos++;
            }
            SW.Stop();

            Console.WriteLine("  Part 2: Position of character when Santa enters basement: {0}", pos);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));
            Console.WriteLine("\n===========================================\n");
        }
    }
}
