using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 13)]
    public class Year2022Day13 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day13()
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
            string input = FileIOHelper.getInstance().ReadDataAsString(file);

            _SW.Start();
            var packets = ParseInputIntoJsonNodes(input);

            int numOrderedPairs = packets.Chunk(2)
              .Select((pair, index) => CompareNodes(pair[0], pair[1]) < 0 ? index + 1 : 0)
              .Sum();
            _SW.Stop();
            Console.WriteLine("Part 1: Decoder Key {0}, Execution Time: {1}", numOrderedPairs, StopwatchUtil.getInstance().GetTimestamp(_SW));
            _SW.Restart();
            var dividerPackets = ParseInputIntoJsonNodes("[[2]]\n[[6]]").ToList();
            var newPacketList = packets.Concat(dividerPackets).ToList();
            newPacketList.Sort(CompareNodes);
            int decoderKey = (newPacketList.IndexOf(dividerPackets[0]) + 1) * (newPacketList.IndexOf(dividerPackets[1]) + 1);
            _SW.Stop();
            Console.WriteLine("Part 2: Decoder Key {0}, Execution Time: {1}", decoderKey, StopwatchUtil.getInstance().GetTimestamp(_SW));

        }

        private IEnumerable<JsonNode> ParseInputIntoJsonNodes(string input)
        {
            var json = from line in input.Split("\n")
                       where !string.IsNullOrEmpty(line)
                       select JsonNode.Parse(line);
            return json;
        }
        private int CompareNodes(JsonNode left, JsonNode right)
        {
            if (left is JsonValue && right is JsonValue)
                return (int)left - (int)right;
            else
            {
                var arrayA = left as JsonArray ?? new JsonArray((int)left);
                var arrayB = right as JsonArray ?? new JsonArray((int)right);
                return Enumerable.Zip(arrayA, arrayB)
                  .Select(p => CompareNodes(p.First, p.Second))
                  .FirstOrDefault(c => c != 0, arrayA.Count - arrayB.Count);
            }
        }
    }
}
