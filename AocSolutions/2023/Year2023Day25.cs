using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 25)]
    public class Year2023Day25: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day25()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            var graph = ParseGraph(lines);

            _SW.Start();

            long part1 = SolvePart1(graph);

            _SW.Stop();

            Console.WriteLine($"  Part 1: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
        }

        private Dictionary<string, HashSet<string>> ParseGraph(string[] lines)
        {
            var graph = new Dictionary<string, HashSet<string>>();
            
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                string node = parts[0].Trim();
                var neighbors = parts[1].Trim().Split(' ').Select(s => s.Trim()).ToArray();
                
                if (!graph.ContainsKey(node))
                    graph[node] = new HashSet<string>();
                
                foreach (var neighbor in neighbors)
                {
                    graph[node].Add(neighbor);
                    
                    if (!graph.ContainsKey(neighbor))
                        graph[neighbor] = new HashSet<string>();
                    graph[neighbor].Add(node);
                }
            }
            
            return graph;
        }

        private long SolvePart1(Dictionary<string, HashSet<string>> graph)
        {
            // Use edge frequency from random walks to identify the 3 cut edges
            var edgeFrequency = new Dictionary<(string, string), int>();
            var nodes = graph.Keys.ToList();
            var random = new Random(42);
            
            // Multiple attempts with different random pairs to find cut edges
            for (int attempt = 0; attempt < 50; attempt++)
            {
                int idx1 = random.Next(nodes.Count);
                int idx2 = random.Next(nodes.Count);
                
                if (idx1 == idx2)
                    continue;
                
                var path = BFS(graph, nodes[idx1], nodes[idx2]);
                if (path == null)
                    continue;
                
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var key = (string.Compare(a, b) < 0) ? (a, b) : (b, a);
                    
                    if (!edgeFrequency.ContainsKey(key))
                        edgeFrequency[key] = 0;
                    edgeFrequency[key]++;
                }
            }
            
            // Get top candidate edges (much fewer to test)
            var topCandidates = edgeFrequency.OrderByDescending(x => x.Value).Take(10).Select(x => x.Key).ToList();
            
            // Test only combinations of top candidates
            for (int i = 0; i < topCandidates.Count; i++)
            {
                for (int j = i + 1; j < topCandidates.Count; j++)
                {
                    for (int k = j + 1; k < topCandidates.Count; k++)
                    {
                        var testGraph = CloneGraph(graph);
                        var edges = new[] { topCandidates[i], topCandidates[j], topCandidates[k] };
                        
                        foreach (var edge in edges)
                        {
                            testGraph[edge.Item1].Remove(edge.Item2);
                            testGraph[edge.Item2].Remove(edge.Item1);
                        }
                        
                        var components = FindConnectedComponents(testGraph);
                        if (components.Count == 2)
                        {
                            return (long)components[0].Count * components[1].Count;
                        }
                    }
                }
            }
            
            return 0;
        }

        private List<string> BFS(Dictionary<string, HashSet<string>> graph, string start, string end)
        {
            var queue = new Queue<string>();
            var visited = new HashSet<string>();
            var parent = new Dictionary<string, string>();
            
            queue.Enqueue(start);
            visited.Add(start);
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                if (current == end)
                {
                    var path = new List<string>();
                    string node = end;
                    while (node != null)
                    {
                        path.Add(node);
                        parent.TryGetValue(node, out node);
                    }
                    path.Reverse();
                    return path;
                }
                
                foreach (var neighbor in graph[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        parent[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }
            
            return null;
        }

        private List<List<string>> FindConnectedComponents(Dictionary<string, HashSet<string>> graph)
        {
            var visited = new HashSet<string>();
            var components = new List<List<string>>();
            
            foreach (var node in graph.Keys)
            {
                if (!visited.Contains(node))
                {
                    var component = new List<string>();
                    var queue = new Queue<string>();
                    queue.Enqueue(node);
                    visited.Add(node);
                    
                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        component.Add(current);
                        
                        foreach (var neighbor in graph[current])
                        {
                            if (!visited.Contains(neighbor))
                            {
                                visited.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                        }
                    }
                    
                    components.Add(component);
                }
            }
            
            return components;
        }

        private Dictionary<string, HashSet<string>> CloneGraph(Dictionary<string, HashSet<string>> graph)
        {
            var clone = new Dictionary<string, HashSet<string>>();
            foreach (var kvp in graph)
            {
                clone[kvp.Key] = new HashSet<string>(kvp.Value);
            }
            return clone;
        }
    }
}
