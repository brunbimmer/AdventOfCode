using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 7)]
    public class Year2017Day07 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day07()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        private sealed record ProgramNode(long Weight, string[] Children);

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            _SW.Restart();
            var nodes = ParseTower(FileIOHelper.getInstance().ReadDataAsLines(file));
            var children = nodes.Values.SelectMany(node => node.Children).ToArray();
            string[] roots = nodes.Keys.Except(children, StringComparer.Ordinal).ToArray();
            if (roots.Length != 1 || children.Distinct(StringComparer.Ordinal).Count() != children.Length)
                throw new FormatException("The programs must form a single rooted tree.");
            string root = roots[0];
            _SW.Stop();
            output.WritePart(1, root, trackTime ? _SW.Elapsed : null, "Bottom program in the tower");
            _SW.Restart();
            var totals = new Dictionary<string, long>(StringComparer.Ordinal);
            SumTower(root, nodes, totals, new HashSet<string>(StringComparer.Ordinal));
            if (totals.Count != nodes.Count)
                throw new FormatException("Some programs are disconnected from the tower.");
            long corrected = CorrectWeight(root, null, nodes, totals);
            _SW.Stop();
            output.WritePart(2, corrected, trackTime ? _SW.Elapsed : null, "Corrected weight of the unbalanced program");
        }

        private Dictionary<string, ProgramNode> ParseTower(string[] lines)
        {
            var nodes = new Dictionary<string, ProgramNode>(StringComparer.Ordinal);
            foreach (string line in lines.Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                string[] parts = line.Split("->", 2, StringSplitOptions.TrimEntries);
                string[] head = parts[0].Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (head.Length != 2 || !head[1].StartsWith("(") || !head[1].EndsWith(")"))
                    throw new FormatException("Invalid tower program description.");
                string[] children = parts.Length == 1 ? Array.Empty<string>()
                    : parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                nodes.Add(head[0], new ProgramNode(long.Parse(head[1][1..^1]), children));
            }
            return nodes;
        }

        private long SumTower(string name, Dictionary<string, ProgramNode> nodes, Dictionary<string, long> totals, HashSet<string> active)
        {
            if (!nodes.TryGetValue(name, out ProgramNode node) || !active.Add(name))
                throw new FormatException("The tower contains an undefined program or a cycle.");
            long total = node.Weight;
            foreach (string child in node.Children)
                total = checked(total + SumTower(child, nodes, totals, active));
            active.Remove(name);
            totals.Add(name, total);
            return total;
        }

        private long CorrectWeight(string name, long? expected, Dictionary<string, ProgramNode> nodes, Dictionary<string, long> totals)
        {
            ProgramNode node = nodes[name];
            var groups = node.Children.GroupBy(child => totals[child]).ToArray();
            if (groups.Length > 1)
            {
                var majority = groups.Where(group => group.Count() > 1).ToArray();
                if (groups.Length == 2 && majority.Length == 1)
                {
                    string bad = groups.Single(group => group.Key != majority[0].Key).Single();
                    return CorrectWeight(bad, majority[0].Key, nodes, totals);
                }
                if (expected.HasValue && (expected.Value - node.Weight) % node.Children.Length == 0)
                {
                    long childTotal = (expected.Value - node.Weight) / node.Children.Length;
                    string[] bad = node.Children.Where(child => totals[child] != childTotal).ToArray();
                    if (bad.Length == 1)
                        return CorrectWeight(bad[0], childTotal, nodes, totals);
                }
                throw new InvalidOperationException("The incorrect program weight cannot be uniquely determined.");
            }
            if (expected.HasValue && expected.Value != totals[name])
                return checked(node.Weight + expected.Value - totals[name]);
            throw new InvalidOperationException("The tower is balanced; no unique weight correction is available.");
        }
    }
}
