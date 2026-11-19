using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 19)]
    public class Year2020Day19 : IAdventOfCode
    {
        private sealed record MessageRule(char? Literal, int[][] Alternatives);

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day19()
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
            var input = ParseInput(lines);

            _SW.Restart();
            int validMessages = Part1(input.Rules, input.Messages);
            _SW.Stop();

            output.WritePart(1, validMessages, trackTime ? _SW.Elapsed : null, "Messages matching rule 0");


            _SW.Restart();
            validMessages = Part2(input.Rules, input.Messages);
            _SW.Stop();

            output.WritePart(2, validMessages, trackTime ? _SW.Elapsed : null, "Messages matching the updated recursive rules");

        }

        private (Dictionary<int, MessageRule> Rules, string[] Messages) ParseInput(string[] lines)
        {
            int separator = Array.FindIndex(lines, string.IsNullOrWhiteSpace);
            if (separator < 0)
                throw new FormatException("Rules and messages must be separated by a blank line.");

            var rules = new Dictionary<int, MessageRule>();
            foreach (string line in lines.Take(separator))
            {
                string[] parts = line.Split(':', 2);
                int id = int.Parse(parts[0]);
                string body = parts[1].Trim();
                if (body[0] == '"')
                {
                    rules.Add(id, new MessageRule(body[1], Array.Empty<int[]>()));
                }
                else
                {
                    int[][] alternatives = body.Split('|').Select(sequence => sequence
                        .Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()).ToArray();
                    rules.Add(id, new MessageRule(null, alternatives));
                }
            }

            string[] messages = lines.Skip(separator + 1).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            return (rules, messages);
        }

        private int Part1(Dictionary<int, MessageRule> rules, string[] messages)
        {
            return messages.Count(message => MatchesMessage(message, rules));
        }

        private int Part2(Dictionary<int, MessageRule> rules, string[] messages)
        {
            var updatedRules = new Dictionary<int, MessageRule>(rules)
            {
                [8] = new MessageRule(null, new[] { new[] { 42 }, new[] { 42, 8 } }),
                [11] = new MessageRule(null, new[] { new[] { 42, 31 }, new[] { 42, 11, 31 } })
            };

            // Each recursive branch consumes rule 42 first, so the message bounds recursion.
            return Part1(updatedRules, messages);
        }

        private bool MatchesMessage(string message, Dictionary<int, MessageRule> rules)
        {
            var memo = new Dictionary<(int Rule, int Position), HashSet<int>>();

            HashSet<int> Match(int ruleId, int position)
            {
                var key = (ruleId, position);
                if (memo.TryGetValue(key, out HashSet<int> cached))
                    return cached;

                HashSet<int> ends = new HashSet<int>();
                if (position >= message.Length)
                {
                    memo[key] = ends;
                    return ends;
                }

                MessageRule rule = rules[ruleId];
                if (rule.Literal.HasValue)
                {
                    if (message[position] == rule.Literal.Value)
                        ends.Add(position + 1);
                }
                else
                {
                    foreach (int[] sequence in rule.Alternatives)
                    {
                        HashSet<int> positions = new HashSet<int> { position };
                        foreach (int subRule in sequence)
                        {
                            HashSet<int> next = new HashSet<int>();
                            foreach (int start in positions)
                                next.UnionWith(Match(subRule, start));

                            positions = next;
                            if (positions.Count == 0)
                                break;
                        }

                        ends.UnionWith(positions);
                    }
                }

                memo[key] = ends;
                return ends;
            }

            return Match(0, 0).Contains(message.Length);
        }
    }
}
