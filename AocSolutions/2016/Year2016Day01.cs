using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 1)]
    public class Year2016Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2016Day01()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }


        private enum Direction
        {
            N,
            E,
            S,
            W

        }

        private Direction _currentDirection = Direction.N;
        private Coordinate2D _currentPosition = new(0, 0);
        private Coordinate2D _doubleHqCoordinate;
        private List<Coordinate2D> _locations = new();

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

 
            //Build BasePath and retrieve input. 
 



            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] directions = FileIOHelper.getInstance().ReadDataAsString(file).Split(",", StringSplitOptions.TrimEntries);
            
            SW.Start();
            ParseDirectionsWithDoubleLocationVisit(directions);

            int part1Distance = Math.Abs(_currentPosition.X) + Math.Abs(_currentPosition.Y);
            int part2Distance = Math.Abs(_doubleHqCoordinate.X) + Math.Abs(_doubleHqCoordinate.Y);
            
            SW.Stop();

            Console.WriteLine("  Part 1: Distance to Bunny HQ: {0}", part1Distance);
            Console.WriteLine("  Part 2: Action Distance to Bunny HQ: {0}", part2Distance);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));





        }

        private void ParseDirectionsWithDoubleLocationVisit(string[] directions)
        {
            bool isDoubleLocationFound = false;

            foreach (string s in directions)
            {

                _currentDirection = GetNewDirection(s[0]);

                for (int i = 0; i < int.Parse(s.Substring(1)); i++)
                {
                    _currentPosition = GetNewPosition();

                    if (!isDoubleLocationFound && _locations.Contains(_currentPosition))
                    {
                        _doubleHqCoordinate = _currentPosition;
                        isDoubleLocationFound = true;
                    }
                    else
                    {
                        _locations.Add(_currentPosition);
                    }
                }
            }
        }

        private Coordinate2D GetNewPosition()
        {
            Coordinate2D newPosition = _currentDirection switch
            {
                Direction.N => _currentPosition with { Y = _currentPosition.Y + 1 },
                Direction.E => _currentPosition with { X = _currentPosition.X + 1 },
                Direction.S => _currentPosition with { Y = _currentPosition.Y - 1 },
                _ => _currentPosition with { X = _currentPosition.X - 1 },
            };
            return newPosition;

        }


        private Direction GetNewDirection(char c)
        {
            Direction newDirection;

            if (c == 'R')
            {
                newDirection = _currentDirection switch
                {
                    Direction.N => Direction.E,
                    Direction.S => Direction.W,
                    Direction.E => Direction.S,
                    _ => Direction.N,
                };
            }
            else
            {
                newDirection = _currentDirection switch
                {
                    Direction.N => Direction.W,
                    Direction.S => Direction.E,
                    Direction.E => Direction.N,
                    _ =>  Direction.S,
                };
            }

            
            return newDirection;
        }
    }
}
