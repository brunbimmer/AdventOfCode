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
    [AdventOfCode(Year = 2021, Day = 1)]
    public class Year2021Day1: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2021Day1()
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
            return GetIncreases(measurements);
        }

        private int Part2(int[] measurements)
        {
            int[] slidingMeasurements = ConvertToSlidingMeasurementStream(measurements);
            return GetIncreases(slidingMeasurements);
        }

        private int GetIncreases(int[] measurements)
        {
            int increases = 0;

            for (int i = 1; i < measurements.Length; i++)
            {
                if (measurements[i] > measurements[i - 1])
                    increases = increases + 1;
            }
            return increases;
        }

        private int[] ConvertToSlidingMeasurementStream(int[] measurements)
        {
            List<int> slidingInputStream = new List<int>();

            for (int i = 0; i < measurements.Length; i++)
            {
                if ((i + 2) >= measurements.Length)
                    break;

                int sum = measurements[i] + measurements[i + 1] + measurements[i + 2];

                slidingInputStream.Add(sum);
            }

            return slidingInputStream.ToArray();
        }
    }
}
