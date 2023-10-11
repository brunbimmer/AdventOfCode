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
    [AdventOfCode(Year = 2022, Day = 20)]
    public class Year2022Day20 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2022Day20()
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
            
            SW.Start();                       
            int[] numbers = FileIOHelper.getInstance().ReadDataToIntArray(file);
            int[] moved = new int[numbers.Length];
            
            List<(int, int)> elements = numbers.Zip(moved).ToList();

            while (elements.Where(x => x.Item2 == 0 && x.Item1 != 0).Count() > 0)
            {                
                int index = elements.Select((n, index) => new { n, index })
                                      .Where(pair => pair.n.Item2 == 0 && pair.n.Item1 != 0)
                                      .Select(pair => (int) pair.index)
                                      .FirstOrDefault();


                int value = elements[index].Item1;
                int _tempNewPositionDiff = mod(value, elements.Count);
                int newIndex = 0;

                if (index + _tempNewPositionDiff > elements.Count)
                {
                    newIndex = _tempNewPositionDiff - (elements.Count - index) + 1;
                }
                else
                {
                    newIndex = index + _tempNewPositionDiff;

                    if (value <= 0) newIndex -= 1;      //we transported a negative # into a position direction. Take away 1 for final positioning
                }

                elements.RemoveAt(index);
                elements.Insert(newIndex, (value, 1));

            }

            //elements are now shuffled.
            
            int sum = FindNumberAt(1000, elements) + FindNumberAt(2000, elements) + FindNumberAt(3000, elements);


            SW.Stop();

            Console.WriteLine("Part 1: Sum of the three numbers forming coordinates: {0}   Execution Time: {1}", sum, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            
            SW.Stop();

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


        }    
        
        int FindNumberAt(int position, List<(int, int)> elements)
        {
            int index = elements.Select((n, index) => new { n, index })
                                      .Where(pair => pair.n.Item1 == 0)
                                      .Select(pair => (int) pair.index)
                                      .FirstOrDefault();

            //cut down on movements based on modular arithmetic
            int NumberOfHops = position % elements.Count;
            int finalPosition = 0;
            if (index + NumberOfHops >= elements.Count)
            {
                NumberOfHops -= (elements.Count - index);
                finalPosition = NumberOfHops;
            }
            else
            {
                finalPosition = index + NumberOfHops;
            }


            return elements[finalPosition].Item1;                
        }

        int mod(int x, int y)
        {
           int t = x - ((x / y) * y);
           if (t < 0) t += y;
           return t;
        }
    }
}
