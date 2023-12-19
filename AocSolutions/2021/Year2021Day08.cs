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
    [AdventOfCode(Year = 2021, Day = 8)]
    public class Year2021Day8 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2021Day8()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();        
            
            int totalDigits, sumSequences = 0;

            (totalDigits, sumSequences) = DecodeSequences(input);

            _SW.Stop();

            Console.WriteLine("Part 1: Sequence of Easy Decipherable Digits: {0}", totalDigits);
            Console.WriteLine("Part 2: Sum of Output Values: {0}", sumSequences);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }        

        private (int, int) DecodeSequences(string[] input)
        {
            int totalSequencesEasyDigits = 0;

            List<int> outputSignalSequence = new List<int>();

            foreach (string line in input)
            {
                Dictionary<string, int> characterMap = new Dictionary<string, int>();

                string[] sequences = line.Split('|');

                string[] signalPatterns = sequences[0].Trim().Split(' ');
                string[] outputPatterns = sequences[1].Trim().Split(' ');


                //This is to tally up the total # of easy digit patterns for Part 1.
                totalSequencesEasyDigits = totalSequencesEasyDigits + outputPatterns.Count(x => x.Length == 2 
                    || x.Length == 3
                    || x.Length == 4
                    || x.Length == 7);

                string seq1 = signalPatterns.First(x => x.Length == 2);     //represents 1
                string seq4 = signalPatterns.First(x => x.Length == 4);     //represents 4
                string seq7 = signalPatterns.First(x => x.Length == 3);     //represents 7
                string seq8 = signalPatterns.First(x => x.Length == 7);     //represents 8

                //Add the ordered sequence of characters to the Character map.
                characterMap.Add(String.Concat(seq1.OrderBy(c => c)), 1);
                characterMap.Add(String.Concat(seq4.OrderBy(c => c)), 4);
                characterMap.Add(String.Concat(seq7.OrderBy(c => c)), 7);
                characterMap.Add(String.Concat(seq8.OrderBy(c => c)), 8);

                string[] leftOver = signalPatterns.Where(x => x.Length != 2 && x.Length != 4 && x.Length != 3 && x.Length != 7).OrderBy(x => x.Length).ToArray();
                
                //use Regular Expressions to determine the rest.

                List<int> digits = new List<int>();
                foreach (string signal in leftOver)
                {
                    //convert to ascending order characters
                    string signalAsc = String.Concat(signal.OrderBy(c => c));
                    if (signal.Length == 5)
                    {
                        if (Regex.Matches(signal, "[" + seq7 + "]").Count == 3) //check for digit 3
                            characterMap.Add(signalAsc, 3);
                        else if (Regex.Matches(signal,"[" + seq4 + "]").Count == 3)  //check for digit 5
                            characterMap.Add(signalAsc, 5);
                        else
                            characterMap.Add(signalAsc, 2); //digit 
                    }
                    else
                    {
                        if (Regex.Matches(signal, "[" + seq4 + seq7 + "]").Count == 5)  //check for digit 9
                            characterMap.Add(signalAsc, 9);
                        else if (Regex.Matches(signal, "[" + seq7 + "]").Count == 3) //check for digit 0
                            characterMap.Add(signalAsc, 0);
                        else
                            characterMap.Add(signalAsc, 6); //digit 6
                    }
                }

                //Now check the output signal and compare the ordered sequence against the character map. 

                foreach (string outputSignal in outputPatterns)
                {
                    string outputSignalOrdered = String.Concat(outputSignal.OrderBy(c => c));
                    digits.Add(characterMap[outputSignalOrdered]);
                }

                outputSignalSequence.Add(int.Parse(string.Join("", digits)));
            }
            
            return (totalSequencesEasyDigits, outputSignalSequence.Sum());
        }
    }
}
