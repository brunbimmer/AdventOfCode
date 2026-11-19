using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using CommonAlgorithms;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 8)]
    public class Year2015Day8Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }



        public Year2015Day8Problem()
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

            //no need to retrieve input from AOC. The input is a simple string
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            var words = FileIOHelper.getInstance().ReadDataAsLines(file);

         
            _SW.Start();
          
            
            //simple regular expression replacements will accurately and conveniently provide expected results.
            int value = words.Sum(w => w.Length - Regex.Replace(w.Trim('"').Replace("\\\"", "A").Replace("\\\\", "B"), "\\\\x[a-f0-9]{2}", "C").Length);


            _SW.Stop();

            output.WritePart(1, value, trackTime ? _SW.Elapsed : null, "Difference between string literals and # of actual characters");


            _SW.Restart();


            //simple regular expressions to simulate encoding without performing the actual encoding.

            int newValue = words.Sum(w => w.Replace("\\", "AA").Replace("\"", "BB").Length + 2 - w.Length);

            output.WritePart(2, newValue, trackTime ? _SW.Elapsed : null, "Difference between encoded strings and original # of string literals");



        }
    }
}
