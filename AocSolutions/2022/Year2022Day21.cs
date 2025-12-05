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
    [AdventOfCode(Year = 2022, Day = 21)]
    public class Year2022Day21 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public class Monkey
        {
            public string Name { get; set; }
            public long? Value { get; set; }
            public string Left { get; set; }
            public string Right { get; set; }
            public char Operation { get; set; }
        }

        public Year2022Day21()
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
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            var monkeys = ParseMonkeys(input);

            _SW.Start();

            long rootValue = ResolveMonkey("root", monkeys, new Dictionary<string, long>());

            _SW.Stop();

            Console.WriteLine($"  Part 1: Root Yells: {rootValue}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            // Reset for Part 2
            monkeys = ParseMonkeys(input);
            long humanValue = FindHumanValue(monkeys);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Human Value: {humanValue}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        private Dictionary<string, Monkey> ParseMonkeys(string[] input)
        {
            var monkeys = new Dictionary<string, Monkey>();

            foreach (string line in input)
            {
                var parts = line.Split(": ");
                string name = parts[0];
                string job = parts[1];

                var monkey = new Monkey { Name = name };

                // Check if it's a number
                if (long.TryParse(job, out long value))
                {
                    monkey.Value = value;
                }
                else
                {
                    // Parse operation: "left op right"
                    var operationMatch = Regex.Match(job, @"(\w+)\s([\+\-\*/])\s(\w+)");
                    if (operationMatch.Success)
                    {
                        monkey.Left = operationMatch.Groups[1].Value;
                        monkey.Operation = operationMatch.Groups[2].Value[0];
                        monkey.Right = operationMatch.Groups[3].Value;
                    }
                }

                monkeys[name] = monkey;
            }

            return monkeys;
        }

        public long ResolveMonkey(string name, Dictionary<string, Monkey> monkeys, Dictionary<string, long> cache)
        {
            if (cache.ContainsKey(name))
                return cache[name];

            var monkey = monkeys[name];

            if (monkey.Value.HasValue)
            {
                cache[name] = monkey.Value.Value;
                return monkey.Value.Value;
            }

            long leftVal = ResolveMonkey(monkey.Left, monkeys, cache);
            long rightVal = ResolveMonkey(monkey.Right, monkeys, cache);

            long result = monkey.Operation switch
            {
                '+' => leftVal + rightVal,
                '-' => leftVal - rightVal,
                '*' => leftVal * rightVal,
                '/' => leftVal / rightVal,
                _ => 0
            };

            cache[name] = result;
            return result;
        }

        public long FindHumanValue(Dictionary<string, Monkey> monkeys)
        {
            var root = monkeys["root"];
            var cache = new Dictionary<string, long>();

            // Determine which side contains humn
            bool leftContainsHuman = ContainsHuman(root.Left, monkeys);

            // Get the value of the side that doesn't contain humn
            long targetValue = leftContainsHuman 
                ? ResolveMonkey(root.Right, monkeys, cache)
                : ResolveMonkey(root.Left, monkeys, cache);

            // Now solve for humn on the side that contains it
            string humanSide = leftContainsHuman ? root.Left : root.Right;
            return SolveForHuman(humanSide, targetValue, monkeys);
        }

        private bool ContainsHuman(string monkeyName, Dictionary<string, Monkey> monkeys)
        {
            if (monkeyName == "humn")
                return true;

            var monkey = monkeys[monkeyName];

            if (monkey.Value.HasValue)
                return false;

            return ContainsHuman(monkey.Left, monkeys) || ContainsHuman(monkey.Right, monkeys);
        }

        private long SolveForHuman(string monkeyName, long target, Dictionary<string, Monkey> monkeys)
        {
            if (monkeyName == "humn")
                return target;

            var monkey = monkeys[monkeyName];
            var cache = new Dictionary<string, long>();

            // Determine which side contains humn
            bool leftContainsHuman = ContainsHuman(monkey.Left, monkeys);

            if (leftContainsHuman)
            {
                // humn is on the left: left op right = target
                // Solve for left
                long rightVal = ResolveMonkey(monkey.Right, monkeys, cache);
                long newTarget = monkey.Operation switch
                {
                    '+' => target - rightVal,                    // left + right = target => left = target - right
                    '-' => target + rightVal,                    // left - right = target => left = target + right
                    '*' => target / rightVal,                    // left * right = target => left = target / right
                    '/' => target * rightVal,                    // left / right = target => left = target * right
                    _ => 0
                };

                return SolveForHuman(monkey.Left, newTarget, monkeys);
            }
            else
            {
                // humn is on the right: left op right = target
                // Solve for right
                long leftVal = ResolveMonkey(monkey.Left, monkeys, cache);
                long newTarget = monkey.Operation switch
                {
                    '+' => target - leftVal,                     // left + right = target => right = target - left
                    '-' => leftVal - target,                     // left - right = target => right = left - target
                    '*' => target / leftVal,                     // left * right = target => right = target / left
                    '/' => leftVal / target,                     // left / right = target => right = left / target
                    _ => 0
                };

                return SolveForHuman(monkey.Right, newTarget, monkeys);
            }
        }
    }
}
