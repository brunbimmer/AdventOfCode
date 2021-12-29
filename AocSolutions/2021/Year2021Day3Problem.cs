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
    [AdventOfCode(Year = 2021, Day = 3)]
    public class Year2021Day3Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2021Day3Problem()
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
            string filename = _OverrideFile ?? path;

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
            List<int[]> binaryInputStream = new List<int[]>();
            //convert to List of int[]
            foreach (string line in lines)
            {
                int[] inputLine = line.Select(c => int.Parse(c.ToString())).ToArray();

                binaryInputStream.Add(inputLine);
            }

            if (trackTime) SW.Start();
            int powerOutput = Part1(binaryInputStream);
            if (trackTime) SW.Stop();



            Console.WriteLine("Part 1: Power Output: {0}", powerOutput);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            int lifeSupport = Part2(binaryInputStream);

            if (trackTime) SW.Stop();

            Console.WriteLine("Part 2: Life Support Rating Output: {0}", lifeSupport);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        private int Part1(List<int[]> binaryInputStream)
        {
            //all lines are the same length, so get the lineLength of first line
            int lineLength = binaryInputStream.First().Length;

            int[] GammaRate = new int[lineLength];
            int[] EpsilonRate = new int[lineLength];

            for (int i = 0; i < lineLength; i++)
            {
                int bit1Counter = 0;
                int bit0Counter = 0;

                foreach (int[] line in binaryInputStream)
                {
                    if (line[i] == 1)
                        bit1Counter++;
                    else
                        bit0Counter++;
                }

                if (bit1Counter == bit0Counter)
                {
                    Console.WriteLine("We encountered an equal number of 0's and 1's in position {0}, what is the solution", i);
                }

                if (bit1Counter > bit0Counter)
                {
                    GammaRate[i] = 1;
                    EpsilonRate[i] = 0;
                }
                else
                {
                    GammaRate[i] = 0;
                    EpsilonRate[i] = 1;
                }
            }

            string strGammaRate = string.Join("", GammaRate);
            string strEpsilonRate = string.Join("", EpsilonRate);

            int iGammaRate = Convert.ToInt32(strGammaRate, 2);
            int iEpilsonRate = Convert.ToInt32(strEpsilonRate, 2);

            return iGammaRate * iEpilsonRate;
        }

        private int Part2(List<int[]> binaryInputStream)
        {
            //Oxygen Generator Rating
            List<int[]> OxygenRate = RecursiveParseOxygenRating(binaryInputStream, 0);

            //CO2 Scrubber Rating
            List<int[]> C02Rate = RecursiveParseC02Rating(binaryInputStream, 0);

            //Display Life Support Rating

            string strOxygenRate = string.Join("", OxygenRate[0]);
            string strC02Rate = string.Join("", C02Rate[0]);

            int iOxygenRate = Convert.ToInt32(strOxygenRate, 2);
            int iCO2Rate = Convert.ToInt32(strC02Rate, 2);

            return iOxygenRate * iCO2Rate;
        }

        private List<int[]> RecursiveParseOxygenRating(List<int[]> recurseItemList, int position)
        {
            if (recurseItemList.Count == 1)
            {
                return recurseItemList;
            }
            else
            {
                List<int[]> interimResults1 = recurseItemList.Where(item => item[position] == 1).ToList();
                List<int[]> interimResults0 = recurseItemList.Where(item => item[position] == 0).ToList();

                if (interimResults1.Count == interimResults0.Count)
                {
                    return RecursiveParseOxygenRating(interimResults1, (position + 1));
                }
                else if (interimResults1.Count > interimResults0.Count)
                {
                    return RecursiveParseOxygenRating(interimResults1, (position + 1));
                }
                else
                {
                    return RecursiveParseOxygenRating(interimResults0, (position + 1));
                }
            }
        }

        private List<int[]> RecursiveParseC02Rating(List<int[]> recurseItemList, int position)
        {
            if (recurseItemList.Count == 1)
            {
                return recurseItemList;
            }
            else
            {
                List<int[]> interimResults1 = recurseItemList.Where(item => item[position] == 1).ToList();
                List<int[]> interimResults0 = recurseItemList.Where(item => item[position] == 0).ToList();

                if (interimResults1.Count == interimResults0.Count)
                {
                    return RecursiveParseC02Rating(interimResults0, (position + 1));
                }
                else if (interimResults0.Count < interimResults1.Count)
                {
                    return RecursiveParseC02Rating(interimResults0, (position + 1));
                }
                else
                {
                    return RecursiveParseC02Rating(interimResults1, (position + 1));
                }
            }
        }
    }
}
