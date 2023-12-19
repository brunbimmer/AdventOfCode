using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventFileIO;
using Common;
using static Common.Utilities;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 19)]
    public class Year2023Day19 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day19()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new();
        }


        private Dictionary<string, WorkFlow> workflows = new Dictionary<string, WorkFlow>();
        private List<XmasPart> Parts = new();

        Regex re = new("^(?'wfName'.+){(?'steps'.*),(?'defaultTarget'.+)}");

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string input = FileIOHelper.getInstance().ReadDataAsString(file);

            ParseInput(input);

            _SW.Start();
            long result = SolvePartOne();
            _SW.Stop();

            Console.WriteLine($"  Part 1: Sum of all Accepted parts: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Restart();
            
            _SW.Start();
            result = SolvePartTwo();
            _SW.Stop();

            Console.WriteLine($"  Part 2: Distinct combinations of ratings : {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        private void ParseInput(string input)
        {
            //finally created a helper method that can split the Advent of Code input into two halves based on a empty line
            //separator. I have seem to many puzzles like this, so finally wrote a method to do exactly this.
            var inHalves = input.SplitByDoubleNewline();

            //Process each half into their data structures (based on private classes I created below).
            foreach (var a in inHalves[0].SplitByNewline())
            {
                var wfParts = re.Match(a);

                WorkFlow wf = new()
                {
                    name = wfParts.Groups["wfName"].Value,
                    defaultTarget = wfParts.Groups["defaultTarget"].Value
                };

                foreach (var n in wfParts.Groups["steps"].Value.Split(','))
                {
                    var section = n[0];
                    var comparator = n[1];
                    var compValue = n.ExtractInts().First();
                    var target = n.Split(":")[^1];

                    wf.commands.Add((section, comparator, compValue, target));
                }

                workflows[wf.name] = wf;
            }

            //Process the Xmas part values into a data structure
            foreach (var p in inHalves[1].SplitByNewline())
            {
                var vals = p.ExtractInts().ToList();
                XmasPart part = new()
                {
                    x = vals[0],
                    m = vals[1],
                    a = vals[2],
                    s = vals[3]
                };

                Parts.Add(part);
            }
        }

        protected long SolvePartOne()
        {
            return Parts.Sum(a => workflows["in"].IsAccepted(a, this) ? a.Value : 0);
        }

        protected long SolvePartTwo()
        {
            //Added some additional assist classes since I know we will be doing range
            //stuff. Just never did it before. This is a dictionary of ranges which fits
            //into what we are trying to achieve in part 2.
            DictMultiRange<char> startRanges = new()
            {
                Ranges = new()
                {
                    {'x', new(1, 4000) },
                    {'m', new(1, 4000) },
                    {'a', new(1, 4000) },
                    {'s', new(1, 4000) }
                }
            };
            return GetTotalNumOfDistinctCombos(startRanges, workflows["in"]);
        }

        private long GetTotalNumOfDistinctCombos(DictMultiRange<char> ranges, WorkFlow startFlow)
        {
            long validCombos = 0;
            
            //now we run each range value through the workflow. Recursion is king here. 
            foreach (var step in startFlow.commands)
            {
                
                DictMultiRange<char> newRange = new(ranges);

                if (step.comparator == '>')
                {
                    
                    if (ranges.Ranges[step.section].End > step.compValue) //Do we have any valid points
                    {
                        //Cut out the invalid range and only work with the valid range for this input.
                        newRange.Ranges[step.section].Start = Math.Max(newRange.Ranges[step.section].Start, step.compValue + 1);
                        
                        if (step.target == "A") 
                            validCombos += newRange.Length;
                        else if (step.target != "R") 
                            validCombos += GetTotalNumOfDistinctCombos(newRange, workflows[step.target]);

                        //Take the invalid values and pass them to the next step in the workflow.
                        ranges.Ranges[step.section].End = step.compValue;
                    }

                }
                if (step.comparator == '<')
                {
                    if (ranges.Ranges[step.section].Start < step.compValue) //Do we have any valid points
                    {
                        //Cut out the invalid range and only work with the valid range for this input.
                        newRange.Ranges[step.section].End = Math.Min(newRange.Ranges[step.section].End, step.compValue - 1);
                        
                        if (step.target == "A") 
                            validCombos += newRange.Length;
                        else if (step.target != "R") 
                            validCombos += GetTotalNumOfDistinctCombos(newRange, workflows[step.target]);

                        //Take the invalid values and pass them to the next step in the workflow.
                        ranges.Ranges[step.section].Start = step.compValue;
                    }

                }
            }

            if (startFlow.defaultTarget == "A")
            {
                validCombos += ranges.Length;
            }
            else if (startFlow.defaultTarget != "R")
            {
                validCombos += GetTotalNumOfDistinctCombos(ranges, workflows[startFlow.defaultTarget]);
            }

            return validCombos;
        }

        //This represents the Xmas Part.
        private class XmasPart
        {
            public int x;
            public int m;
            public int a;
            public int s;

            public int Value => x + m + a + s;
        }

        //Build a class to assist with running through the workflow. The IsPartAccepted method is recursive
        private class WorkFlow
        {
            public string name;
            public readonly List<(char section, char comparator, int compValue, string target)> commands = new();
            public string defaultTarget;

            //This is a recursive call and will exit once we get a "A" or "R" result.
            public bool IsAccepted(XmasPart part, Year2023Day19 thisDay)
            {
                foreach (var command in commands)
                {
                    (char section, char comparator, int compValue, string target) = command;

                    int checkVal = (section) switch
                    {
                        'x' => part.x,
                        'm' => part.m,
                        'a' => part.a,
                        's' => part.s,
                        _ => throw new ArgumentException()
                    };

                    bool compResult = (comparator) switch
                    {
                        '<' => checkVal < compValue,
                        '>' => checkVal > compValue,
                        _ => throw new ArgumentException()
                    };

                    if (!compResult) continue;

                    return (target) switch
                    {
                        "R" => false,
                        "A" => true,
                        _ => thisDay.workflows[target].IsAccepted(part, thisDay)
                    };
                }

                return (defaultTarget) switch
                {
                    "R" => false,
                    "A" => true,
                    _ => thisDay.workflows[defaultTarget].IsAccepted(part, thisDay)
                };
            }
        }
    }
}