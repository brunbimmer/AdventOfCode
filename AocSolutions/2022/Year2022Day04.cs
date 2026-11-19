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
    [AdventOfCode(Year = 2022, Day = 4)]
    public class Year2022Day04 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day04()
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

            List<string> input = FileIOHelper.getInstance().ReadDataAsLines(file).ToList();

            _SW.Start();                       

            int inclusivePairs, overlapPairs;

            (inclusivePairs, overlapPairs) = FindPairs(input);
            
            _SW.Stop();

            output.WritePart(1, inclusivePairs, trackTime ? _SW.Elapsed : null, "Inclusive Assignment Pairs");
            output.WritePart(2, overlapPairs, trackTime ? _SW.Elapsed : null, "Overlap Assignment Pairs");


        }       

        private (int,int) FindPairs(List<string> input)
        {
            int inclusivePairs = 0;
            int overlappingPairs = 0;

            foreach (string pair in input)
            {
                string[] sectionRanges = pair.Split(new char[] {',','-'});

                int minPair1 = Convert.ToInt32(sectionRanges[0]);
                int maxPair1 = Convert.ToInt32(sectionRanges[1]);
                int minPair2 = Convert.ToInt32(sectionRanges[2]);
                int maxPair2 = Convert.ToInt32(sectionRanges[3]);


                int[] section1 = Enumerable.Range(minPair1, maxPair1 - minPair1 + 1).ToArray();
                int[] section2 = Enumerable.Range(minPair2, maxPair2 - minPair2 + 1).ToArray();

                int[] intersection = section1.Intersect(section2).ToArray();

                if (intersection.Length == section1.Length || intersection.Length == section2.Length)
                    inclusivePairs += 1;

                if (intersection.Length > 0)
                    overlappingPairs+= 1;

            }
            return (inclusivePairs, overlappingPairs);
        }
    }
}
