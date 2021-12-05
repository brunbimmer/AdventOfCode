using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPuzzle
{
    class Program
    {

        private static List<int[]> inputStream = new List<int[]>();

        private static int[] GammaRate;
        private static int[] EpsilonRate;
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
                    Console.WriteLine("   Day3Solution inputFile.txt");
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
            ReadFile(filename);

            #region Part 1

            //Step 2: Parse the inputstream
            ParseStream();

            //Step 3: Calculate Power Output
            CalculatePowerOutput();

            #endregion

            #region Part 2

            //Oxygen Generator Rating
            List<int[]> OxygenRate = RecursiveParseOxygenRating(inputStream, 0);

            //CO2 Scrubber Rating
            List<int[]> C02Rate = RecursiveParseC02Rating(inputStream, 0);

            //Display Life Support Rating

            string strOxygenRate = string.Join("", OxygenRate[0]);
            string strC02Rate = string.Join("", C02Rate[0]);

            Console.WriteLine("OxygenRate Final Value: " + strOxygenRate);
            Console.WriteLine("CO2Rate Final Value:" + strC02Rate);

            int iOxygenRate = Convert.ToInt32(strOxygenRate, 2);
            int iCO2Rate = Convert.ToInt32(strC02Rate, 2);

            Console.WriteLine("Life Support Rating Output: " + iOxygenRate * iCO2Rate);

            #endregion
        }

        private static void ReadFile(string filename)
        {
            try
            {
                string line;
                using (TextReader reader = File.OpenText(filename))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        int[] inputLine = line.Select(c => int.Parse(c.ToString())).ToArray();

                        inputStream.Add(inputLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ParseStream()
        {
            //all lines are the same length, so get the lineLength of first line
            int lineLength = inputStream.First().Length;

            GammaRate = new int[lineLength];
            EpsilonRate = new int[lineLength];

            for (int i = 0; i < lineLength; i++)
            {
                int bit1Counter = 0;
                int bit0Counter = 0;

                foreach (int[] line in inputStream)
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
        }

        private static void CalculatePowerOutput()
        {
            string strGammaRate = string.Join("", GammaRate);
            string strEpsilonRate = string.Join("", EpsilonRate);

            Console.WriteLine("GammaRate Final Value: " + strGammaRate);
            Console.WriteLine("EpsilonRate Final Value:" + strEpsilonRate);

            int iGammaRate = Convert.ToInt32(strGammaRate, 2);
            int iEpilsonRate = Convert.ToInt32(strEpsilonRate, 2);

            Console.WriteLine("Power Output: " + iGammaRate * iEpilsonRate);
        }



        private static List<int[]> RecursiveParseOxygenRating(List<int[]> recurseItemList, int position)
        {
            if (recurseItemList.Count == 1)
            {
                return recurseItemList;
            }
            else
            {
                //all lines are the same length, so get the lineLength of first line
                int lineLength = inputStream.First().Length;

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

        private static List<int[]> RecursiveParseC02Rating(List<int[]> recurseItemList, int position)
        {
            if (recurseItemList.Count == 1)
            {
                return recurseItemList;
            }
            else
            {
                //all lines are the same length, so get the lineLength of first line
                int lineLength = inputStream.First().Length;

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
