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
    [AdventOfCode(Year = 2022, Day = 12)]
    public class Year2022Day12 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2022Day12()
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

            Dictionary<(int, int), int> grid = FileIOHelper.getInstance().GetDataAsCharMapWithValues(file);

            SW.Start();
            (int, int) startPoint = grid.Where(x => x.Value == CalculateLetterValue('S')).FirstOrDefault().Key;
            (int, int) endPoint = grid.Where(x => x.Value == CalculateLetterValue('E')).FirstOrDefault().Key;
            grid[startPoint] = 1;   //set the value of the start point to 1
            grid[endPoint] = 27;    //one higher than the highest elevation before reaching destination;
            var pathPart = MapLowestPathScoreAStar(grid, startPoint, endPoint);
            int steps = pathPart.Skip(1).Count();   //don't count the original location

            SW.Stop();
            Console.WriteLine("Part 1: Shortest # of steps {0}, Execution Time: {1}", steps, StopwatchUtil.getInstance().GetTimestamp(SW));
            SW.Restart();
            List<int> PathLengths = new();
            var lowPoints = grid.Where(a => a.Value == 1).ToList();     //get all low points
            foreach (var a in lowPoints)
            {
                var a_path = MapLowestPathScoreAStar(grid, a.Key, endPoint);
                if (a_path != null && a_path[0] == a.Key)
                {
                    PathLengths.Add(a_path.Count - 1);
                }
            }
            int shortestSteps = PathLengths.Min();
            SW.Stop();
            Console.WriteLine("Part 2: Least Steps from any starting point A {0}, Execution Time: {1}", shortestSteps, StopwatchUtil.getInstance().GetTimestamp(SW));

        }
        private List<(int, int)> MapLowestPathScoreAStar(Dictionary<(int, int), int> grid, (int, int) start, (int, int) goal)
        {
            PriorityQueue<(int, int), int> openSet = new();
            Dictionary<(int, int), (int, int)> cameFrom = new();
            Dictionary<(int, int), int> gScore = new();
            gScore[start] = 0;
            Dictionary<(int, int), int> fScore = new();
            fScore[start] = 0;
            openSet.Enqueue(start, fScore[start]);
            while (openSet.TryDequeue(out (int, int) cur, out _))
            {
                if (cur.Equals(goal))
                {
                    return ReconstructPath(cameFrom, cur);
                }
                (int, int)[] adjacentPoints = { (cur.Item1 - 1, cur.Item2),         //left
                                   (cur.Item1 + 1, cur.Item2),         //right
                                   (cur.Item1, cur.Item2 - 1),         //up
                                   (cur.Item1, cur.Item2 + 1) };       //down
                foreach (var n in adjacentPoints.ToList())
                {
                    //only go on paths that are equal or with a difference of one (going down).
                    //Not using int.max to avoid overflow
                    if (grid.GetValueOrDefault(n, 10_000_000) - grid.GetValueOrDefault(cur) <= 1)
                    {
                        var tentGScore = gScore[cur] + 1;
                        if (tentGScore < gScore.GetValueOrDefault(n, int.MaxValue) && grid.ContainsKey(n))
                        {
                            cameFrom[n] = cur;
                            gScore[n] = tentGScore;
                            fScore[n] = tentGScore + Distance(cur, goal);
                            openSet.Enqueue(n, fScore[n]);
                        }
                    }
                }
            }
            return null;
        }
        private List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> cameFrom, (int, int) current)
        {
            List<(int, int)> res = new();
            res.Add(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                res.Add(current);
            }
            res.Reverse();
            return res;
        }
        private int Distance((int, int) point, (int, int) otherPoint)
        {
            int x = Math.Abs(point.Item1 - otherPoint.Item1);
            int y = Math.Abs(point.Item2 - otherPoint.Item2);
            return x + y;
        }

        private int CalculateLetterValue(char character)
        {
            int ascii = (int)character;
            if (Char.IsUpper(character)) 
                return (ascii - 38);
            else
                return (ascii - 96);
        }
    }
}
