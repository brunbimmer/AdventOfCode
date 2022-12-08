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
    [AdventOfCode(Year = 2021, Day = 17)]
    public class Year2021Day17 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2021Day17()
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

            int x1, x2, y1, y2;
            
            (x1, x2, y1, y2) = ReadTargetCoordinates(file);

            SW.Start();                       

            int yMax = CalculateParabolicVertex(y1);
            
            SW.Stop();

            Console.WriteLine("Part 1: Highest Y position is {0}", yMax);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            int nHits = CalculateInitialVelocityValuesOptimized(x1, x2, y1, y2);
            
            SW.Stop();

            Console.WriteLine("Part 2: Number of distinct initial velocity values that cause probe to hit target area: {0}", nHits);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


        }       

        private   (int, int, int, int) ReadTargetCoordinates(string filename)
        {
            string target = File.ReadAllText(filename);
            int xPosStart = target.IndexOf("x") + 2;
            int xPosEnd = target.IndexOf(",");

            int yPosStart = target.IndexOf("y") + 2;
            
            string xRange = target.Substring(xPosStart, xPosEnd - xPosStart);
            string yRange = target.Substring(yPosStart, target.Length - yPosStart);

            string[] xRangeSplit = xRange.Split("..");
            string[] yRangeSplit = yRange.Split("..");

            return (int.Parse(xRangeSplit[0]), int.Parse(xRangeSplit[1]), int.Parse(yRangeSplit[0]), int.Parse(yRangeSplit[1]));
        }      
        
        private   int CalculateParabolicVertex(int y1)
        {
            //Getting the maximum height is about simple algebra. Let's ignore position and
            //just focus on the y coordinates.
            //We can get the maximum Y value by using the following formula
            //
            //  
            //      yMax = y1 * (y1 + 1) / 2        //y1 is the lowest edge, which is y1.
            //
            //
            int yMax = y1 * (y1 + 1) / 2;
            return yMax;
        
        }

        private   int CalculateInitialVelocityValues(int x1, int x2, int y1, int y2)
        {
            int successfulHits = 0;

            //so we test initial velocities between starting with 1 and going to the maximum x-Value
            //and starting from minimum (which I think could be tightened) and having an relatively
            //high upper Y boundary. Boundary will affect performance. 

            //since Posting solution, there is a mathematical formula to posting

            for(int x = 1; x <= x2; x++)
                for (int y = y1; y <= 1000; y++)
                {
                    successfulHits += SimulateShots(x, y, x1, x2, y1, y2);
                }

            return successfulHits;
        }

        private   int CalculateInitialVelocityValuesOptimized(int x1, int x2, int y1, int y2)
        {
            int successfulHits = 0;

            //since Posting solution, there is a mathematical narrow the x velocity window

            int xStart = (int)Math.Round(Math.Sqrt(x1 * 2));

            for (int x = xStart; x <= x2; x++)
                for (int y = y1; y <= 1000; y++)
                {
                    successfulHits += SimulateShots(x, y, x1, x2, y1, y2);
                }

            return successfulHits;
        }


        private   int SimulateShots(int xVel, int yVel, int x1, int x2, int y1, int y2)
        {
            int x = 0;
            int y = 0;
            
            while (true)
            {
                x += xVel;
                y += yVel;

                if ((x1 <= x && x <= x2) && (y1 <= y && y <= y2)) return 1;

                if (yVel < 0 && y < y1) return 0;

                if (xVel > 0) xVel --;
                yVel -= 1;

            }
        }
    }
}
