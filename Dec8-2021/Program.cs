using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DayPuzzle
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Count() == 1)
                {
                    string filename = args[0];
                    Program.Run(filename).Wait();
                }
                else
                {
                    Console.WriteLine("Invalid Arguments. Please specify input filename only.");
                    Console.WriteLine("   DailyPuzzle input.txt");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        private async static Task Run(string filename)
        {
            //Step 1: Read in the file
            Console.WriteLine("Loading Input file");
            string[] lines = File.ReadAllLines(filename);

            int totalSequencesEasyDigits = 0;

            List<int> outputSignalSequence = new List<int>();

            foreach (string line in lines)
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

            Console.WriteLine("Part 1: Sequence of Easy Decipherable Digits: {0}", totalSequencesEasyDigits);
            Console.WriteLine("Part 2: Sum of Output Values: {0}", outputSignalSequence.Sum());

        }
    }
}
