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
        private class Directions
        {
            public string Direction { get; set; }
            public int Value { get; set; }
        }


        private static List<Directions> inputStream = new List<Directions>();

        private static int finalHorizontalPosition = 0;
        private static int finalDepth = 0;



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
            ParsePositionStream(inputStream);

			Console.WriteLine("Part 1: Final Horizontal Position ({0}) * Final Depth ({1}): {2}", finalHorizontalPosition, finalDepth, finalHorizontalPosition * finalDepth);

            #endregion

            #region Part 2

            ParsePositionForAimOutput(inputStream);

            Console.WriteLine("Part 2: Final Horizontal Position ({0}) * Final Depth ({1}): {2}", finalHorizontalPosition, finalDepth, finalHorizontalPosition * finalDepth);


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
                        Directions newDirection = new Directions();
                        string[] dataInput = line.Split(' ');
                        newDirection.Direction = dataInput[0];
                        newDirection.Value = Convert.ToInt32(dataInput[1]);

                        inputStream.Add(newDirection);
                    }
                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ParsePositionStream(List<Directions> inputStream)
        {
            finalHorizontalPosition = inputStream.Where(x => x.Direction == "forward").ToList().Sum(x => x.Value);
            int numOfUps = inputStream.Where(x => x.Direction == "up").ToList().Sum(x => x.Value);
            int numOfDowns = inputStream.Where(x => x.Direction == "down").ToList().Sum(x => x.Value);

            finalDepth = (numOfDowns - numOfUps);
        }

        private static void ParsePositionForAimOutput(List<Directions> inputStream)
        {
            //now we have to loop as each instruction will result in a altered depth.
            int horizontalPosition = 0;
            int aim = 0;
            int depth = 0;

            foreach (Directions direction in inputStream)
            {
                switch (direction.Direction)
                {
                    case "down":
                        aim = aim + direction.Value;
                        break;
                    case "up":
                        aim = aim - direction.Value;
                        break;
                    case "forward":
                        horizontalPosition = horizontalPosition + direction.Value;
                        depth = depth + direction.Value * aim;
                        break;
                    default:
                    break;
                }
            }

            finalHorizontalPosition = horizontalPosition;
            finalDepth = depth;
        }
    }
}
