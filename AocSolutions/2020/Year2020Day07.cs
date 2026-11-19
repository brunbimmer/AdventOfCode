using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 7)]
    public class Year2020Day07 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day07()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] rules = FileIOHelper.getInstance().ReadDataAsLines(file);
            var bags = ParseRules(rules);

            _SW.Restart();
            int colors = Part1(bags);
            _SW.Stop();

            output.WritePart(1, colors, trackTime ? _SW.Elapsed : null, "Bag colors that can contain shiny gold");


            _SW.Restart();
            long total = Part2(bags);
            _SW.Stop();

            output.WritePart(2, total, trackTime ? _SW.Elapsed : null, "Bags required inside shiny gold");

        }

        private Dictionary<string, List<(string Color, int Count)>> ParseRules(string[] rules)
        {
            var bags = new Dictionary<string, List<(string Color, int Count)>>();

            foreach (string rule in rules)
            {
                string[] parts = rule.Split(" bags contain ", 2);
                var contents = new List<(string Color, int Count)>();

                foreach (Match match in Regex.Matches(parts[1], @"([0-9]+) ([a-z]+ [a-z]+) bags?"))
                {
                    contents.Add((match.Groups[2].Value, int.Parse(match.Groups[1].Value)));
                }

                bags[parts[0]] = contents;
            }

            return bags;
        }

        private int Part1(Dictionary<string, List<(string Color, int Count)>> bags)
        {
            Dictionary<string, List<string>> containers = new Dictionary<string, List<string>>();

            foreach (var bag in bags)
            {
                foreach (var content in bag.Value)
                {
                    if (!containers.TryGetValue(content.Color, out List<string> outerColors))
                    {
                        outerColors = new List<string>();
                        containers[content.Color] = outerColors;
                    }

                    outerColors.Add(bag.Key);
                }
            }

            HashSet<string> visited = new HashSet<string> { "shiny gold" };
            Queue<string> pending = new Queue<string>();
            pending.Enqueue("shiny gold");

            while (pending.Count > 0)
            {
                string color = pending.Dequeue();
                if (!containers.TryGetValue(color, out List<string> outerColors))
                    continue;

                foreach (string outerColor in outerColors)
                {
                    if (visited.Add(outerColor))
                        pending.Enqueue(outerColor);
                }
            }

            return visited.Count - 1;
        }

        private long Part2(Dictionary<string, List<(string Color, int Count)>> bags)
        {
            Dictionary<string, long> totals = new Dictionary<string, long>();
            HashSet<string> visiting = new HashSet<string>();

            long CountBags(string color)
            {
                if (totals.TryGetValue(color, out long cachedTotal))
                    return cachedTotal;

                if (!visiting.Add(color))
                    throw new InvalidOperationException("Bag containment rules contain a cycle.");

                long total = 0;
                foreach (var content in bags[color])
                {
                    // Each contained bag contributes itself and all bags inside it.
                    total = checked(total + content.Count * (1 + CountBags(content.Color)));
                }

                visiting.Remove(color);
                totals[color] = total;
                return total;
            }

            return CountBags("shiny gold");
        }
    }
}
