using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventFileIO;
using Common;

using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 14)]
    public class Year2023Day14 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day14()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

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
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            var values = ParseToCharacterDimensionArray(input);
            int result = SolvePart1(values);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Total Load: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Restart();
            
            _SW.Start();
            result = SolvePart2(values);
            _SW.Stop();

            Console.WriteLine($"  Part 2: Total Load: {result}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        private char[,] ParseToCharacterDimensionArray(string[] lines)
        {
            int width = lines[0].Length, height = lines.Length;
            var rocks = new char[width, height];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    rocks[x, y] = lines[y][x];
            return rocks;
        }

        private int SolvePart1(char[,] rocks) =>
           GetLoad(TiltNorth((char[,])rocks.Clone()));

        private int SolvePart2(char[,] rocks)
        {
            List<char[,]> history = new();
            int i;
            for (; (i = history.FindIndex(r => ArrayEquals(rocks, r))) < 0; rocks = GetNext(rocks))
                history.Add(rocks);
            return GetLoad(history[(1000000000 - i) % (history.Count - i) + i]);
        }

        private int GetLoad(char[,] rocks)
        {
            int load = 0;
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (rocks[x, y] == 'O')
                        load += height - y;
            return load;
        }

        private bool ArrayEquals(char[,] left, char[,] right)
        {
            int width = left.GetLength(0), height = left.GetLength(1);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (left[x, y] != right[x, y])
                        return false;
            return true;
        }

        private char[,] GetNext(char[,] rocks) =>
            TiltEast(TiltSouth(TiltWest(TiltNorth((char[,])rocks.Clone()))));

        private char[,] TiltNorth(char[,] rocks)
        {
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (rocks[x, y] == 'O')
                    {
                        rocks[x, y] = '.';
                        rocks[x, SlideNorth(rocks, x, y)] = 'O';
                    }
            return rocks;
        }

        private char[,] TiltWest(char[,] rocks)
        {
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (rocks[x, y] == 'O')
                    {
                        rocks[x, y] = '.';
                        rocks[SlideWest(rocks, x, y), y] = 'O';
                    }
            return rocks;
        }

        private char[,] TiltSouth(char[,] rocks)
        {
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            for (int x = 0; x < width; ++x)
                for (int y = height - 1; y >= 0; --y)
                    if (rocks[x, y] == 'O')
                    {
                        rocks[x, y] = '.';
                        rocks[x, SlideSouth(rocks, x, y, height)] = 'O';
                    }
            return rocks;
        }

        private char[,] TiltEast(char[,] rocks)
        {
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            for (int x = width - 1; x >= 0; --x)
                for (int y = 0; y < height; ++y)
                    if (rocks[x, y] == 'O')
                    {
                        rocks[x, y] = '.';
                        rocks[SlideEast(rocks, x, y, width), y] = 'O';
                    }
            return rocks;
        }

        private int SlideNorth(char[,] rocks, int x, int y)
        {
            for (int i = y - 1; i >= 0; --i)
                if (rocks[x, i] != '.')
                    return i + 1;
            return 0;
        }

        private int SlideWest(char[,] rocks, int x, int y)
        {
            for (int i = x - 1; i >= 0; --i)
                if (rocks[i, y] != '.')
                    return i + 1;
            return 0;
        }

        private int SlideSouth(char[,] rocks, int x, int y, int height)
        {
            for (int i = y + 1; i < height; ++i)
                if (rocks[x, i] != '.')
                    return i - 1;
            return height - 1;
        }

        private int SlideEast(char[,] rocks, int x, int y, int width)
        {
            for (int i = x + 1; i < width; ++i)
                if (rocks[i, y] != '.')
                    return i - 1;
            return width - 1;
        }
    }
}