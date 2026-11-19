using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 16)]
    public class Year2020Day16 : IAdventOfCode
    {
        private sealed record FieldRule(string Name, (int Minimum, int Maximum)[] Ranges)
        {
            public bool Accepts(int value) => Ranges.Any(range => value >= range.Minimum && value <= range.Maximum);
        }

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day16()
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
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
            var notes = ParseNotes(lines);

            _SW.Restart();
            long errorRate = Part1(notes.Rules, notes.NearbyTickets);
            _SW.Stop();

            output.WritePart(1, errorRate, trackTime ? _SW.Elapsed : null, "Ticket scanning error rate");


            _SW.Restart();
            long product = Part2(notes.Rules, notes.YourTicket, notes.NearbyTickets);
            _SW.Stop();

            output.WritePart(2, product, trackTime ? _SW.Elapsed : null, "Product of departure fields on your ticket");

        }

        private (FieldRule[] Rules, int[] YourTicket, int[][] NearbyTickets) ParseNotes(string[] lines)
        {
            var rules = lines.TakeWhile(line => !string.IsNullOrWhiteSpace(line)).Select(line =>
            {
                string[] parts = line.Split(':', 2);
                var ranges = parts[1].Split(" or ", StringSplitOptions.TrimEntries).Select(range =>
                {
                    string[] bounds = range.Split('-');
                    return (Minimum: int.Parse(bounds[0]), Maximum: int.Parse(bounds[1]));
                }).ToArray();
                return new FieldRule(parts[0], ranges);
            }).ToArray();

            int yourStart = Array.IndexOf(lines, "your ticket:");
            int nearbyStart = Array.IndexOf(lines, "nearby tickets:");
            if (yourStart < 0 || nearbyStart < 0)
                throw new FormatException("The notes must contain your ticket and nearby tickets sections.");

            int[] yourTicket = ParseTicket(lines[yourStart + 1]);
            int[][] nearbyTickets = lines.Skip(nearbyStart + 1)
                .Where(line => !string.IsNullOrWhiteSpace(line)).Select(ParseTicket).ToArray();
            return (rules, yourTicket, nearbyTickets);
        }

        private int[] ParseTicket(string ticket)
        {
            return Array.ConvertAll(ticket.Split(','), int.Parse);
        }

        private long Part1(FieldRule[] rules, int[][] nearbyTickets)
        {
            long errorRate = 0;
            foreach (int[] ticket in nearbyTickets)
            {
                foreach (int value in ticket)
                {
                    if (!rules.Any(rule => rule.Accepts(value)))
                        errorRate = checked(errorRate + value);
                }
            }

            return errorRate;
        }

        private long Part2(FieldRule[] rules, int[] yourTicket, int[][] nearbyTickets)
        {
            string[] fieldOrder = DetermineFieldOrder(rules, yourTicket, nearbyTickets);
            long product = 1;
            for (int column = 0; column < fieldOrder.Length; column++)
            {
                if (fieldOrder[column].StartsWith("departure", StringComparison.Ordinal))
                    product = checked(product * yourTicket[column]);
            }

            return product;
        }

        private string[] DetermineFieldOrder(FieldRule[] rules, int[] yourTicket, int[][] nearbyTickets)
        {
            int[][] validTickets = nearbyTickets
                .Where(ticket => ticket.All(value => rules.Any(rule => rule.Accepts(value))))
                .Append(yourTicket).ToArray();
            if (yourTicket.Length != rules.Length || validTickets.Any(ticket => ticket.Length != rules.Length))
                throw new FormatException("Each valid ticket must have one value per field rule.");

            var candidates = new HashSet<int>[rules.Length];
            for (int column = 0; column < rules.Length; column++)
            {
                candidates[column] = Enumerable.Range(0, rules.Length)
                    .Where(rule => validTickets.All(ticket => rules[rule].Accepts(ticket[column])))
                    .ToHashSet();
            }

            string[] fieldOrder = new string[rules.Length];
            for (int resolved = 0; resolved < rules.Length; resolved++)
            {
                int column = Array.FindIndex(candidates, options => options.Count == 1);
                if (column < 0)
                    throw new InvalidOperationException("The field order cannot be uniquely determined.");

                int rule = candidates[column].Single();
                fieldOrder[column] = rules[rule].Name;
                foreach (var options in candidates)
                    options.Remove(rule);
            }

            return fieldOrder;
        }
    }
}
