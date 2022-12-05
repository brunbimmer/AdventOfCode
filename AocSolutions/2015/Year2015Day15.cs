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
    [AdventOfCode(Year = 2015, Day = 15)]
    public class Year2015Day15 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day15()
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

            //Input is simple, use it directly instead of spending time reading it in.           

            SW.Start();                       

            long maxSum = FindMaximum();
           
            SW.Stop();

            Console.WriteLine("Part 1: Total score of highest-scoring cookie {0}, Execution Time: {1}", maxSum, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            long maxSumCalorie = FindMaximum(true);
            
            SW.Stop();

            Console.WriteLine("Part 1: Total score of highest-scoring cookie with 500 calories {0}, Execution Time: {1}", maxSumCalorie, StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("\n===========================================\n");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }       

        private long FindMaximum(bool calorieCount = false)
        {
            long maximum = 0;

            for (int a = 0; a < 100; a++)               //capacity
            {
                for (int b = 0; b < 100; b++)           //durability
                {
                    for (int c = 0; c < 100; c++)       //flavor
                    {
                        for (int d = 0; d < 100; d++)   //texture
                        { 
                            long value = 0;                                   
                            if (a + b + c + d != 100)
                                continue;

                            if (calorieCount)
                            {
                                int calories = a * 3 + b * 3 + c * 8 + d * 8;

                                if (calories != 500) continue;
                            }

                            long capacity =   Math.Max(a * 2, 0);
                            long durability = Math.Max(b * 5 +  d * -1, 0);
                            long flavor =     Math.Max(a * -2 + b * -3 + c * 5, 0);
                            long texture =    Math.Max(c * -1 +  d * 5, 0);
                            


                            value = capacity * durability * flavor * texture;

                            if (value > maximum) 
                                maximum = value;

                            
                        }                    
                    }    
                }                               
            }

            return maximum;
        }
    }
}
