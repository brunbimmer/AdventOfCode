using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using System.Numerics;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 24)]
    public class Year2017Day24 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day24()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        private readonly record struct Component(int First, int Second);
        private readonly record struct Bridge(long Strength, int Length, long LongestStrength);

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            _SW.Restart();
            Bridge result = BuildBridges(FileIOHelper.getInstance().ReadDataAsLines(file));
            _SW.Stop();
            output.WritePart(1, result.Strength, trackTime ? _SW.Elapsed : null, "Strength of the strongest bridge");
            output.WritePart(2, result.LongestStrength, description: "Strength of the strongest longest bridge");
        }

        private Bridge BuildBridges(string[] lines)
        {
            Component[] components = lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line =>
            {
                string[] ports = line.Split('/');
                if (ports.Length != 2) throw new FormatException("A bridge component must contain two ports.");
                var component = new Component(int.Parse(ports[0]), int.Parse(ports[1]));
                if (component.First < 0 || component.Second < 0) throw new FormatException("Port types must be non-negative.");
                return component;
            }).ToArray();
            var connections = new Dictionary<int, List<int>>();
            for (int index = 0; index < components.Length; index++)
            {
                foreach (int port in new[] { components[index].First, components[index].Second }.Distinct())
                {
                    if (!connections.TryGetValue(port, out var choices)) connections.Add(port, choices = new List<int>());
                    choices.Add(index);
                }
            }
            var memo = new Dictionary<(int Port, BigInteger Used), Bridge>();
            Bridge Search(int port, BigInteger used)
            {
                if (memo.TryGetValue((port, used), out Bridge cached)) return cached;
                var best = new Bridge(0, 0, 0);
                if (connections.TryGetValue(port, out var choices))
                {
                    foreach (int index in choices)
                    {
                        BigInteger bit = BigInteger.One << index;
                        if ((used & bit) != 0) continue;
                        Component component = components[index];
                        int nextPort = component.First == port ? component.Second : component.First;
                        Bridge child = Search(nextPort, used | bit);
                        long strength = (long)component.First + component.Second;
                        long strongest = Math.Max(best.Strength, strength + child.Strength);
                        int length = child.Length + 1;
                        long longestStrength = strength + child.LongestStrength;
                        if (length > best.Length || (length == best.Length && longestStrength > best.LongestStrength))
                            best = new Bridge(strongest, length, longestStrength);
                        else
                            best = best with { Strength = strongest };
                    }
                }
                memo[(port, used)] = best;
                return best;
            }
            return Search(0, BigInteger.Zero);
        }
    }
}
