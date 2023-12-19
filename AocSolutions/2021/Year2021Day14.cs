using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2021, Day = 14)]
    public class Year2021Day14 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2021Day14()
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

            Dictionary<string, long> polymerTemplate;
            List<(string, string)> rules;

            _SW.Start();                       

            string originalPolymer = "";
            (polymerTemplate, rules) = ReadData(file, ref originalPolymer);

            long part1 = GrowPolymer(polymerTemplate, rules, 10, originalPolymer);

            
            _SW.Stop();

            Console.WriteLine("Part 1: Difference between most common and least common element after 10 steps: {0}", part1);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part2 = GrowPolymer(polymerTemplate, rules, 40, originalPolymer);
            
            _SW.Stop();

            Console.WriteLine("Part 1: Difference between most common and least common element after 40 steps: {0}", part2);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       

        private (Dictionary<string, long> pairs, List<(string, string)>) ReadData(string filename, ref string originalPolymer)
        {
            var pairs = new Dictionary<string, long>();
            string[] lines = File.ReadAllLines(filename);
            originalPolymer = lines[0];

            for (int i = 0; i < originalPolymer.Length - 1; i++)
            {
                string newPair = originalPolymer.Substring(i, 2);
                if (pairs.ContainsKey(newPair))
                    pairs[newPair] = pairs[newPair] + 1;
                else
                    pairs.Add(newPair, 1);
            }

            var instructions = new List<(string, string)>();

            foreach (var line in lines.Where(x => x.Contains("->")))
            {
                var i = line.Split('-', '>');
                instructions.Add((i[0].Trim(), i[2].Trim())); //split operation odd
            }

            return (pairs, instructions);
        }

        private long GrowPolymer(Dictionary<string, long> polymerPairs, List<(string, string)> instructions, int maxStep, string originalPolymer)
        {
            for (int i = 0; i < maxStep; i++)
            {
                polymerPairs = Polymerize(polymerPairs, instructions);
            }

            Dictionary<char, long> finalCounts = new Dictionary<char, long>();
            
            foreach (KeyValuePair<string, long> entry in polymerPairs)
            {
                finalCounts[entry.Key[0]] = finalCounts.GetValueOrDefault(entry.Key[0], 0) + entry.Value;
            }

            finalCounts[originalPolymer[^1]]++;
            
            long max = finalCounts.Values.Max();
            long min = finalCounts.Values.Min();

            return max - min;
        }

        private Dictionary<string, long> Polymerize(Dictionary<string, long> polymerPairs, List<(string, string)> instructions)
        {
            var newPolymerPairs = new Dictionary<string, long>();

            foreach (KeyValuePair<string, long> entry in polymerPairs)
            {
                string pair = entry.Key;
                long occurrences = entry.Value;

                string insertionElement = instructions.Find(x => x.Item1 == pair).Item2;

                string newPair1 = new string(pair[0] + insertionElement);
                string newPair2 = new string(insertionElement + pair[1]);

                if (newPolymerPairs.ContainsKey(newPair1))
                    newPolymerPairs[newPair1] += occurrences;
                else
                    newPolymerPairs[newPair1] = occurrences;

                if (newPolymerPairs.ContainsKey(newPair2))
                    newPolymerPairs[newPair2] += occurrences;
                else
                    newPolymerPairs[newPair2] = occurrences;
            }

            return newPolymerPairs;
        }
    }
}
