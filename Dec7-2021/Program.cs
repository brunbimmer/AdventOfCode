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

    int[] crabPositions = Array.ConvertAll(lines[0].Split(','), s => int.Parse(s));

    int median = GetMedian(crabPositions);
    int average = GetAverage(crabPositions);
    int centerOfMass = GetCenterOfMass(crabPositions);
    double fuelCost = 0;

    fuelCost = GetFuelCost(median, crabPositions, true);

    Console.WriteLine("Fuel Cost - Part 1 (Median: {0}): {1}", centerOfMass, fuelCost);

    fuelCost = GetFuelCost(average, crabPositions, false);

    Console.WriteLine("Fuel Cost - Part 2 (Average: {0}): {1}", average, fuelCost);

    fuelCost = GetFuelCost(centerOfMass, crabPositions, false);

    Console.WriteLine("Fuel Cost - Part 2 (Center of Mass: {0}): {1}", centerOfMass, fuelCost);
    Console.WriteLine("");
    Console.WriteLine("I am showing two possible fuel cost answers to the Part 2. One using average and another using center of mass.");
    Console.WriteLine("One of these options will result in the lower fuel cost. I am displaying both for comparision purposes.");

}

private static int GetMedian(int[] crabs)
{
    int numberCount = crabs.Count();
    int halfIndex = crabs.Count()/2;
    var sortedNumbers = crabs.OrderBy(n=>n);
    double median;
    if ((numberCount % 2) == 0)
    {
        median = ((sortedNumbers.ElementAt(halfIndex) +
                   sortedNumbers.ElementAt(halfIndex - 1))/ 2);
    } else {
        median = sortedNumbers.ElementAt(halfIndex);
    }
    return Convert.ToInt32(median);
}

private static int GetAverage(int[] crabs)
{
    return Convert.ToInt32(Math.Round(crabs.Average()));
}

private static int GetCenterOfMass(int[] crabs)
{
    var massPoints = crabs.Distinct().ToDictionary(x => x, x => crabs.Count(y => y == x));

    int centerOfMass = massPoints.Sum(x => x.Key * x.Value) / massPoints.Sum(x => x.Value);

    return centerOfMass;
}

private static double GetFuelCost(int position, int[] crabs, bool linearFuelConsumption)
{
    double fuelCost = 0;
    for (int i = 0; i < crabs.Length; i++)
    {
        if (linearFuelConsumption)
            fuelCost = fuelCost + Math.Abs(crabs[i] - position);
        else
            fuelCost = fuelCost + Convert.ToDouble(Enumerable.Range(1, Math.Abs(crabs[i] - position)).Sum());
    }

    return fuelCost;
}
    }
}
