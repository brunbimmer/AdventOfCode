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
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 1)]
    public class Year2024Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2024Day01()
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

            var (leftList, rightList) = lines
                .Select(line => line.Split("   ", StringSplitOptions.TrimEntries))
                .Select(values => (Convert.ToInt32(values[0]), Convert.ToInt32(values[1])))
                .Aggregate((left: new List<int>(), right: new List<int>()), (acc, val) =>
                {
                    acc.left.Add(val.Item1);
                    acc.right.Add(val.Item2);
                    return acc;
                });


            _SW.Start();

            int sum = CalculateTotalDistance(leftList, rightList);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Total Distance {sum}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            int similarityScore = CalculateSimilarityScore(leftList, rightList);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Similarity Score: {similarityScore}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        public int CalculateTotalDistance(List<int> left, List<int> right)
        {
            left.Sort();
            right.Sort();

            return left.Zip(right, (l, r) => Math.Abs(r - l)).Sum();
        }

        public int CalculateSimilarityScore(List<int> left, List<int> right)
        {
            return left.Sum(i => i * right.Count(x => x == i));
        }
    }
}
