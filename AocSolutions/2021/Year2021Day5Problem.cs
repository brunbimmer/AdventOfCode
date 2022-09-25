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
    [AdventOfCode(Year = 2021, Day = 5)]
    public class Year2021Day5Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        private class CoordinatePair
        {
            public int x1 {get; set;}
            public int y1 {get; set;}
            public int x2 {get; set;}
            public int y2 {get; set;}
        }

        private static List<CoordinatePair> Coordinates = new List<CoordinatePair>();

        private static int MaxXCoord = 0;
        private static int MaxYCoord = 0;

        private static int[,] VentMap;

        private static int OverlappingPoints;

        public Year2021Day5Problem()
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
            string filename = _OverrideFile ?? path;

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            ReadFile(file);

            if (trackTime) SW.Start();
            
            ParseInputForOverlappingPoints(true);
            if (trackTime) SW.Stop();



            Console.WriteLine("Part 1: Number of overlapping points for horizontal and vertical lines: {0}", OverlappingPoints);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            ParseInputForOverlappingPoints();
            if (trackTime) SW.Stop();

            Console.WriteLine("Part 2: Number of overlapping points for horizontal/vertical and diagonal lines: {0}", OverlappingPoints);

            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        private void ReadFile(string filename)
        {
            try
            {
                string line;
                using (TextReader reader = File.OpenText(filename))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] coordinates = line.Split(new string[] { "->" }, StringSplitOptions.None);

                        CoordinatePair pair = new CoordinatePair();

                        pair.x1 = Convert.ToInt32(coordinates[0].Split(',')[0]);
                        pair.y1 = Convert.ToInt32(coordinates[0].Split(',')[1]);
                        pair.x2 = Convert.ToInt32(coordinates[1].Split(',')[0]);
                        pair.y2 = Convert.ToInt32(coordinates[1].Split(',')[1]);

                        //while reading all the inputs, determine the maximum X and Y coordinate from the 
                        //list for initialization.
                        MaxXCoord = (MaxXCoord < pair.x1 || MaxXCoord < pair.x2) ? (pair.x1 > pair.x2 ? pair.x1 : pair.x2) : MaxXCoord;
                        MaxYCoord = (MaxYCoord < pair.y1 || MaxYCoord < pair.y2) ? (pair.y1 > pair.y2 ? pair.y1 : pair.y2) : MaxYCoord;

                        Coordinates.Add(pair);
                    }                  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }      

        private void ParseInputForOverlappingPoints(bool straightLinesOnly = false)
        {
            //zero-based, so add a 1 to each coordinate for proper dimensional sizing
            VentMap = new int[MaxXCoord + 1, MaxYCoord + 1];

            List<CoordinatePair> SourceCoordinates;

            if (straightLinesOnly)
                SourceCoordinates = Coordinates.Where(coord => (coord.x1 == coord.x2) || (coord.y1 == coord.y2))
                    .ToList();
            else
                SourceCoordinates = Coordinates;

            foreach (CoordinatePair pair in SourceCoordinates)
            {
                if (pair.x1 == pair.x2)
                {
                    int yStart = (pair.y1 < pair.y2)? pair.y1 : pair.y2;  //determine start position
                    int yEnd = (pair.y1 > pair.y2)? pair.y1 : pair.y2;    //determine end position

                    for (int y = yStart; y <= yEnd; y++)
                        VentMap[pair.x1, y] = VentMap[pair.x1, y] + 1;
                }
                else if (pair.y1 == pair.y2)
                {
                    int xStart = (pair.x1 < pair.x2)? pair.x1 : pair.x2;  //determine start position
                    int xEnd = (pair.x1 > pair.x2)? pair.x1 : pair.x2;    //determine end position

                    for (int x = xStart; x <= xEnd; x++)
                        VentMap[x, pair.y1] = VentMap[x, pair.y1] + 1;
                }
                else 
                {
                    //xDirection: 0 for negative, 1 for positive
                    //yDirection: 0 for negative, 1 for positive

                    int xDirection = (pair.x1 < pair.x2) ? 1 : 0;
                    int yDirection = (pair.y1 < pair.y2) ? 1 : 0;

                    int xCoord = pair.x1;
                    int yCoord = pair.y1;

                    bool stop = false;

                    while (!stop)
                    {
                        VentMap[xCoord, yCoord] = VentMap[xCoord, yCoord] + 1;

                        if (xCoord == pair.x2 && yCoord == pair.y2)
                            stop = true;

                        //set next coordinates based on direction 
                        xCoord = (xDirection == 1) ? xCoord + 1 : xCoord - 1;
                        yCoord = (yDirection == 1) ? yCoord + 1 : yCoord - 1;
                    }
                }

            }

            OverlappingPoints = (from int val in VentMap where val >=2 select val).Count();
        }
    }
}
