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
    [AdventOfCode(Year = 2015, Day = 14)]
    public class Year2015Day14 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        private class ReinderStat
        {
            public string name;
            public int speed;
            public int flyingTime;
            public int restTime;
            public int distancePart1;

            public int GetDistance(int time)
            {
                return (flyingTime * (time / (flyingTime + restTime)) 
                                        + Math.Min(flyingTime, time % (flyingTime + restTime))) * speed;

            }
        }


        public Year2015Day14()
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

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            List<ReinderStat> stats = ParseReinderStats(instructions);

            RunSimulation1(stats);

            int reinderMaxDistance = stats.Max(x => x.distancePart1);
            
            SW.Stop();

            Console.WriteLine("Part 1: Distance winning deer travelled {0}, Execution Time: {1}", reinderMaxDistance, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            int reinderMaxPoints = RunSimulation2(stats);
            
            SW.Stop();

            Console.WriteLine("Part 2: Points winning deer is {0}, Execution Time: {1}", reinderMaxPoints, StopwatchUtil.getInstance().GetTimestamp(SW));



        }       

        List<ReinderStat> ParseReinderStats(string[] instructions)
        {
            List<ReinderStat> stats = new List<ReinderStat>();

            foreach (string line in instructions)
            {
                var data = line.Split(' ');
                ReinderStat stat = new();

                stat.name = data[0];
                stat.speed = Convert.ToInt32(data[3]);
                stat.flyingTime = Convert.ToInt32(data[6]);
                stat.restTime = Convert.ToInt32(data[13]);
                               
                stats.Add(stat);
            }

            return stats;
        }

        private void RunSimulation1(List<ReinderStat> stats)
        {
            int totalTime = 2503;

            stats.ForEach(stat =>
            {
                stat.distancePart1 = stat.GetDistance(totalTime);
            });
        }

        private int RunSimulation2(List<ReinderStat> stats)
        {
            var points = new Dictionary<string, int>(stats.Count);
            stats.ToList().ForEach(r => points[r.name] = 0);

            int totalTime = 2503;

            for(int t = 1; t <= totalTime; t++)
            {
                var maxDistance = stats.Select (r => r.GetDistance(t)).Max();

                stats.Where(r => r.GetDistance(t) == maxDistance).ToList().ForEach(r => points[r.name]++);
            }

            return points.Values.Max();
        }
    }
}
