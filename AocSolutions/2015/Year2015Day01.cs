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

        public Stopwatch _SW { get; set; }

        public Year2015Day1Problem()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute) Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string line = FileIOHelper.getInstance().ReadDataAsString(file);

            //Part 1

            _SW.Start();

            int countUp = CommonAlgorithms.StringUtils.CountCharacters(line, '(');
            int countDown = CommonAlgorithms.StringUtils.CountCharacters(line, ')');

            _SW.Stop();

            output.WritePart(1, countUp - countDown, trackTime ? _SW.Elapsed : null, "What floor is Santa on");

            //Part 2
            _SW.Restart();
            _SW.Start();

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
            _SW.Stop();

            output.WritePart(2, pos, trackTime ? _SW.Elapsed : null, "Position of character when Santa enters basement");


        }
    }
}
