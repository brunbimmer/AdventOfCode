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
    [AdventOfCode(Year = 2021, Day = 12)]
    public class Year2021Day12 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2021Day12()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            if (trackTime) SW.Start();                       

            var graph = BuildGraphMap(lines);
            int paths = CountPaths(graph, "start", "end", new HashSet<string> { "start" });

            
            if (trackTime) SW.Stop();

            Console.WriteLine("Part 1: Number of Paths through cave system Part 1: {0}", paths);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            int pathsPart2 = CountSmallCavesVisitTwice(graph, "start", "end", new Dictionary<string, int> { ["start"] = 1 });
            
            if (trackTime) SW.Stop();

            Console.WriteLine("Part 2: Number of Paths through cave system Part 2: {0}", pathsPart2);

            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }       


        private Dictionary<string, List<string>> BuildGraphMap(string[] lines)
        {
            var ans = new Dictionary<string, List<string>>();
            foreach (var line in lines)
            {
                var edge = line.Split("-");
                var (a, b) = (edge[0], edge[1]);
                AddPath(ans, a, b);
                AddPath(ans, b, a);
            }

            return ans;
        }

        private void AddPath(Dictionary<string, List<string>> graph, string src, string dest)
        {
            if (!graph.ContainsKey(src)) graph[src] = new List<string>();
            graph[src].Add(dest);
        }

        private int CountPaths(Dictionary<string, List<string>> graph, string src, string dest,
            HashSet<string> smallCavesSeen)
        {
            if (src.Equals(dest))
            {
                //recursion loop completes when we reach the "end", which will be passed in the "src" field as a neighbour.
                return 1;
            }
            else
            {
                var ans = 0;
                foreach (var neighbor in graph[src].Where(n => !smallCavesSeen.Contains(n)))
                {
                    var newSmallCavesSeen = new HashSet<string>(smallCavesSeen);
                    if (neighbor.All(char.IsLower)) newSmallCavesSeen.Add(neighbor);
                    ans += CountPaths(graph, neighbor, dest, newSmallCavesSeen);
                }

                return ans;
            }
        }

        private int CountSmallCavesVisitTwice(Dictionary<string, List<string>> graph, string src, string dest,
            Dictionary<string, int> smallCavesSeen)
        {
            if (src.Equals(dest))
            {
                //recursion loop completes when we reach the "end", which will be passed in the "src" field as a neighbour.
                return 1;
            }
            else
            {
                var ans = 0;
                foreach (var neighbor in graph[src])
                {
                    if (smallCavesSeen.ContainsKey(neighbor))
                    {
                        //skip processing this neighbour as we visited them twice and it is not "start" or "end"
                        if (smallCavesSeen.Values.Contains(2) || neighbor.Equals("start") || neighbor.Equals("end"))
                            continue;
                    }

                    var newSmallCavesSeen = new Dictionary<string, int>(smallCavesSeen);
                    if (neighbor.All(char.IsLower))
                    {
                        //add appropriate small cave visit count
                        newSmallCavesSeen[neighbor] = (newSmallCavesSeen.ContainsKey(neighbor)) ? 2 : 1;
                    }

                    ans += CountSmallCavesVisitTwice(graph, neighbor, dest, newSmallCavesSeen);
                }

                return ans;
            }

        }
    }
}
