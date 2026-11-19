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
    [AdventOfCode(Year = 2018, Day = 1)]
    public class Year2018Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2018Day01()
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

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            int[] measurements = FileIOHelper.getInstance().ReadDataToIntArray(file);
            
            _SW.Start();
            int increases = Part1(measurements);
            _SW.Stop();



            output.WritePart(1, increases, trackTime ? _SW.Elapsed : null, "Number of increases (Actual Measurements)");

            
            _SW.Restart();

            increases = Part2(measurements);
            _SW.Stop();

            output.WritePart(2, increases, trackTime ? _SW.Elapsed : null, "Number of increases (Sliding Measurements)");



        }

        private int Part1(int[] measurements)
        {
            return 0;
        }

        private int Part2(int[] measurements)
        {
            return 0;
        }
    }
}
