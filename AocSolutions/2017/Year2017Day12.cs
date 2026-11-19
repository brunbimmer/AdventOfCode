using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 12)]
    public class Year2017Day12 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day12()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            _SW.Restart();
            var result = CountGroups(FileIOHelper.getInstance().ReadDataAsLines(file));
            _SW.Stop();
            output.WritePart(1, result.GroupZero, trackTime ? _SW.Elapsed : null, "Programs connected to program zero");
            output.WritePart(2, result.Groups, description: "Number of connected program groups");
        }

        private (int GroupZero, int Groups) CountGroups(string[] lines)
        {
            var graph = new Dictionary<int, HashSet<int>>();
            foreach (string line in lines.Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                string[] parts = line.Split("<->", StringSplitOptions.TrimEntries);
                if (parts.Length != 2) throw new FormatException("Invalid pipe connection.");
                int node = int.Parse(parts[0]);
                graph.TryAdd(node, new HashSet<int>());
                foreach (int neighbor in parts[1].Split(',', StringSplitOptions.TrimEntries).Select(int.Parse))
                {
                    graph.TryAdd(neighbor, new HashSet<int>());
                    graph[node].Add(neighbor);
                    graph[neighbor].Add(node);
                }
            }
            var visited = new HashSet<int>();
            int groups = 0;
            int groupZero = 0;
            foreach (int start in graph.Keys)
            {
                if (!visited.Add(start)) continue;
                groups++;
                var pending = new Queue<int>();
                pending.Enqueue(start);
                int size = 0;
                bool containsZero = false;
                while (pending.Count > 0)
                {
                    int node = pending.Dequeue();
                    size++;
                    containsZero |= node == 0;
                    foreach (int neighbor in graph[node])
                        if (visited.Add(neighbor)) pending.Enqueue(neighbor);
                }
                if (containsZero) groupZero = size;
            }
            return (groupZero, groups);
        }
    }
}
