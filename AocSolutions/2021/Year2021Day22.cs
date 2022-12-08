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
    [AdventOfCode(Year = 2021, Day = 22)]
    public class Year2021Day22 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        private   List<(bool turnOn, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)> Steps = new();

        public Year2021Day22()
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

            string[] _Instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            foreach (var line in _Instructions)
            {
                bool turnOn = line[..2] == "on";
                var rawList = Regex.Split(line, @"[^\d-]+");
                var intList = rawList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                Steps.Add((turnOn, int.Parse(intList[0]), int.Parse(intList[1]), int.Parse(intList[2]), int.Parse(intList[3]), int.Parse(intList[4]), int.Parse(intList[5])));
            }

            int answer1 = Part1();

            
            SW.Stop();

             Console.WriteLine("Part 1: Number of Cubes on in the +/- 50 initialization perimeter boundary: {0}", answer1);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            long answer2 = Part2();
            
            SW.Stop();

            Console.WriteLine("Part 2: Number of Cubes on in entire surface area: {0}", answer2);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


        }      
        
        public int Part1()
        {
            Dictionary<Coordinate3D, bool> cuboids = new();

            foreach (var step in Steps)
            {
                var (turnOn, minX, maxX, minY, maxY, minZ, maxZ) = step;

                if (maxX > 50 || minX < -50
                    || maxY > 50 || minY < -50
                    || maxY > 50 || minZ < -50)

                { continue; }

                foreach (int x in Enumerable.Range(minX, (maxX - minX) + 1))
                    foreach (int y in Enumerable.Range(minY, (maxY - minY) + 1))
                        foreach (int z in Enumerable.Range(minZ, (maxZ - minZ) + 1))
                        {
                            cuboids[new Coordinate3D(x, y, z)] = turnOn;
                        }
            }

            return cuboids.Count(x => x.Value == true);
        }

        public long Part2()
        {
            //Record of all cubes
            Dictionary<(int minX, int maxX, int minY, int maxY, int minZ, int maxZ), long> cubes = new();

            foreach (var step in Steps)
            {
                (bool turnOn, int minX, int maxX, int minY, int maxY, int minZ, int maxZ) = step;
                
                long newStatus = turnOn ? 1 : -1;
                var newCuboid = (minX, maxX, minY, maxY, minZ, maxZ);

                Dictionary<(int minX, int maxX, int minY, int maxY, int minZ, int maxZ), long> newCuboids = new();


                foreach (var cube in cubes)
                {
                    //we are looking for intersections, split out the key to min/max values.
                    (int minX2, int maxX2, int minY2, int maxY2, int minZ2, int maxZ2) = cube.Key;
                    
                    //get the current value (on or off)
                    var curStatus = cube.Value;

                    //These determine the overlapping region, it even grabs complete interior bits.
                    int tmpMinX = Math.Max(minX, minX2);
                    int tmpMaxX = Math.Min(maxX, maxX2);
                    int tmpMinY = Math.Max(minY, minY2);
                    int tmpMaxY = Math.Min(maxY, maxY2);
                    int tmpMinZ = Math.Max(minZ, minZ2);
                    int tmpMaxZ = Math.Min(maxZ, maxZ2);

                    //Define a temporary cuboid.
                    var tmpCuboid = (tmpMinX, tmpMaxX, tmpMinY, tmpMaxY, tmpMinZ, tmpMaxZ);

                    //Basically, do we have a normal cuboid?
                    if (tmpMinX <= tmpMaxX && tmpMinY <= tmpMaxY && tmpMinZ <= tmpMaxZ)
                    {
                        //We are an intersection and must subtract it from our final count to avoid double counting
                        //if the instructions are to turn it on and also to want to remove it if the instructions
                        //are to turn this region off.
                        newCuboids[tmpCuboid] = newCuboids.GetValueOrDefault(tmpCuboid, 0) - curStatus;
                    }
                }
                //If instruction is set to turnOn, we want to set this with the new Cuboid. We don't just assign to
                //one in case a previous detected intersection set its status.
                if (newStatus == 1) newCuboids[newCuboid] = newCuboids.GetValueOrDefault(newCuboid, 0) + newStatus;

                //Add or update the value of the cuboids.
                foreach (var newCube in newCuboids)
                {
                    cubes[newCube.Key] = cubes.GetValueOrDefault(newCube.Key, 0) + newCube.Value;
                }
            }
            return cubes.Sum(a => (a.Key.maxX - a.Key.minX + 1L) * (a.Key.maxY - a.Key.minY + 1) * (a.Key.maxZ - a.Key.minZ + 1) * a.Value);
        }       
    }
}
