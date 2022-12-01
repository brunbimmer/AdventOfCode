using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 3)]
    public class Year2015Day3Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day3Problem()
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

            string elfDirections = FileIOHelper.getInstance().ReadDataAsString(file);

            SW.Start();

            int housesVisited = Part1(elfDirections);

            SW.Stop();

            Console.WriteLine("  Part 1: Number of houses visited by Santa Prime: {0}", housesVisited);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            housesVisited = Part2(elfDirections);

            Console.WriteLine("  Part 2: Number of houses visited by twin Santa's: {0}", housesVisited);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("\n===========================================\n");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        public int Part1(string elfDirections)
        {
            List<Coordinate2D> houseList = new();

            int x = 0;
            int y = 0;
            houseList.Add(new Coordinate2D(x, y));

            for(int i = 0; i < elfDirections.Length; i++)
            {
                Coordinate2D nexthouse;
                (x, y) = GetNewCoordinates(elfDirections[i], x, y);
                nexthouse = new Coordinate2D(x, y);

                if (!houseList.Contains(nexthouse)) houseList.Add(nexthouse);
            }
            return houseList.Count;
        }

        public int Part2(string elfDirections)
        {
            List<Coordinate2D> houseList = new();

            int realSantaX = 0;
            int realSantaY = 0;

            int roboSantaX = 0;
            int roboSantaY = 0;

            houseList.Add(new Coordinate2D(0, 0));

            for (int i = 0; i < elfDirections.Length; i++)
            {
                Coordinate2D nexthouse;

                //start w/ Santa on even counts, Robo Santa on odd counts.
                if (i % 2 == 0)
                {
                    (realSantaX, realSantaY) = GetNewCoordinates(elfDirections[i], realSantaX, realSantaY);
                    nexthouse = new Coordinate2D(realSantaX, realSantaY);
                }
                else
                {
                    (roboSantaX, roboSantaY) = GetNewCoordinates(elfDirections[i], roboSantaX, roboSantaY);
                    nexthouse = new Coordinate2D(roboSantaX, roboSantaY);
                }

                if (!houseList.Contains(nexthouse)) houseList.Add(nexthouse);
            }
            return houseList.Count;
        }


        public (int, int) GetNewCoordinates(char direction, int x, int y)
        {            
            switch (direction)
            {
                case '>':
                    x += 1;
                    break;
                case '<':
                    x -= 1;
                    break;
                case '^':
                    y += 1;
                    break;
                case 'v':
                    y -= 1;
                    break;
            }

            return (x, y);
        }
    }
}
