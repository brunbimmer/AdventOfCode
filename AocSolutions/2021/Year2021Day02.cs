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
    [AdventOfCode(Year = 2021, Day = 2)]
    public class Year2021Day2 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2021Day2()
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

            List<(string, string)> directions = FileIOHelper.getInstance().ReadDataAsTupleList(file);

            int distanceX, distanceY;

            SW.Start();
            (distanceX, distanceY) = Part1(directions);
            SW.Stop();



            Console.WriteLine("  Part 1: Final Horizontal Position ({0}) * Final Depth ({1}): {2}", distanceX, distanceY, distanceX * distanceY);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            (distanceX, distanceY) = Part2(directions);
            SW.Stop();

            Console.WriteLine("Part 2: Final Horizontal Position ({0}) * Final Depth ({1}): {2}", distanceX, distanceY, distanceX * distanceY);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


        }

        private (int, int) Part1(List<(string, string)> directions)
        {
            int distanceX = directions.Where(x => x.Item1 == "forward").ToList().Sum(x => int.Parse(x.Item2));
            int numOfUps = directions.Where(x => x.Item1 == "up").ToList().Sum(x => int.Parse(x.Item2));
            int numOfDowns = directions.Where(x => x.Item1 == "down").ToList().Sum(x => int.Parse(x.Item2));

            int distanceY = (numOfDowns - numOfUps);

            return (distanceX, distanceY);
        }

        private (int, int) Part2(List<(string, string)> directions)
        {
            int distanceX = 0;
            int distanceY = 0;

            int aim = 0;

            foreach (var direction in directions)
            {
                switch (direction.Item1)
                {
                    case "down":
                        aim = aim + int.Parse(direction.Item2);
                        break;
                    case "up":
                        aim = aim - int.Parse(direction.Item2);
                        break;
                    case "forward":
                        distanceX = distanceX + int.Parse(direction.Item2);
                        distanceY = distanceY + int.Parse(direction.Item2) * aim;
                        break;
                    default:
                        break;
                }
            }

            return (distanceX, distanceY);

        }
    }
}