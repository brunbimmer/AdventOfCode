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

        public Stopwatch _SW { get; set; }

        
        public Year2015Day12Problem()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string content = FileIOHelper.getInstance().ReadDataAsString(file);

            _SW.Start();

            long[] numbersOnly = Regex.Matches(content, "(-?[0-9]+)").OfType<Match>().Select(m => long.Parse(m.Value)).ToArray();
                         

            _SW.Stop();            

            output.WritePart(1, numbersOnly.Sum(), trackTime ? _SW.Elapsed : null, "The sum of all numbers in this JSON sequence is");


            _SW.Restart();

            long sum = Part2Algorithm(content);

            _SW.Stop();

            output.WritePart(2, sum, trackTime ? _SW.Elapsed : null, "The sum of all numbers in this JSON sequence with RED removed is");


            


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
