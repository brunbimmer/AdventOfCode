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
using static AdventOfCode.Year2015Day16;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 5)]
    public class Year2024Day05: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2024Day05()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }
        private record InputData(
             List<string> PageOrderingRules,
             List<string> Updates,
             Dictionary<int, List<int>> GroupRules);

        List<string> InvalidPages = new List<string>();


        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            var inputData = ParseInput(lines);

            _SW.Start();

            int part1 = ProcessPages(inputData);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Sum of correctly-ordered updates ==> {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            int part2 = FixPages(inputData);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Sum of Correct Pages ==> {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        private InputData ParseInput(string[] input)
        {
            Dictionary<int, List<int>> groupedRules = new Dictionary<int, List<int>>();
            var pageOrderingRules = input.Where(x => x.Contains('|')).ToList();
            var pageUpdates = input.Where(x => x.Contains(',')).ToList();

            Regex keyRegex = new Regex(@"[0-9]{2}");
            Regex groupRegex = new Regex(@"\|[0-9]{2}");
            var allRules = new List<Match>();
            var ruleValues = pageOrderingRules.Select(rule => keyRegex.Match(rule).Value).Distinct().ToList();
            foreach (var rule in ruleValues)
            {
                var ruleGroup = pageOrderingRules.Where(x => x.Contains($"{rule}|")).ToList();
                var dictGroup = new List<int>();
                foreach (var value in ruleGroup)
                {
                    dictGroup.Add(int.Parse(groupRegex.Match(value).Value.Substring(1)));
                }
                groupedRules.Add(int.Parse(rule), dictGroup);
            }

            return new InputData(pageOrderingRules, pageUpdates, groupedRules);
        }

        int ProcessPages(InputData data)
        {
            int sum = 0;

            foreach (var page in data.Updates)
            {
                var pageInvalid = false;
                var numbers = page.Split(',').Select(int.Parse).ToList();
                var middleNumber = numbers[numbers.Count / 2];
                for (int i = 0; i < numbers.Count; i++)
                {
                    if (pageInvalid)
                        break;

                    data.GroupRules.TryGetValue(numbers[i], out var group);
                    for (int j = i + 1; j < numbers.Count; j++)
                    {
                        if (!group.Contains(numbers[j]))
                        {
                            pageInvalid = true;
                            InvalidPages.Add(page);
                            break;
                        }
                    }
                }
                if (!pageInvalid)
                    sum += middleNumber;
            }
            return sum;
        }

        int FixPages(InputData data)
        {
            int sum = 0;

            foreach (var invalidPage in InvalidPages)
            {
                var numbers = invalidPage.Split(',').Select(int.Parse).ToList();
                var allPageGroups = data.GroupRules.Where(x => numbers.Contains(x.Key));
                var numberCorrectPositions = new List<(int number, int corrrectPosition)>();
                var fixedPage = new List<int>();
                for (int i = 0; i < numbers.Count; i++)
                {
                    var groups = allPageGroups.Select(x => x.Value).ToList();
                    var numberCorrectPosition = -1; //Start off negative as each iteration will ammend the position by 1
                    foreach (var group in groups)
                    {
                        if (group.Contains(numbers[i]))
                            numberCorrectPosition++;
                    }

                    numberCorrectPositions.Add((numbers[i], numberCorrectPosition));
                }
                foreach (var fix in numberCorrectPositions.OrderByDescending(x => x.corrrectPosition))
                {
                    fixedPage.Add(fix.number);
                }
                sum += fixedPage[fixedPage.Count / 2];
            }

            return sum;
        }


    }
}
