using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2019, Day = 1)]
    public class Year2019Day1Problem: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2019Day1Problem()
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
            Console.WriteLine("==========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("==========================================");

            //Build BasePath and retrieve input. 
            string filename = _OverrideFile ?? path;

          
            Console.WriteLine("");
            Console.WriteLine("==========================================");
        }
    }
}
