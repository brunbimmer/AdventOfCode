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
    [AdventOfCode(Year = 2021, Day = 13)]
    public class Year2021Day13 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2021Day13()
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

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       

            List<(int, int)> graphMap;
            List<string> instructions;
            (graphMap, instructions) = BuildInstructionMap(lines);
            List<(int, int)> graphMapPart1 = TransformGraphMap(graphMap, instructions, 1);

            _SW.Stop();

            Console.WriteLine("Part 1: Number of Points after First Fold: {0}", graphMapPart1.Count);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            List<(int, int)> graphMapFinal = TransformGraphMap(graphMap, instructions, instructions.Count);
            string code = BuildCode(graphMapFinal.OrderBy(x => x.Item2).ToList());
            
            _SW.Stop();

             Console.WriteLine("Part 2: The code\n\n{0}", code);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       

        private (List<(int, int)>, List<string>) BuildInstructionMap(string[] lines)
        {
            var graphMap = new List<(int, int)>();
            var instructions = new List<string>();
            foreach (var line in lines.Where(x => x.Contains(",")))
            {
                var coords = line.Split(',');
                int x, y;
                (x, y) = (int.Parse(coords[0]), int.Parse(coords[1]));
                graphMap.Add((x, y));
            }

            foreach (var line in lines.Where(x => x.Contains("fold")))
            {
                instructions.Add(line);
            }

            return (graphMap, instructions);
        }

        private List<(int, int)> TransformGraphMap(List<(int, int)> graphMap, List<string> instructions, int numOfInstructions)
        {
            var newGraphMap = new List<(int, int)>(graphMap);
            for (var i = 0; i < numOfInstructions; i++)
            {
                var fold = instructions.ElementAt(i).Split('=', ' ');
                string direction;
                int position;
                (direction, position) = (fold[2], int.Parse(fold[3]));
                switch (direction)
                {
                    case "x":
                        newGraphMap = FoldX(newGraphMap, position);
                        break;
                    case "y":
                        newGraphMap = FoldY(newGraphMap, position);
                        break;
                }
            }

            return newGraphMap;
        }

        private List<(int, int)> FoldY(List<(int, int)> graphMap, int position)
        {
            var newGraphMap = new List<(int, int)>(graphMap.Where(x => x.Item2 < position));
            var MaxY = graphMap.OrderByDescending(x => x.Item2).First().Item2;
            var newYIndex = position - (MaxY - position);
            for (var y = MaxY; y > position; y--)
            {
                var oldLine = graphMap.Where(e => e.Item2 == y).ToArray();
                var newLine = new List<(int, int)>();
                foreach (var element in oldLine)
                {
                    int newX, newY;
                    (newX, newY) = (element.Item1, newYIndex);
                    if (!newGraphMap.Contains((newX, newY)))
                        newLine.Add((newX, newY));
                }

                newGraphMap.AddRange(newLine);
                newYIndex++;
            }

            return newGraphMap;
        }

        private List<(int, int)> FoldX(List<(int, int)> graphMap, int position)
        {
            var newGraphMap = new List<(int, int)>(graphMap.Where(x => x.Item1 < position));
            var MaxX = graphMap.OrderByDescending(x => x.Item1).First().Item1;
            var newXIndex = position - (MaxX - position);
            for (var x = MaxX; x > position; x--)
            {
                var oldLine = graphMap.Where(e => e.Item1 == x).ToArray();
                var newLine = new List<(int, int)>();
                foreach (var element in oldLine)
                {
                    int newX, newY;
                    (newX, newY) = (newXIndex, element.Item2);
                    if (!newGraphMap.Contains((newX, newY)))
                        newLine.Add((newX, newY));
                }

                newGraphMap.AddRange(newLine);
                newXIndex++;
            }

            return newGraphMap;
        }

        private string BuildCode(List<(int, int)> graphMap)
        {
            var sb = new StringBuilder();
            var maxX = graphMap.OrderByDescending(x => x.Item1).First().Item1;
            var MaxY = graphMap.OrderByDescending(x => x.Item2).First().Item2;
            for (int posY = 0; posY <= MaxY; posY++)
            {
                var line = new List<char>();
                for (int posX = 0; posX <= maxX; posX++)
                    line.Add(graphMap.Contains((posX, posY)) ? 'X' : ' ');
                sb.AppendLine(new string(line.ToArray()));
            }

            return sb.ToString();
        }
    }
}
