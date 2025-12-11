using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2025, Day = 11)]
    public class Year2025Day11 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day11()
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

            var graph = ParseGraph(input);

            _SW.Stop();
            Console.WriteLine($"  Parsed {graph.Count} devices");
            Console.WriteLine("  Execution Time to Prepare Data: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part1 = SolvePart1(graph);

            _SW.Stop();
            Console.WriteLine($"  Part 1 - Total paths from 'you' to 'out': {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part2 = SolvePart2(graph);

            _SW.Stop();
            Console.WriteLine($"  Part 2 - Paths from 'svr' to 'out' visiting both 'dac' and 'fft': {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private Dictionary<string, List<string>> ParseGraph(string[] lines)
        {
            var graph = new Dictionary<string, List<string>>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(':');
                if (parts.Length != 2)
                    continue;

                string device = parts[0].Trim();
                var outputs = parts[1].Trim().Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                graph[device] = outputs;
            }

            return graph;
        }

        private long SolvePart1(Dictionary<string, List<string>> graph)
        {
            // Count all paths from "you" to "out"
            var pathCount = CountPaths(graph, "you", "out", new HashSet<string>());
            return pathCount;
        }

        private long CountPaths(Dictionary<string, List<string>> graph, string current, string target, HashSet<string> visited)
        {
            // Base case: reached the target
            if (current == target)
                return 1;

            // Mark as visited
            visited.Add(current);

            long totalPaths = 0;

            // Explore all outputs of current device
            if (graph.ContainsKey(current))
            {
                foreach (var next in graph[current])
                {
                    // Only visit unvisited nodes to avoid cycles
                    if (!visited.Contains(next))
                    {
                        totalPaths += CountPaths(graph, next, target, visited);
                    }
                }
            }

            // Backtrack: unmark as visited to allow other paths
            visited.Remove(current);

            return totalPaths;
        }

        private long SolvePart2(Dictionary<string, List<string>> graph)
        {
            // Count paths from "svr" to "out" that visit both "dac" and "fft"
            // Use memoization to avoid recalculating the same subproblems
            var memo = new Dictionary<string, long>();
            var pathCount = CountPathsWithMemo(graph, "svr", "out", new HashSet<string>(), 
                new HashSet<string> { "dac", "fft" }, memo);
            return pathCount;
        }

        private long CountPathsWithMemo(Dictionary<string, List<string>> graph, string current, 
            string target, HashSet<string> visited, HashSet<string> requiredNodes, 
            Dictionary<string, long> memo)
        {
            // Base case: reached the target
            if (current == target)
            {
                // Check if we visited all required nodes
                if (requiredNodes.Count == 0)
                    return 1;
                else
                    return 0;
            }

            // Create a cache key based on current state
            string requiredKey = string.Join(",", requiredNodes.OrderBy(x => x));
            string cacheKey = $"{current}|{requiredKey}";
            
            if (memo.ContainsKey(cacheKey))
                return memo[cacheKey];

            // Mark as visited
            visited.Add(current);

            // If current is a required node, remove it from the set
            bool wasRequired = requiredNodes.Contains(current);
            if (wasRequired)
                requiredNodes.Remove(current);

            long totalPaths = 0;

            // Explore all outputs of current device
            if (graph.ContainsKey(current))
            {
                foreach (var next in graph[current])
                {
                    // Only visit unvisited nodes to avoid cycles
                    if (!visited.Contains(next))
                    {
                        totalPaths += CountPathsWithMemo(graph, next, target, visited, requiredNodes, memo);
                    }
                }
            }

            // Backtrack: unmark as visited and restore required nodes
            visited.Remove(current);
            if (wasRequired)
                requiredNodes.Add(current);

            memo[cacheKey] = totalPaths;
            return totalPaths;
        }
    }
}
