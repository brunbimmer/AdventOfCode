using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            long[] fishInput = Array.ConvertAll(lines[0].Split(','), s => long.Parse(s));

            CountFish(fishInput, 80);
            CountFish(fishInput, 256);
        }

        private static void CountFish(long[] fishInput, int days)
        {
            long[] fishCounter = new long[9];

            for(int i = 0; i <=8; i++)
                fishCounter[i] = fishInput.Count(x => x == i);

            for(int i = 0; i < days; i++) {
                long newFish = fishCounter[0];

                for(int d = 0; d < 8; d++) {
                    fishCounter[d] = fishCounter[d + 1];
                }
                
                fishCounter[6] += newFish;
                fishCounter[8] = newFish;
            }
            long NumOfFish = fishCounter.Sum();

            Console.WriteLine("Number of Fish after {0} days: {1}", days, NumOfFish);
        }
    }
}
