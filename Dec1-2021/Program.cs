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

        private static List<int> inputStream = new List<int>();

		private static int numberOfIncreases = 0;


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
                    Console.WriteLine("   Day1Puzzle input.txt");
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
            ParseMeasurementStream(inputStream);

			Console.WriteLine("Number of increases (Actual Measurements): " + numberOfIncreases);

            #endregion

            #region Part 2

            List<int> slidingWindowInputStream = ParseStreamIntoSlidingMeasurementStream(inputStream); 
            
            //Redo the ParseStreamPart1 again to get # of increases;
            //reset Number of Increases;

            numberOfIncreases = 0;
            ParseMeasurementStream(slidingWindowInputStream);

            Console.WriteLine("Number of increases  (Sliding Measurement Window): " + numberOfIncreases);

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
                        inputStream.Add(Convert.ToInt32(line));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ParseMeasurementStream(List<int> inputStream)
        {

            for (int i = 0; i < inputStream.Count; i++)
            {
				if (i == 0)
					continue;
				
				if (inputStream[i] > inputStream[i - 1])
					numberOfIncreases = numberOfIncreases + 1;
            }
        }



        private static List<int> ParseStreamIntoSlidingMeasurementStream(List<int> inputStream)
        {
            List<int> slidingInputStream = new List<int>();

            for (int i = 0; i < inputStream.Count; i++)
            {
				if ((i + 2) >= inputStream.Count)
					break;
				
				int sum = inputStream[i] + inputStream[i + 1] + inputStream[i + 2];

                slidingInputStream.Add(sum);
            }

            return slidingInputStream;
        }
    }
}
