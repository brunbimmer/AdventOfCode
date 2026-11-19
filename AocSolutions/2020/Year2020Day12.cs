using System;
using System.Diagnostics;
using AdventFileIO;
using Common;
using static Common.Utilities;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 12)]
    public class Year2020Day12 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day12()
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
            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            long distance = Part1(instructions);
            _SW.Stop();

            output.WritePart(1, distance, trackTime ? _SW.Elapsed : null, "Manhattan distance from the starting position");


            _SW.Restart();
            distance = Part2(instructions);
            _SW.Stop();

            output.WritePart(2, distance, trackTime ? _SW.Elapsed : null, "Manhattan distance using waypoint navigation");

        }

        private long Part1(string[] instructions)
        {
            Coordinate2DLong position = new Coordinate2DLong(0, 0);
            CompassDirection facing = CompassDirection.E;

            foreach (string instruction in instructions)
            {
                char action = instruction[0];
                int value = int.Parse(instruction.AsSpan(1));

                switch (action)
                {
                    case 'N':
                        position = position.MoveDirection(CompassDirection.N, distance: value);
                        break;
                    case 'S':
                        position = position.MoveDirection(CompassDirection.S, distance: value);
                        break;
                    case 'E':
                        position = position.MoveDirection(CompassDirection.E, distance: value);
                        break;
                    case 'W':
                        position = position.MoveDirection(CompassDirection.W, distance: value);
                        break;
                    case 'L':
                        facing = (CompassDirection)(((int)facing - value % 360 + 360) % 360);
                        break;
                    case 'R':
                        facing = (CompassDirection)(((int)facing + value % 360) % 360);
                        break;
                    case 'F':
                        position = position.MoveDirection(facing, distance: value);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown navigation action: {action}");
                }
            }

            return position.Magnitude();
        }

        private long Part2(string[] instructions)
        {
            Coordinate2DLong position = new Coordinate2DLong(0, 0);
            Coordinate2DLong waypoint = new Coordinate2DLong(10, 1);

            foreach (string instruction in instructions)
            {
                char action = instruction[0];
                int value = int.Parse(instruction.AsSpan(1));

                switch (action)
                {
                    case 'N':
                        waypoint = waypoint.MoveDirection(CompassDirection.N, distance: value);
                        break;
                    case 'S':
                        waypoint = waypoint.MoveDirection(CompassDirection.S, distance: value);
                        break;
                    case 'E':
                        waypoint = waypoint.MoveDirection(CompassDirection.E, distance: value);
                        break;
                    case 'W':
                        waypoint = waypoint.MoveDirection(CompassDirection.W, distance: value);
                        break;
                    case 'L':
                    case 'R':
                        int clockwiseDegrees = action == 'R' ? value % 360 : (360 - value % 360) % 360;
                        waypoint = clockwiseDegrees switch
                        {
                            0 => waypoint,
                            90 => new Coordinate2DLong(waypoint.Y, -waypoint.X),
                            180 => -waypoint,
                            270 => new Coordinate2DLong(-waypoint.Y, waypoint.X),
                            _ => throw new InvalidOperationException("Waypoint rotations must be multiples of 90 degrees.")
                        };
                        break;
                    case 'F':
                        position += value * waypoint;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown navigation action: {action}");
                }
            }

            return position.Magnitude();
        }
    }
}
