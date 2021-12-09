using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
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

            Dictionary<(int, int), int> caveData = GetDataSourceAsMap(filename);

            List<(int,int)> lowDataPoints = new List<(int,int)>();
            List<int> basins = new List<int>();

            lowDataPoints = GetLowPoints(caveData);                 
            basins = GetBasins(caveData, lowDataPoints);

            //Take the largest 3 datapoints only to calculate the required result set.
            var maxDataPoints = basins.OrderByDescending(x => x).Take(3).ToArray();

          
            Console.WriteLine("Part 1: Sum of Risk Levels of Low Points: {0}", lowDataPoints.Select(lp => caveData[lp] + 1).Sum());
            Console.WriteLine("Part 2: Largest Three Basins Multiplied: {0}", maxDataPoints[0] * maxDataPoints[1] * maxDataPoints[2]);
        }



        private static Dictionary<(int, int), int> GetDataSourceAsMap(string filepath)
        {
            Dictionary<(int, int), int> grid = new Dictionary<(int, int), int>();

            //read in the
            string[] lines = File.ReadAllLines(filepath);

            //Read the data
            var caveData = lines.Select(x => x.ToArray()).ToArray();

            for (int x = 0; x < caveData.Length; x++)
                for (int y = 0; y < caveData[x].Length; y++)
                    grid.Add((x, y), int.Parse(caveData[x][y].ToString()));

            return grid;
        }

        private static List<(int, int)> GetLowPoints(Dictionary<(int, int), int> caveData)
        {
            List<(int, int)> lowDataPoints = new List<(int, int)>();

            foreach (var e in caveData)
            {
                var value = e.Value;        //DataPoint Value
                var dataPoint = e.Key;     //Map Coordinate represents map coordinates (item1, item2);
                
                //get coordinates to the left, right, above and below the data point
                (int, int)[] adjacentsPoints = { (dataPoint.Item1 - 1, dataPoint.Item2), 
                                                 (dataPoint.Item1 + 1, dataPoint.Item2), 
                                                 (dataPoint.Item1, dataPoint.Item2 - 1), 
                                                 (dataPoint.Item1, dataPoint.Item2 + 1)
                                        };

                //For all valid adjacent points, determine if it is the lowest.
                var isLowPoint = adjacentsPoints.Select(v => (int?)(caveData.ContainsKey(v) ? caveData[v] : null))
                                                .All(a => (!a.HasValue || value < a));
                if (isLowPoint)
                {
                    lowDataPoints.Add(dataPoint);
                }
            }

            return lowDataPoints;
        }

        private static List<int> GetBasins(Dictionary<(int, int), int> cavaData, List<(int, int)> lowDataPoints)
        {
            List<int> basins = new List<int>();

            lowDataPoints.ForEach(lpDataPoint =>
            {
                var basinMap = new List<(int, int)>();
                MapBasin(cavaData, lpDataPoint, basinMap);
                basins.Add(basinMap.Count);
            });

            return basins;
        }

        private static void MapBasin(Dictionary<(int, int), int> caveData, (int, int) dataPoint, List<(int, int)> basinPoints)
        {
            //do the same thing, get immediate adjacement neighbours
            (int, int)[] adjacentPoints = { (dataPoint.Item1 - 1, dataPoint.Item2),         //left
                                            (dataPoint.Item1 + 1, dataPoint.Item2),         //right
                                            (dataPoint.Item1, dataPoint.Item2 - 1),         //up
                                            (dataPoint.Item1, dataPoint.Item2 + 1) };       //down

            basinPoints.Add(dataPoint);
            
            //Get List of adjacent targets to test except those already in the basinPoint Map.
            List<(int, int)> adjacentPointList = adjacentPoints.Where(v => v.Item1 >= 0 && v.Item2 >= 0 && !basinPoints.Contains(v)).ToList();
            
            foreach ((int, int) adjacentDataPoint in adjacentPointList)
            {
                //check if adjacent point contains data
                int? dataPointValue = caveData.ContainsKey(adjacentDataPoint) ? caveData[adjacentDataPoint] : null;    
                
                //continue to check on neighbours if the datapoint value is less than 9
                if (dataPointValue.HasValue && dataPointValue < 9 && !basinPoints.Contains(adjacentDataPoint))
                {
                    //Recurse to this datapoint and check adjacents.
                    MapBasin(caveData, adjacentDataPoint, basinPoints);
                }
            }
        }
    }
}
