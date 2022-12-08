using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using CommonAlgorithms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 12)]
    public class Year2015Day12Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        
        public Year2015Day12Problem()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string content = FileIOHelper.getInstance().ReadDataAsString(file);

            SW.Start();

            long[] numbersOnly = Regex.Matches(content, "(-?[0-9]+)").OfType<Match>().Select(m => long.Parse(m.Value)).ToArray();
                         

            SW.Stop();            

            Console.WriteLine("  Part 1: The sum of all numbers in this JSON sequence is: {0}", numbersOnly.Sum());
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            long sum = Part2Algorithm(content);

            SW.Stop();

            Console.WriteLine("  Part 1: The sum of all numbers in this JSON sequence with RED removed is: {0}", sum);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));
            


        }

        long Part2Algorithm(string content)
        {
            dynamic o = JsonConvert.DeserializeObject(content);

            return GetSum(o, "red");

        }

        long GetSum(JObject o, string avoid = null) {
            bool shouldAvoid = o.Properties()
                .Select(a => a.Value).OfType<JValue>()
                .Select(v => v.Value).Contains(avoid);
            if (shouldAvoid) return 0;

            return o.Properties().Sum((dynamic a) => (long)GetSum(a.Value, avoid));
        }

        long GetSum(JArray arr, string avoid) => arr.Sum((dynamic a) => (long)GetSum(a, avoid));

        long GetSum(JValue val, string avoid) => val.Type == JTokenType.Integer ? (long)val.Value : 0;
    }
}
