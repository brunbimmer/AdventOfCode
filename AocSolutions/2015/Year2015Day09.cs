﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using CommonAlgorithms;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 9)]
    public class Year2015Day9Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        
        private Dictionary<Tuple<string, string>, int> locations = new Dictionary<Tuple<string, string>, int>();
        private List<string> allTowns = new List<string>();

        public Year2015Day9Problem()
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

 

            //

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);         
            _SW.Start();
          
            foreach (string line in input)
                AddToMap(line);
            
            long shortestRoute, longestRoute;
                
            (shortestRoute, longestRoute) = ProcessDistancePermutations();


            _SW.Stop();

            Console.WriteLine("  Part 1: Shortest route: {0}", shortestRoute);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            Console.WriteLine("  Part 2: Longest route: {0}", longestRoute);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            


        }

        private void AddToMap(string line)
        {
            string[] sides = line.Split(new []{" = "}, StringSplitOptions.None);

            string lhs = sides[0];
            
            int rhs = Int16.Parse(sides[1]);
            
            string[] towns = lhs.Split(new[] { " to " }, StringSplitOptions.None);

            locations[new Tuple<string, string>(towns[0], towns[1])] = rhs;
            
            locations[new Tuple<string, string>(towns[1], towns[0])] = rhs;

            if (!allTowns.Contains(towns[0]))
                allTowns.Add(towns[0]);

            if (!allTowns.Contains(towns[1]))
                allTowns.Add(towns[1]);

        }

        private (long, long) ProcessDistancePermutations()
        {
            long minTripLength = long.MaxValue;
            long maxTripLength = 0;

            List<List<string>> allPermutations = BuildPermutations(allTowns);
            
            foreach (List<string> thisPermutation in allPermutations)
            {
                long tripLength = 0;
                for (int i = 0; i < thisPermutation.Count - 1; i++)
                    tripLength += locations[new Tuple<string, string>(thisPermutation[i], thisPermutation[i + 1])];

                minTripLength = Math.Min(tripLength, minTripLength);
                maxTripLength = Math.Max(tripLength, maxTripLength);
            }
            //Console.WriteLine("Max: {0}", maxTripLength);

            return (minTripLength, maxTripLength);
        }

        public List<List<string>> BuildPermutations(List<string> items)
        {
            if (items.Count > 1)
            {
                return items.SelectMany(item => BuildPermutations(items.Where(i => !i.Equals(item)).ToList()),
                                       (item, permutation) => new [] { item }.Concat(permutation).ToList()).ToList();
            }

            return new List<List<string>> { items };
        }
    }
}
