using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 20)]
    public class Year2023Day20: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private abstract record Module(string Name, List<string> Destinations);
        
        private record FlipFlop(string Name, List<string> Destinations, bool IsOn = false) : Module(Name, Destinations)
        {
            public bool IsOn { get; set; } = IsOn;
        }
        private record Conjunction(string Name, List<string> Destinations, Dictionary<string, bool> Memory = null) : Module(Name, Destinations);
        private record Broadcast(string Name, List<string> Destinations) : Module(Name, Destinations);

        public Year2023Day20()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            long part1 = SolvePart1(lines);

            _SW.Stop();

            Console.WriteLine($"  Part 1: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            long part2 = SolvePart2(lines);

            _SW.Stop();

            Console.WriteLine($"  Part 2: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private long SolvePart1(string[] input)
        {
            var modules = ParseModules(input);
            
            long lowPulses = 0;
            long highPulses = 0;
            
            for (int i = 0; i < 1000; i++)
            {
                var (low, high) = PushButton(modules);
                lowPulses += low;
                highPulses += high;
            }
            
            return lowPulses * highPulses;
        }

        private long SolvePart2(string[] input)
        {
            var modules = ParseModules(input);
            
            // Find the module that sends to 'rx'
            var rxInput = modules.Values.FirstOrDefault(m => m.Destinations.Contains("rx"));
            if (rxInput == null)
                return -1;
            
            // If it's a conjunction, we need to find when all its inputs send high
            if (rxInput is Conjunction conj)
            {
                var cycleLengths = new Dictionary<string, long>();
                var targetInputs = conj.Memory.Keys.ToHashSet();
                
                for (long buttonPresses = 1; buttonPresses <= 10000000; buttonPresses++)
                {
                    SimulateAndTrack(modules, targetInputs, out var highSenders);
                    
                    foreach (var sender in highSenders)
                    {
                        if (!cycleLengths.ContainsKey(sender))
                            cycleLengths[sender] = buttonPresses;
                    }
                    
                    if (cycleLengths.Count == targetInputs.Count)
                        return LCM(cycleLengths.Values.ToList());
                }
            }
            
            return -1;
        }

        private Dictionary<string, Module> ParseModules(string[] input)
        {
            var modules = new Dictionary<string, Module>();
            
            // First pass: create all modules
            foreach (var line in input)
            {
                var parts = line.Split(" -> ");
                var name = parts[0];
                var destinations = parts[1].Split(", ").ToList();
                
                if (name == "broadcaster")
                {
                    modules[name] = new Broadcast(name, destinations);
                }
                else if (name.StartsWith('%'))
                {
                    var cleanName = name.Substring(1);
                    modules[cleanName] = new FlipFlop(cleanName, destinations);
                }
                else if (name.StartsWith('&'))
                {
                    var cleanName = name.Substring(1);
                    modules[cleanName] = new Conjunction(cleanName, destinations, new Dictionary<string, bool>());
                }
            }
            
            // Second pass: initialize conjunction memory
            foreach (var module in modules.Values)
            {
                foreach (var dest in module.Destinations)
                {
                    if (modules.TryGetValue(dest, out var destModule) && destModule is Conjunction conj)
                    {
                        conj.Memory[module.Name] = false;
                    }
                }
            }
            
            return modules;
        }

        private (long, long) PushButton(Dictionary<string, Module> modules)
        {
            var queue = new Queue<(string from, string to, bool isHigh)>();
            queue.Enqueue(("button", "broadcaster", false));
            
            long lowCount = 1;
            long highCount = 0;
            
            while (queue.Count > 0)
            {
                var (from, to, isHigh) = queue.Dequeue();
                
                if (!modules.TryGetValue(to, out var module))
                    continue;
                
                bool? sendPulse = null;
                
                if (module is FlipFlop ff)
                {
                    if (!isHigh)
                    {
                        ff.IsOn = !ff.IsOn;
                        sendPulse = ff.IsOn;
                    }
                }
                else if (module is Conjunction conj)
                {
                    if (conj.Memory.ContainsKey(from))
                        conj.Memory[from] = isHigh;
                    sendPulse = !conj.Memory.Values.All(v => v);
                }
                else if (module is Broadcast)
                {
                    sendPulse = isHigh;
                }
                
                if (sendPulse.HasValue)
                {
                    foreach (var dest in module.Destinations)
                    {
                        queue.Enqueue((module.Name, dest, sendPulse.Value));
                        if (sendPulse.Value)
                            highCount++;
                        else
                            lowCount++;
                    }
                }
            }
            
            return (lowCount, highCount);
        }

        private void SimulateAndTrack(Dictionary<string, Module> modules, HashSet<string> targetInputs, out HashSet<string> highSenders)
        {
            highSenders = new HashSet<string>();
            var queue = new Queue<(string from, string to, bool isHigh)>();
            queue.Enqueue(("button", "broadcaster", false));
            
            while (queue.Count > 0)
            {
                var (from, to, isHigh) = queue.Dequeue();
                
                if (isHigh && targetInputs.Contains(from))
                    highSenders.Add(from);
                
                if (!modules.TryGetValue(to, out var module))
                    continue;
                
                bool? sendPulse = null;
                
                if (module is FlipFlop ff)
                {
                    if (!isHigh)
                    {
                        ff.IsOn = !ff.IsOn;
                        sendPulse = ff.IsOn;
                    }
                }
                else if (module is Conjunction conj)
                {
                    if (conj.Memory.ContainsKey(from))
                        conj.Memory[from] = isHigh;
                    sendPulse = !conj.Memory.Values.All(v => v);
                }
                else if (module is Broadcast)
                {
                    sendPulse = isHigh;
                }
                
                if (sendPulse.HasValue)
                {
                    foreach (var dest in module.Destinations)
                    {
                        queue.Enqueue((module.Name, dest, sendPulse.Value));
                    }
                }
            }
        }

        private long GCD(long a, long b)
        {
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private long LCM(long a, long b)
        {
            return (a / GCD(a, b)) * b;
        }

        private long LCM(List<long> numbers)
        {
            return numbers.Aggregate((a, b) => LCM(a, b));
        }
    }
}
