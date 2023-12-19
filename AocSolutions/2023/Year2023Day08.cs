using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 8)]
    public class Year2023Day08 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day08()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        private readonly record struct Node(string Id, string Left, string Right);

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            var directions = input[0];
            var nodes = input[2..].Select(ParseNode).ToDictionary(n => n.Id);

            _SW.Start();

            long steps = Travel(directions, nodes);

            Console.WriteLine($"  Part 1: Number of steps to reach destination: {steps}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");


            _SW.Stop();

            _SW.Reset();
            _SW.Start();

            steps = GhostlyTravels(directions, nodes);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Number of steps to reach destination as a ghost: {steps}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");
        }

        private long Travel(string directions, IDictionary<string, Node> nodes)
        {
            return TraverseMap(directions, nodes, start: "AAA", stop: id => id == "ZZZ");
        }

        private long GhostlyTravels(string directions, IDictionary<string, Node> nodes)
        {
            var stop = (string id) => id.EndsWith('Z');
            var cycles = nodes.Keys
                                        .Where(id => id.EndsWith('A'))
                                        .Select(id => TraverseMap(directions, nodes, start: id, stop))
                                        .ToList();

            return Numerics.Lcm(cycles);
        }

        private long TraverseMap(string directions, IDictionary<string, Node> nodes, string start, Func<string, bool> stop)
        {
            var i = 0L;
            var pos = nodes[start];

            while (!stop.Invoke(pos.Id))
            {
                pos = directions[(int)(i++ % directions.Length)] switch
                {
                    'L' => nodes[pos.Left],
                    'R' => nodes[pos.Right]
                };
            }

            return i;
        }

        private Node ParseNode(string line)
        {
            var match = Regex.Match(line, @"(?<I>[1-9A-Z]+) = \((?<L>[1-9A-Z]+), (?<R>[1-9A-Z]+)\)");
            return new Node(
                Id: match.Groups["I"].Value,
                Left: match.Groups["L"].Value,
                Right: match.Groups["R"].Value);
        }
    }

}
