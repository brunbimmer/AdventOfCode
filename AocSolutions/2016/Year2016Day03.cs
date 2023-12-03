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
using LINQPad.Controls;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 3)]
    public class Year2016Day03 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2016Day03()
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

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] triangleList = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            int numPossibleTriangles = ParseTriangleList(triangleList);

            
            SW.Stop();

            Console.WriteLine("Part 1: Number of possible triangles: {0}, Execution Time: {1}", numPossibleTriangles, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            numPossibleTriangles = ParseVerticalListTriangles(triangleList);
            
            SW.Stop();

            Console.WriteLine("Part 2: Number of possible triangles by column: {0}, Execution Time: {1}", numPossibleTriangles, StopwatchUtil.getInstance().GetTimestamp(SW));


        }

        int ParseTriangleList(string[] triangleList)
        {
            int possibleTriangles = 0;

            foreach (string triangle in triangleList)
            {
                int[] sides = triangle.Trim().Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                      .Select(int.Parse)
                                      .OrderBy(x => x)
                                      .ToArray();

                if (sides[0] + sides[1] > sides[2])
                    possibleTriangles += 1;
            }

            return possibleTriangles;
        }

        int ParseVerticalListTriangles(string[] triangleList)
        {
            int possibleTriangles = 0;

            for(int i = 0; i <= (triangleList.Length - 3); i += 3)
            {
                string[] side1 = triangleList[i].Trim().Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                string[] side2 = triangleList[i + 1].Trim().Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                string[] side3 = triangleList[i + 2].Trim().Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                int[] triangle1 = { int.Parse(side1[0]), int.Parse(side2[0]), int.Parse(side3[0]) };
                int[] triangle2 = { int.Parse(side1[1]), int.Parse(side2[1]), int.Parse(side3[1]) };
                int[] triangle3 = { int.Parse(side1[2]), int.Parse(side2[2]), int.Parse(side3[2]) };

                var orderedTriangle1 = triangle1.OrderBy(x => x).ToArray();
                var orderedTriangle2 = triangle2.OrderBy(x => x).ToArray();
                var orderedTriangle3 = triangle3.OrderBy(x => x).ToArray();

                if (orderedTriangle1[0] + orderedTriangle1[1] > orderedTriangle1[2])
                    possibleTriangles += 1;

                if (orderedTriangle2[0] + orderedTriangle2[1] > orderedTriangle2[2])
                    possibleTriangles += 1;

                if (orderedTriangle3[0] + orderedTriangle3[1] > orderedTriangle3[2])
                    possibleTriangles += 1;
            }

            return possibleTriangles;
        }
    }
}
