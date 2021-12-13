using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuzzleMain
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Count() == 1)
                {
                    var filename = args[0];
                    Program.Run(filename).Wait();
                }
                else
                {
                    Console.WriteLine("Invalid Arguments. Please specify input filename only.");
                    Console.WriteLine("   DailyPuzzle input.txt");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        private static async Task Run(string filename)
        {
            //Step 1: Read in the file
            Console.WriteLine("Loading Input file");
            var graph = BuildGraphMap(filename);
            int paths = CountPaths(graph, "start", "end", new HashSet<string> { "start" });
            int pathsPart2 =
                CountSmallCavesVisitTwice(graph, "start", "end", new Dictionary<string, int> { ["start"] = 1 });
            Console.WriteLine("Part 1: Number of Paths through cave system Part 1: {0}", paths);
            Console.WriteLine("Part 2: Number of Paths through cave system Part 2: {0}", pathsPart2);
        }

        private static Dictionary<string, List<string>> BuildGraphMap(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
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

        private static void AddPath(Dictionary<string, List<string>> graph, string src, string dest)
        {
            if (!graph.ContainsKey(src)) graph[src] = new List<string>();
            graph[src].Add(dest);
        }

        private static int CountPaths(Dictionary<string, List<string>> graph, string src, string dest,
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

        private static int CountSmallCavesVisitTwice(Dictionary<string, List<string>> graph, string src, string dest,
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
