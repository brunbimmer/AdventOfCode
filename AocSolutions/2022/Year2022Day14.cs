using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 14)]
    public class Year2022Day14 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day14()
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

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       


            List<Coordinate2D[]> rockPath = input.Select(ParsePath).ToList();

            HashSet<Coordinate2D> walls = BuildWalls(rockPath).ToHashSet();

            var start = new Coordinate2D(500, 0);
            var bottom = walls.Select(p => p.Y).Max();
            
            var sandUnits = RunSandSimulation(start, bottom, walls.Contains);
           
            _SW.Stop();

            Console.WriteLine("Part 1: Number of Sand Units {0}, Execution Time: {1}", sandUnits.Count, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            var floor = bottom + 2;

            var sandUnits2 = RunSandSimulation(start, int.MaxValue, p => p.Y == floor || walls.Contains(p));
            
            _SW.Stop();

            Console.WriteLine("Part 2: Number of Sand Units {0}, Execution Time: {1}", sandUnits2.Count, StopwatchUtil.getInstance().GetTimestamp(_SW));

        }
        
        private Coordinate2D[] ParsePath(string input)
        {
           return input.Split("->", StringSplitOptions.TrimEntries)
                       .Select(p => p.Split(',', 2))
                       .Select(p => new Coordinate2D(int.Parse(p[0]), int.Parse(p[1])))
                       .ToArray();
        }

        private HashSet<Coordinate2D> BuildWalls(List<Coordinate2D[]> rockPathList )
        {
            HashSet<Coordinate2D> walls = new HashSet<Coordinate2D>();

            foreach(var path in rockPathList)
            {
                for (int i = 1; i < path.Length; i++)
                {
                    var wall = from X in Enumerable.Range(
                                            (int)Math.Min(path[i].X, path[i-1].X),
                                            (int)Math.Abs(path[i].X - path[i-1].X) + 1)
                               from Y in Enumerable.Range(
                                            (int)Math.Min(path[i].Y, path[i-1].Y),
                                            (int)Math.Abs(path[i].Y - path[i-1].Y) + 1)
                               select new Coordinate2D(X,Y);
                    
                    foreach(var brick in wall)
                        walls.Add(brick);

                }
            }

            return walls;
        }

        static HashSet<Coordinate2D> RunSandSimulation(Coordinate2D start, long bottom, Func<Coordinate2D, bool> isWall)
        {
          HashSet<Coordinate2D> sand = new();

          while (true)
          {
            var sandPos = ComputeFinalSandRestingPlace(start, bottom, p => isWall(p) || sand.Contains(p));

            if (sandPos == null)
              break;

            sand.Add(sandPos);
          }

          return sand;
        }

        static Coordinate2D ComputeFinalSandRestingPlace(Coordinate2D start, long bottom, Func<Coordinate2D, bool> blocked)
        {
            if (blocked(start))
                return null;

            var pos = start;
            while (pos.Y <= bottom)
            {
                var next = (from p in NextSandPositions(pos)
                                where !blocked(p)
                                select (Coordinate2D)p).FirstOrDefault();
            
                if (next == null)
                    return pos;

                pos = next;
            }

            return null;
        }

        static IEnumerable<Coordinate2D> NextSandPositions(Coordinate2D point)
        {
          var newY = point.Y + 1;
          yield return new(point.X, newY);
          yield return new(point.X - 1, newY);
          yield return new(point.X + 1, newY);
        }
    }
}
