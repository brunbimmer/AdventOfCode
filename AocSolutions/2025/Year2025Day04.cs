using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;
using MoreLinq.Extensions;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2025, Day = 4)]
    public class Year2025Day04: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day04()
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

            //Build BasePath and retrieve input. 
 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            var (input, width, height) = FileIOHelper.getInstance().GetDataAsCharMap(file);

            _SW.Start();

            int rollsOfPaper = CountAccessibleRolls(new Dictionary<Coordinate2D, char>(input), singleIteration: true);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Rolls Accessible by Forklift : {rollsOfPaper}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            int totalAccessible = CountAccessibleRolls(input, singleIteration: false);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Total Rolls Removed : {totalAccessible}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }

        public int CountAccessibleRolls(Dictionary<Coordinate2D, char> grid, bool singleIteration = false)
        {
            int totalRemoved = 0;

            while (true)
            {
                List<Coordinate2D> toRemove = new List<Coordinate2D>();

                // Find all rolls that are currently accessible
                foreach (var pos in grid.Keys.Where(k => grid[k] == '@'))
                {
                    int adjacentRolls = 0;
                    
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            
                            var adjacent = new Coordinate2D(pos.X + dx, pos.Y + dy);
                            if (grid.ContainsKey(adjacent) && grid[adjacent] == '@')
                            {
                                adjacentRolls++;
                            }
                        }
                    }

                    // If accessible, mark for removal
                    if (adjacentRolls < 4)
                    {
                        toRemove.Add(pos);
                    }
                }

                // If no accessible rolls found, we're done
                if (toRemove.Count == 0)
                    break;

                // Remove all accessible rolls
                foreach (var pos in toRemove)
                {
                    grid.Remove(pos);
                    totalRemoved++;
                }

                // If single iteration mode, stop after one iteration
                if (singleIteration)
                    break;
            }

            return totalRemoved;
        }
    }
}
