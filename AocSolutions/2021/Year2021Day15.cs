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
    [AdventOfCode(Year = 2021, Day = 15)]
    public class Year2021Day15 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2021Day15()
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

            Dictionary<(int, int), int> caveMap = FileIOHelper.getInstance().GetDataAsMap(file);

            SW.Start();                       

            var pathPart1 = MapLowestPathScoreAStar(caveMap, ( 0, 0), (caveMap.Keys.Max(x => x.Item1), caveMap.Keys.Max(x => x.Item2)));
            int pathScore1 = pathPart1.Skip(1).Sum(x => caveMap[x]);

            
            SW.Stop();

            Console.WriteLine("Part 1: Lowest Total Risk from top left to bottom right: {0}", pathScore1);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            GrowMap(caveMap);
            
            SW.Stop();

            var pathPart2 = MapLowestPathScoreAStar(caveMap, ( 0, 0), (caveMap.Keys.Max(x => x.Item1), caveMap.Keys.Max(x => x.Item2)));
            int pathScore2 = pathPart2.Skip(1).Sum(x => caveMap[x]);

            Console.WriteLine("Part 2: Lowest Total Risk from top left to bottom right in larger map: {0}", pathScore2);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("\n===========================================\n");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }       

        private int MapLowestPathScore(Dictionary<(int, int), int> caveMap, (int, int) point, (int, int) MaxPoint)
        {
            if (point == MaxPoint)
                return caveMap[MaxPoint];

            int value = (point != (0,0)) ? caveMap[point]: 0;

            (int, int) adjacentPointDown = (point.Item1 + 1, point.Item2);
            (int, int) adjacentPointRight = (point.Item1, point.Item2 + 1);

            int? ValueRight = caveMap.ContainsKey(adjacentPointRight) ? caveMap[adjacentPointRight] : null;
            int? ValueDown = caveMap.ContainsKey(adjacentPointDown) ? caveMap[adjacentPointDown] : null;

            int? pathValueDown = (ValueDown != null) ? MapLowestPathScore(caveMap, adjacentPointDown, MaxPoint) : null;
            int? pathValueRight = (ValueRight != null) ? MapLowestPathScore(caveMap, adjacentPointRight, MaxPoint) : null;

            if (pathValueRight == null || pathValueDown < pathValueRight)
                return value + (pathValueDown ?? -1);
            else
                return value + (pathValueRight ?? -1);
        }

        private List<(int, int)> MapLowestPathScoreAStar(Dictionary<(int, int), int> caveMap, (int, int) start, (int, int) goal)
        {
            PriorityQueue<(int, int), int> openSet = new();
            Dictionary<(int, int), (int, int)> cameFrom = new();

            Dictionary<(int, int), int> gScore = new();
            gScore[start] = 0;

            Dictionary<(int, int), int> fScore = new();
            fScore[start] = 0;
            openSet.Enqueue(start, fScore[start]);

            while(openSet.TryDequeue(out (int, int) cur, out _))
            {
                if(cur.Equals(goal))
                {
                    return ReconstructPath(cameFrom, cur);
                }

                (int, int)[] adjacentPoints = { (cur.Item1 - 1, cur.Item2),         //left
                                                (cur.Item1 + 1, cur.Item2),         //right
                                                (cur.Item1, cur.Item2 - 1),         //up
                                                (cur.Item1, cur.Item2 + 1) };       //down

                foreach(var n in adjacentPoints.ToList())
                {
                    //Not using int.max to avoid overflow
                    var tentGScore = gScore[cur] + caveMap.GetValueOrDefault(n, 10_000_000);
                    if(tentGScore < gScore.GetValueOrDefault(n, int.MaxValue) && caveMap.ContainsKey(n))
                    {
                        cameFrom[n] = cur;
                        gScore[n] = tentGScore;
                        fScore[n] = tentGScore + Distance((cur.Item1, cur.Item2), goal);
                        openSet.Enqueue(n, fScore[n]);
                    }
                }
            }

            return null;

        }

        private int Distance((int, int) point, (int, int) otherPoint)
        {
            int x = Math.Abs(point.Item1 - otherPoint.Item1);
            int y = Math.Abs(point.Item2 - otherPoint.Item2);
            return x + y;
        }

        private List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> cameFrom, (int, int) current)
        {
            List<(int, int)> res = new();
            res.Add(current);
            while(cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                res.Add(current);
            }
            res.Reverse();
            return res;
        }

        private void GrowMap(Dictionary<(int, int), int> caveMap)
        {
            var largeMap = new Dictionary<(int, int), int>(caveMap);

            int originalDimension = caveMap.Keys.Max(x => x.Item1) + 1;

            int newMaxDimension = (originalDimension * 5) - 1;

            //Expand down
            var curKeys = KeyListCopy(caveMap);

            foreach(var key in curKeys)
            {
                int newVal = caveMap[key];
                foreach(int i in Enumerable.Range(1,4))
                {
                    newVal++;
                    if (newVal > 9) newVal = 1;
                    caveMap[(key.Item1 + (i * originalDimension), key.Item2)] = newVal;

                }
            }

            //Expand right
            curKeys = KeyListCopy(caveMap);

            foreach (var key in curKeys)
            {
                int newVal = caveMap[key];
                foreach (int i in Enumerable.Range(1, 4))
                {
                    newVal++;
                    if (newVal > 9) newVal = 1;
                    caveMap[(key.Item1, key.Item2 + (i * originalDimension))] = newVal;
                }
            }
        }

        public List<(int, int)> KeyListCopy(Dictionary<(int, int), int> caveMap, bool sorted = false)
        {
            List<(int, int)> keyList = new();

            foreach ((int, int) key in caveMap.Keys)
            {
                keyList.Add(key);
            }

            return keyList;
        }
    }
}
