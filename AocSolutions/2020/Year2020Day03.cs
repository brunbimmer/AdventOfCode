using System;
using System.Diagnostics;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 3)]
    public class Year2020Day03 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day03()
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
            string[] map = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            int trees = Part1(map);
            _SW.Stop();

            output.WritePart(1, trees, trackTime ? _SW.Elapsed : null, "Number of trees encountered");


            _SW.Restart();
            long product = Part2(map);
            _SW.Stop();

            output.WritePart(2, product, trackTime ? _SW.Elapsed : null, "Product of trees encountered on all five slopes");

        }

        private int Part1(string[] map)
        {
            return CountTrees(map, 3, 1);
        }

        private long Part2(string[] map)
        {
            long product = CountTrees(map, 1, 1);
            product *= CountTrees(map, 3, 1);
            product *= CountTrees(map, 5, 1);
            product *= CountTrees(map, 7, 1);
            product *= CountTrees(map, 1, 2);
            return product;
        }

        private int CountTrees(string[] map, int right, int down)
        {
            int trees = 0;
            int column = 0;

            for (int row = down; row < map.Length; row += down)
            {
                column = (column + right) % map[row].Length;
                if (map[row][column] == '#')
                    trees++;
            }

            return trees;
        }
    }
}
