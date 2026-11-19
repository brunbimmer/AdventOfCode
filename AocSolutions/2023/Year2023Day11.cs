using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;


namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 11)]
    public class Year2023Day11 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day11()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        private List<int> EmptyRows, EmptyColumns;
        private char[][] rawMap;
        private Dictionary<Coordinate2D, char> map;

        private long Part2_Expansion = 1_000_000;

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            //Build BasePath and retrieve input. 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            ParseInput(file);

            _SW.Start();

            (long totalSum, long totalSum2) = ParseMap(map);

            _SW.Stop();

            output.WritePart(1, totalSum, trackTime ? _SW.Elapsed : null, "Sum of Galaxy Pair Manhattan Distances");
            output.WritePart(2, totalSum2, trackTime ? _SW.Elapsed : null, "Sum of Galaxy Pair Manhattan Distances");
;


        }

        public void ParseInput(string file)
        {
            rawMap = FileIOHelper.getInstance().GetDataAsDoubleCharArray(file);
            map = FileIOHelper.getInstance().GetDataAsCoordinates(file);


            EmptyRows = Enumerable.Range(0, rawMap.Length)
                .Where(row => rawMap[row].All(cell => cell == '.'))
                .ToList();

            EmptyColumns = Enumerable.Range(0, rawMap[0].Length)
                .Where(col => rawMap.All(row => row[col] == '.'))
                .ToList();
        }


        public (long,long) ParseMap(Dictionary<Coordinate2D, char> map)
        {
            long totalSum = 0;
            long totalSumPart2 = 0;
            var galaxyList = map.Where(x => x.Value == '#').ToDictionary();

            List<Coordinate2D> galaxyCoordinates = galaxyList.Keys.ToList();

            var galaxyPermutations = GetCombinations(galaxyCoordinates);

            foreach (var galaxyPair in galaxyPermutations)
            {
                var first = galaxyPair.Item1;
                var second = galaxyPair.Item2;

                //get top left and bottom right coordinates to determine the 
                var boundingBox = GetBox(first, second);
                var padding = GetPadding(boundingBox.topLeft, boundingBox.bottomRight);
                var padding2 = padding * (Part2_Expansion - 1);     //Take away one as we already did first expansion in part 2
                var distance = first.ManhattenDistance(second);
                var sum = distance + padding;
                var sum2 = distance + padding2;
                totalSum += sum;
                totalSumPart2 += sum2;
            }

            return (totalSum, totalSumPart2);
        }

        public List<(Coordinate2D, Coordinate2D)> GetCombinations(List<Coordinate2D> items)
        {
            var combinations = items.SelectMany((x, i) => items.Skip(i + 1).Select(y => (x, y))).ToList();

            return combinations;
        }


        long GetPadding((int row, int col) topLeft, (int row, int col) bottomRight) => GetColumnPadding(topLeft, bottomRight) + GetRowPadding(topLeft, bottomRight);
        
        long GetRowPadding((int row, int col) topLeft, (int row, int col) bottomRight) => EmptyRows.Count(x => x >= topLeft.row && x <= bottomRight.row);

        long GetColumnPadding((int row, int col) topLeft, (int row, int col) bottomRight) => EmptyColumns.Count(x => x >= topLeft.col && x <= bottomRight.col);

        ((int row, int col) topLeft, (int row, int col) bottomRight) GetBox(Coordinate2D a, Coordinate2D b)
            => ((Math.Min(a.X, b.X), Math.Min(a.Y, b.Y)), (Math.Max(a.X, b.X), Math.Max(a.Y, b.Y)));
    }
}