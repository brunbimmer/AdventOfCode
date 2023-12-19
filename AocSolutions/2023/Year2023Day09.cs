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
using MoreLinq;
using static AdventOfCode.Year2015Day16;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 9)]
    public class Year2023Day09 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day09()
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

            _SW.Start();

            (long left_sum, long right_sum)  = FindSolution(input);
            _SW.Stop();

            Console.WriteLine($"  Part 1: Sum of Right element sequences: {right_sum}");
            Console.WriteLine($"  Part 2: Sum of Left element sequences: {left_sum}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");
        }

        private (long, long) FindSolution(string[] input)
        {
            long left_sum = 0;
            long right_sum = 0;
            foreach (string line in input)
            {
                long[] numbers = line.Split(" ").Select(long.Parse).ToArray();

                (long left, long right) = FindNextSequenceElement(numbers);

                left_sum += (numbers.First() - left);
                right_sum += (numbers.Last() + right);
            }

            return (left_sum, right_sum);
        }

        private (long, long) FindNextSequenceElement(long[] numbers)
        {
            long[] differences = new long[numbers.Length - 1];

            for (int i = 1; i < numbers.Length; i++)
            {
                differences[i - 1] = numbers[i] - numbers[i - 1];
            }

            if (differences.All(num => num == 0))
                return (0L, 0L);
            else
            {
                (long left, long right) = FindNextSequenceElement(differences); 

                return (differences.First() - left, differences.Last() + right);
            }
        }
    }

}
