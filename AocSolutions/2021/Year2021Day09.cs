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
    [AdventOfCode(Year = 2021, Day = 9)]
    public class Year2021Day9 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2021Day9()
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

            Dictionary<(int, int), int> caveData = FileIOHelper.getInstance().GetDataAsIntegerMap(file);

            List<(int,int)> lowDataPoints = new List<(int,int)>();
            List<int> basins = new List<int>();

            _SW.Start();                       
            lowDataPoints = GetLowPoints(caveData);                            
            _SW.Stop();

            Console.WriteLine("Part 1: Sum of Risk Levels of Low Points: {0}", lowDataPoints.Select(lp => caveData[lp] + 1).Sum());
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            basins = GetBasins(caveData, lowDataPoints);

            //Take the largest 3 datapoints only to calculate the required result set.
            var maxDataPoints = basins.OrderByDescending(x => x).Take(3).ToArray();
            
            _SW.Stop();

            Console.WriteLine("Part 2: Largest Three Basins Multiplied: {0}", maxDataPoints[0] * maxDataPoints[1] * maxDataPoints[2]);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }        

       private   List<(int, int)> GetLowPoints(Dictionary<(int, int), int> caveData)
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

        private   List<int> GetBasins(Dictionary<(int, int), int> cavaData, List<(int, int)> lowDataPoints)
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

        private   void MapBasin(Dictionary<(int, int), int> caveData, (int, int) dataPoint, List<(int, int)> basinPoints)
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
