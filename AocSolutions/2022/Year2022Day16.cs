using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    [AdventOfCode(Year = 2022, Day = 16)]
    public class Year2022Day16 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2022Day16()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        internal class Valve
        {
            public string Name { get; set; }
            public int Flowrate { get; set; }
            public List<string> Neighbors { get; set; }
        }

        Dictionary<string, Valve> Valves = new();       // Valves keyed by name.
        int[,] impDists;                                // The important distances (that is dists between non-zero flow + AA)
        List<string> impValves;                // Non-zero Flow and AA
        int[] valveMasks;                               // Ints with 1 bit turned on


        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
           
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            Initialize(input);

            Dictionary<int, int> cache = new();
            Visit(0, 30, 0, 0, cache);

            SW.Stop();

            Console.WriteLine("Part 1: {0}, Execution Time: {1}", cache.Values.Max(), StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

           
            
            SW.Stop();

            //Console.WriteLine("Part 2: {0}, Execution Time: {1}", result2, StopwatchUtil.getInstance().GetTimestamp(SW));


        }    
        
        private void Initialize(string[] input)
        {
            int[,] dists;
            List<string> ValveList;

            foreach (var l in input) //Read and parse input
            {
                var c = Regex.Matches(l, "([A-Z]{2}|\\d+)").ToList();

                Valve newValve = new()
                {
                    Name = c[0].Value,
                    Flowrate = int.Parse(c[1].Value),
                    Neighbors = new(c.Skip(2).Select(a => a.Value)),
                };
                Valves[c[0].Value] = newValve;
            }

            //Floyd Warshall Algorithm

            ValveList = Valves.Values.OrderBy(a => a.Name).Select(a => a.Name).ToList();

            dists = new int[Valves.Count, Valves.Count];
            for (int i = 0; i < ValveList.Count; i++) //Fill in the default values
            {
                for (int j = i; j < ValveList.Count; j++)
                {
                    if (i == j) dists[i, j] = 0;
                    else if (Valves[ValveList[i]].Neighbors.Contains(ValveList[j]))
                    {
                        dists[i, j] = dists[j, i] = 1;
                    }
                    else
                    {
                        dists[i, j] = dists[j, i] = 9999; //Don't use int.MaxValue here because we need to do some additions.
                    }
                }
            }

            for (int k = 0; k < ValveList.Count; k++)
            {
                for (int i = 0; i < ValveList.Count; i++)
                {
                    for (int j = i + 1; j < ValveList.Count; j++)
                    {
                        if (dists[i, k] + dists[k, j] < dists[i, j])
                            dists[i, j] = dists[j, i] = dists[i, k] + dists[k, j];
                    }
                }
            }

            //Only care about AA and the ones that generate flow.

            impValves = ValveList.Where(a => a == "AA" || Valves[a].Flowrate != 0).ToList();
            List<int> indices = new();

            for (int i = 0; i < ValveList.Count; i++) if (Valves[ValveList[i]].Flowrate == 0 && ValveList[i] != "AA") indices.Add(i);
            indices.Reverse();

            impDists = dists;
            foreach(var i in indices)
            {
                impDists = impDists.TrimArray(i, i);
            }

            valveMasks = new int[impDists.GetLength(0)];
            for (int i = 0; i < valveMasks.Length; i++) valveMasks[i] = 1 << i; 


        }



        /// <summary>
        /// Recursively check all paths to find all the possible outputs.
        /// </summary>
        /// <param name="node">Node you are at, specifically teh index of the name from impNodes</param>
        /// <param name="time">Time remaining</param>
        /// <param name="state">int/bitmask of valves that are turned on</param>
        /// <param name="flow">Total flow (calculated from each valve as its turned on)</param>
        /// <param name="cache">Key is state (see above) value is max flow achieved in that state</param>
        private void Visit(int node, int time, int state, int flow, Dictionary<int, int> cache)
        {
            //cache[state] = int.Max(cache.GetValueOrDefault(state, 0), flow); //Are we at a better point with the current valves turned on than last time we were at this point? if so, update value
            //for(int i = 0; i < impValves.Count; i++) //For all valves
            //{
            //    var newTime = time - impDists[node, i] - 1; //time remaining is time minus walking time, minus 1 minute to open valve
            //    if ((valveMasks[i] & state) != 0 || newTime <= 0) continue; //Don't go to the same valve twice, don't go to a valve if it means we run out of time.
            //    Visit(i, newTime, state | valveMasks[i], flow + (newTime * Valves[impValves[i]].Flowrate), cache);
            //    //Go to new valve, update state so it's turned on, add it's flow, repeat everything above.
            //}
        }
    }
}
