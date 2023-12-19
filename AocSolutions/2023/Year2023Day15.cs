using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventFileIO;
using Common;

using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 15)]
    public class Year2023Day15 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day15()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new();
        }

        

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string input = FileIOHelper.getInstance().ReadDataAsString(file);
            string[] parts = input.Trim().Split(',');

            _SW.Start();

            int result = SolvePart1(parts);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Total Load: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Restart();
            
            _SW.Start();
            result = SolvePart2(parts);
            _SW.Stop();

            Console.WriteLine($"  Part 2: Total Load: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        public int SolvePart1(string[] parts)
        {
            return parts.Sum(CalculateHash);
        }

        public int SolvePart2(string[] parts)
        {
            Dictionary<int, OrderedDictionary> boxes = new();

            int sum = 0;

            foreach (string part in parts)
            {
                var _sequences = Regex.Split(part, @"[-=]");
         
                var label = _sequences[0];
                int.TryParse(_sequences[1], out var value);

                int hash = CalculateHash(label);

                if (boxes.TryGetValue(hash, out var elements))
                {
                    if (part.Contains("="))
                    {
                        if (elements.Contains(label))
                            elements[label] = value;
                        else
                            elements.Add(label, value);
                    }
                    else
                        elements.Remove(label);
                }
                else
                {
                    if (part.Contains("="))
                        boxes.Add(hash, new OrderedDictionary() {{label, value}});
                }
            }

            foreach (var (boxNum, value) in boxes)
            {
                int boxIndex = 1;

                foreach (DictionaryEntry element in value)
                {
                    sum += (boxNum + 1) * boxIndex * Convert.ToInt32(element.Value);
                    boxIndex += 1;
                }
            }

            return sum;
        }

        private int CalculateHash(string sequence)
        {
            var stepSum = 0;

            foreach (char c in sequence)
            {
                stepSum += (Convert.ToInt32(c));
                stepSum *= 17;
                stepSum %= 256;
            }

            return stepSum;
        }
    }
}