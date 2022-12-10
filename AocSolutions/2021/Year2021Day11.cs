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
    [AdventOfCode(Year = 2021, Day = 11)]
    public class Year2021Day11 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2021Day11()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            Dictionary<(int, int), int> octopusGrid = FileIOHelper.getInstance().GetDataAsIntegerMap(file);

            SW.Start();                       

            int flashCountAt100 = 0;
            int stepCount = 0;
            (flashCountAt100, stepCount) = OctopusSimulation(octopusGrid, 100);

            
            SW.Stop();

            Console.WriteLine("Part 1: Number of Flashes at Step 100: {0}", flashCountAt100);
            Console.WriteLine("Part 2: First Step Count when all Octopus are lit: {0}", stepCount);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


        }    
        
        private (int, int) OctopusSimulation(Dictionary<(int, int), int> octopusGrid, int stepCount)
        {
            //Find corrupt lines
            int numOfFlashesAtStep100 = 0;
            int numFlashes = 0;

            int step = 0;
            bool stopSimulation = false;
            
            while (stopSimulation != true)
            {
                //increase the value of the light output in each step
                foreach (KeyValuePair<(int, int), int> entry in octopusGrid)
                {
                    var dataPoint = entry.Key;
                    var newLightOutput = (entry.Value == 9) ? 0 : entry.Value + 1;
                    octopusGrid[entry.Key] = newLightOutput;
                }
                List<(int, int)> processed = new List<(int, int)>();

                PerformZeroCheckAndUpdate(octopusGrid, octopusGrid.Where(x => x.Value == 0).ToDictionary(x => x.Key, x => x.Value), processed);

                numFlashes = numFlashes + octopusGrid.Values.Count(x => x == 0);

                if (step == (stepCount - 1))
                    numOfFlashesAtStep100 = numFlashes;

                step = step + 1;

                if (octopusGrid.Values.Count(x => x == 0) == 100)
                    stopSimulation = true;
            }

            return (numOfFlashesAtStep100, step);
        }


        private void PerformZeroCheckAndUpdate(Dictionary<(int, int), int> octopusGrid, Dictionary<(int, int), int> zeroList, List<(int, int)> processed)
        {
            if (zeroList.Count == processed.Count) return;
           
            foreach (KeyValuePair<(int, int), int> entry in zeroList)
            {
                //skip if this is not a new zero entry.
                if (processed.Contains(entry.Key)) continue;
                
                //add new data point to processed List.
                processed.Add(entry.Key);
                var dataPoint = entry.Key;

                (int, int)[] adjacentsPoints = { (dataPoint.Item1 - 1, dataPoint.Item2),     //left
                                                (dataPoint.Item1 + 1, dataPoint.Item2),      //right
                                                (dataPoint.Item1, dataPoint.Item2 - 1),      //up
                                                (dataPoint.Item1, dataPoint.Item2 + 1),      //down
                                                (dataPoint.Item1 - 1, dataPoint.Item2 - 1),  //top left
                                                (dataPoint.Item1 + 1, dataPoint.Item2 + 1),  //top right
                                                (dataPoint.Item1 - 1, dataPoint.Item2 + 1),  //bottom left
                                                (dataPoint.Item1 + 1, dataPoint.Item2 - 1),  //bottom right
                };

                //increase light out output at every adjacent non-zero points;

                foreach ((int, int) point in adjacentsPoints)
                {
                    if (!octopusGrid.ContainsKey(point)) continue;
                    if (octopusGrid[point] != 0)
                        octopusGrid[point] = (octopusGrid[point] == 9) ? 0 : octopusGrid[point] + 1;
                }
            }

            PerformZeroCheckAndUpdate(octopusGrid, octopusGrid.Where(x => x.Value == 0).ToDictionary(x => x.Key, x => x.Value), processed);
        }
    }
}
