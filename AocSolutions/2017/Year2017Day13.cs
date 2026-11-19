using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using System.Numerics;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 13)]
    public class Year2017Day13 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day13()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        private readonly record struct Layer(long Depth, long Range)
        {
            public long Period => 2 * (Range - 1);
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            Layer[] layers = ParseLayers(FileIOHelper.getInstance().ReadDataAsLines(file));
            _SW.Restart();
            long severity = layers.Where(layer => layer.Range == 1 || layer.Depth % layer.Period == 0)
                .Sum(layer => checked(layer.Depth * layer.Range));
            _SW.Stop();
            output.WritePart(1, severity, trackTime ? _SW.Elapsed : null, "Severity of crossing the firewall immediately");
            _SW.Restart();
            long delay = SafeDelay(layers);
            _SW.Stop();
            output.WritePart(2, delay, trackTime ? _SW.Elapsed : null, "Smallest delay that avoids every scanner");
        }

        private Layer[] ParseLayers(string[] lines)
        {
            return lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line =>
            {
                string[] parts = line.Split(':', StringSplitOptions.TrimEntries);
                if (parts.Length != 2) throw new FormatException("Invalid firewall layer.");
                var layer = new Layer(long.Parse(parts[0]), long.Parse(parts[1]));
                if (layer.Depth < 0 || layer.Range < 1 || layer.Range > long.MaxValue / 2)
                    throw new FormatException("Firewall depths must be non-negative and ranges must be positive.");
                return layer;
            }).OrderBy(layer => layer.Range).ToArray();
        }

        private long SafeDelay(Layer[] layers)
        {
            if (layers.Any(layer => layer.Range == 1))
                throw new InvalidOperationException("A range-one scanner cannot be avoided.");
            BigInteger cycle = BigInteger.One;
            foreach (Layer layer in layers)
                cycle = cycle / BigInteger.GreatestCommonDivisor(cycle, layer.Period) * layer.Period;
            for (long delay = 0; delay < cycle; delay = checked(delay + 1))
            {
                bool caught = false;
                foreach (Layer layer in layers)
                {
                    if (delay % layer.Period == (layer.Period - layer.Depth % layer.Period) % layer.Period)
                    {
                        caught = true;
                        break;
                    }
                }
                if (!caught) return delay;
            }
            throw new InvalidOperationException("No delay can avoid all the scanners.");
        }
    }
}
