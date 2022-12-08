using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 2)]
    public class Year2015Day2Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day2Problem()
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
            string[] line = FileIOHelper.getInstance().ReadDataAsLines(file);

            //Part 1

            SW.Start();

            int wrappingPaperArea = 0;
            int ribbonLength = 0;

            foreach (int i in Enumerable.Range(0, line.Length))
            {
                var numbers = line[i].Split('x').Select(Int32.Parse).ToArray();
                int l = numbers[0];
                int w = numbers[1];
                int h = numbers[2];


                var extra = numbers.OrderBy(x => x).Take(2).ToArray();    //take the smallest dimension

                wrappingPaperArea += 2 * l * w + 2 * w * h + 2 * h * l + extra[0] * extra[1];

                ribbonLength += (2 * extra[0] + 2 * extra[1]) + l * w * h;
            }

            SW.Stop();

            Console.WriteLine("  Part 1: Required wrapping paper: {0}", wrappingPaperArea);
            Console.WriteLine("  Part 2: Total Ribben Length: {0}", ribbonLength);
            Console.WriteLine("   Execution Time: {0} ms", SW.ElapsedMilliseconds);

        }
    }
}
