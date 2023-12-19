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
using LINQPad;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 10)]
    public class Year2016Day10 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day10()
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

            FindBotNumber(lines);

            
            _SW.Stop();

            Console.WriteLine("Total Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private void FindBotNumber(string[] lines)
        {
            var bots = new Dictionary<int, Action<int>>();
            int[] outputs = new int[21];

            var regex = new Regex(@"value (?<value>\d+) goes to bot (?<bot>\d+)|bot (?<source>\d+) gives low to (?<low>(bot|output)) (?<lowval>\d+) and high to (?<high>(bot|output)) (?<highval>\d+)");

            foreach (var line in lines.OrderBy(x => x))
            {
                var match = regex.Match(line);
                if (match.Groups["value"].Success)
                {
                    bots[int.Parse(match.Groups["bot"].Value)](int.Parse(match.Groups["value"].Value));
                }
                if (match.Groups["source"].Success)
                {
                    List<int> vals = new List<int>();
                    var botnum = int.Parse(match.Groups["source"].Value);
                    bots[botnum] = (int n) =>
                    {
                        vals.Add(n);
                        if (vals.Count == 2)
                        {
                            if (vals.Min() == 17 && vals.Max() == 61) 
                                System.Console.WriteLine($"Part 1: Bot Number that compares value-61 microchip w/ value-17 microchip: {botnum}");
                            if (match.Groups["low"].Value == "bot")
                            {
                                bots[int.Parse(match.Groups["lowval"].Value)](vals.Min());
                            }
                            else
                            {
                                outputs[int.Parse(match.Groups["lowval"].Value)] = vals.Min();
                            }
                            if (match.Groups["high"].Value == "bot")
                            {
                                bots[int.Parse(match.Groups["highval"].Value)](vals.Max());
                            }
                            else
                            {
                                outputs[int.Parse(match.Groups["highval"].Value)] = vals.Max();
                            }
                        }
                    };
                }
            }

            System.Console.WriteLine($"Part 2: Result of multiplying outputs 0 x 1 x 2: {outputs[0] * outputs[1] * outputs[2]}");
        }
    }
}
