using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 15)]
    public class Year2022Day15 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2022Day15()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        private record Sensor (long X, long Y, long ManhattanDistance);

        HashSet<(long X, long Y)> mapOfObjects;
        List<Sensor> sensors;

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            (mapOfObjects, sensors) = BuildTunnelMap(input);

            long numExcludedBeaconLocations = CalculateExcludedZoneOptimized(mapOfObjects, sensors, 2000000);
            
            SW.Stop();

            Console.WriteLine("Part 1: Number of Exclusion Beacon Locations : {0}, Execution Time: {1}", numExcludedBeaconLocations, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            long tuningFrequency = CalculateFrequency(sensors);            
            
            SW.Stop();

            Console.WriteLine("Part 2: Tuning Frequency {0}, Execution Time: {1}", tuningFrequency, StopwatchUtil.getInstance().GetTimestamp(SW));


        }       

        private (HashSet<(long X, long Y)> Map, List<Sensor> Sensors) BuildTunnelMap(string[] input)
        {
            var map = new HashSet<(long X, long Y)>();
            var sensors = new List<Sensor>();

            foreach (string line in input)
            {
                string[] parts = line.Split(":", StringSplitOptions.RemoveEmptyEntries);

                string sensorCoordinates = parts[0].Remove(0, new String("Sensor at ").Length);
                string beaconCoordinates = parts[1].Remove(0, new String("closest beacon is at ").Length);

                long x = Convert.ToInt32(sensorCoordinates.Split(",", StringSplitOptions.TrimEntries)[0].Remove(0,2));
                long y = Convert.ToInt32(sensorCoordinates.Split(",", StringSplitOptions.TrimEntries)[1].Remove(0,2));

                var sensorPoint = (x, y);

                x = Convert.ToInt32(beaconCoordinates.Split(",", StringSplitOptions.TrimEntries)[0].Remove(0,2));
                y = Convert.ToInt32(beaconCoordinates.Split(",", StringSplitOptions.TrimEntries)[1].Remove(0,2));

                var beaconPoint = (x, y);

                var sensor = new Sensor(sensorPoint.x, sensorPoint.y, Math.Abs(sensorPoint.x - beaconPoint.x) + Math.Abs(sensorPoint.y - beaconPoint.y));

                map.Add(sensorPoint);
                map.Add(beaconPoint);
                sensors.Add(sensor);
            }

            return (map, sensors);
        }       

        private long CalculateExcludedZoneOptimized(HashSet<(long X, long Y)> map, List<Sensor> sensors, long row)
        {
            var excludedPoints = new HashSet<(long X, long Y)>();

            HashSet<Coordinate2D> excludedZone = new HashSet<Coordinate2D>();

            long[] xPointsTaken = map.Where(p => p.Y == row).Select(p => p.X).ToArray();

            foreach(var sensor in sensors)
            {
                long manhattenDistance = sensor.ManhattanDistance;

                //work with the coordinates inside the sensor boundary
                if (sensor.Y - manhattenDistance <= row && row <= sensor.Y + manhattenDistance)
                {
                    //find the actual distance between the row and sensor. This becomes our x-span
                    long rowDistance = row > sensor.Y ?
                                (sensor.Y + manhattenDistance) - row :
                                    row - (sensor.Y - manhattenDistance);


                    //Add points to a list, but don't include points taken by a beacon or sensor.
                    for (var x = sensor.X - rowDistance; x <= sensor.X + rowDistance; x++)
                    {
                        if (!xPointsTaken.Contains(x))
                            excludedPoints.Add((x, row));
                    } 
                }                  
            }
            return excludedPoints.Count();
        }

        private long CalculateFrequency(List<Sensor> sensors)
        {
            var maxCoordinate = 4000000;

            foreach (var sensor in sensors)
            {
                //starting with x coordinate, we go from the 0, or the outer edge of the Manhatten radius and move forward to the sensor edge or max coordinate.
                for (var i = Math.Max(0, sensor.X - sensor.ManhattanDistance - 1); i <= Math.Min(maxCoordinate, sensor.X + sensor.ManhattanDistance + 1); ++i)
                {
                    //compute a upper Y coordinate of the sensor range or maximum coordinate.
                    var upperY = Math.Min(sensor.Y + sensor.ManhattanDistance + 1 - Math.Abs(sensor.X - i), maxCoordinate);
                    
                    //compute a lower Y coordinate of the sensor range or minimum coordinate.
                    var lowerY = Math.Max(sensor.Y - (sensor.ManhattanDistance + 1 - Math.Abs(sensor.X - i)), 0);
                    
                    //perform an all sensor sweep to the upper boundary.

                    //Test condition against all sensor inputs. We found out coordinate when the manhatten distance for each
                    //sensor and the test point is greater than the manhatten distance the sensor and its associated beacon.
                    if (sensors.All(s => Math.Abs(s.X - i) + Math.Abs(s.Y - upperY) > s.ManhattanDistance))
                    {
                        return i * 4000000 + upperY;
                    }
                    
                    //Test condition for the lower Y point test.
                    if (sensors.All(s => Math.Abs(s.X - i) + Math.Abs(s.Y - lowerY) > s.ManhattanDistance))
                    {
                        return i * 4000000 + lowerY;
                    }
                }
            }

            return -1;
        }
      
    }
}
