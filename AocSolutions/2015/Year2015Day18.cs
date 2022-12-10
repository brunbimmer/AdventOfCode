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
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 18)]
    public class Year2015Day18 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day18()
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

            Dictionary<Coordinate2D, char> lightGrid = FileIOHelper.getInstance().GetDataAsCoordinates(file);

            SW.Start();                       

            int activeLights = AnimateLights(lightGrid, 100, false);
            
            SW.Stop();

            Console.WriteLine("Part 1: Num of Active Lights after 100 steps -> {0}, Execution Time: {1}", activeLights, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            //corner lights stuck in on position
            int maxX = lightGrid.Max(a => a.Key.X);
            int maxY = lightGrid.Max(a => a.Key.Y);
            
            lightGrid[new Coordinate2D(0, 0)] = '#';
            lightGrid[new Coordinate2D(0, maxY)] = '#';
            lightGrid[new Coordinate2D(maxX, 0)] = '#';
            lightGrid[new Coordinate2D(maxX, maxY)] = '#';

            activeLights = AnimateLights(lightGrid, 100, true);
                     
            SW.Stop();

            Console.WriteLine("Part 2: Num of Active Lights after 100 steps --> {0}, Execution Time: {1}", activeLights, StopwatchUtil.getInstance().GetTimestamp(SW));


        }
        
        public int AnimateLights(Dictionary<Coordinate2D, char> lightGrid, int numPasses, bool cornerActive)
        {
            //copy over existing data to another image that will store the enhancedImage so we don't destory original
            Dictionary<Coordinate2D, char> lightGridClone = new Dictionary<Coordinate2D, char>(lightGrid);

            foreach (int i in Enumerable.Range(1, numPasses))
            {
                lightGridClone = AnimateStep(lightGridClone, cornerActive);
            }
            return lightGridClone.Values.Count(x => x == '#');
        }

        private Dictionary<Coordinate2D, char> AnimateStep(Dictionary<Coordinate2D, char> lightGrid, bool cornerActive)
        {
            Dictionary<Coordinate2D, char> newLightGrid = new Dictionary<Coordinate2D, char>();

            int maxX = lightGrid.Max(a => a.Key.X);
            int maxY = lightGrid.Max(a => a.Key.Y);

            if (cornerActive)
            {
                newLightGrid[new Coordinate2D(0, 0)] = '#';
                newLightGrid[new Coordinate2D(0, maxY)] = '#';
                newLightGrid[new Coordinate2D(maxX, 0)] = '#';
                newLightGrid[new Coordinate2D(maxX, maxY)] = '#';
            }

            for (int x = 0; x <= maxX; x++)
            {
                for (int y = 0; y <= maxY; y++)
                {
                    if (cornerActive)
                    {
                        if (x == 0 && y == 0) continue;
                        if (x == 0 && y == maxY) continue;
                        if (x == maxX && y == 0) continue;
                        if (x == maxX && y == maxY) continue;
                    }

                    Coordinate2D currentCoordinate = new Coordinate2D(x, y);

                    List<Coordinate2D> neighbours = currentCoordinate.Neighbours(true, false);

                    int lightsOn = 0;
                    foreach (Coordinate2D neighbour in neighbours)
                    {
                        lightsOn += (lightGrid.GetValueOrDefault(neighbour, '.') == '#') ? 1 : 0; 
                    }

                    if (lightGrid[currentCoordinate] == '#')
                    {
                        if (lightsOn == 2 || lightsOn == 3) 
                            newLightGrid.Add(currentCoordinate, '#');
                        else
                            newLightGrid.Add(currentCoordinate, '.');
                    }

                    if (lightGrid[currentCoordinate] == '.')
                    {
                        if (lightsOn == 3) 
                            newLightGrid.Add(currentCoordinate, '#');
                        else
                            newLightGrid.Add(currentCoordinate, '.');
                    }
                }
            }

            return newLightGrid;
        } 
    }
}
