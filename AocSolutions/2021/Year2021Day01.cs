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

        public Stopwatch SW { get; set; }

        public Year2021Day1()
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

            int[] measurements = FileIOHelper.getInstance().ReadDataToIntArray(file);
            
            SW.Start();
            int increases = Part1(measurements);
            SW.Stop();



            Console.WriteLine("  Part 1: Number of increases (Actual Measurements): {0}", increases);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));
            
            SW.Restart();

            increases = Part2(measurements);
            SW.Stop();

            Console.WriteLine("  Part 1: Number of increases (Sliding Measurements): {0}", increases);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


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
