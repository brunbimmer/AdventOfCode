using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 9)]
    public class Year2022Day09 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        List<Coordinate2D> tPosVisited;

        public Year2022Day09()
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

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            tPosVisited = new List<Coordinate2D>();

            Coordinate2D hPos = new Coordinate2D(0,0);
            Coordinate2D tPos = new Coordinate2D(0,0);

            tPosVisited.Add(tPos);
            

            SW.Start();                       

            foreach (string instruction in instructions)
            {
                string direction = instruction.Split(' ').First();
                int steps = Convert.ToInt32(instruction.Split(' ').Last().Trim());

                switch(direction)
                {
                    case "R":

                        for(int i = 1; i <= steps; i++)
                        {
                            hPos = new Coordinate2D(hPos.X + 1, hPos.Y);
                            
                            tPos = DoMoveTail(tPos, hPos); 
                            
                            if (!tPosVisited.Contains(tPos))
                                tPosVisited.Add(tPos);
                        }
                        break;

                    case "U":
                        for(int i = 1; i <= steps; i++)
                        {
                            hPos = new Coordinate2D(hPos.X, hPos.Y + 1);
                            
                            tPos = DoMoveTail(tPos, hPos);                            

                            if (!tPosVisited.Contains(tPos))
                                tPosVisited.Add(tPos);
                        }
                        break;

                    case "L":
                        for(int i = 1; i <= steps; i++)
                        {
                            hPos = new Coordinate2D(hPos.X - 1, hPos.Y);
                            
                            tPos = DoMoveTail(tPos, hPos);                            

                            if (!tPosVisited.Contains(tPos))
                                tPosVisited.Add(tPos);
                        }
                        break;
                    case "D":
                        for(int i = 1; i <= steps; i++)
                        {
                            hPos = new Coordinate2D(hPos.X, hPos.Y - 1);
                            
                            tPos = DoMoveTail(tPos, hPos);                            

                            if (!tPosVisited.Contains(tPos))
                                tPosVisited.Add(tPos);
                        }
                        break;
                }
            }

            
            SW.Stop();

            Console.WriteLine("Part 1: Number of Visited Positions {0}, Execution Time: {1}", tPosVisited.Count, StopwatchUtil.getInstance().GetTimestamp(SW));

            tPosVisited.Clear();

            hPos = new Coordinate2D(0,0);

            Coordinate2D[] rope = new Coordinate2D[8];

            for (int i = 0; i < 8; i++)
            {
                rope[i] = new Coordinate2D(0,0);
            }

            tPos = new Coordinate2D(0,0);

            tPosVisited.Add(tPos);

            SW.Restart();    
            
            foreach (string instruction in instructions)
            {
                string direction = instruction.Split(' ').First();
                int steps = Convert.ToInt32(instruction.Split(' ').Last().Trim());

                switch(direction)
                {
                    case "R":

                        for(int i = 1; i <= steps; i++)
                        {
                            hPos = new Coordinate2D(hPos.X + 1, hPos.Y);
                            
                            rope[0] = DoMoveTail(rope[0], hPos);

                            for (int x = 1; x < rope.Length; x++)
                            {
                                rope[x] = DoMoveTail(rope[x], rope[x - 1]);
                            }

                            tPos = DoMoveTail(tPos, rope[7]); 
                            
                            if (!tPosVisited.Contains(tPos))
                                tPosVisited.Add(tPos);
                        }
                        break;

                    case "U":
                        for(int i = 1; i <= steps; i++)
                        {
                            hPos = new Coordinate2D(hPos.X, hPos.Y + 1);
                            
                            rope[0] = DoMoveTail(rope[0], hPos);
                            
                            for (int x = 1; x < rope.Length; x++)
                            {
                                rope[x] = DoMoveTail(rope[x], rope[x - 1]);
                            }
                            
                            tPos = DoMoveTail(tPos, rope[7]); 


                            if (!tPosVisited.Contains(tPos))
                                tPosVisited.Add(tPos);
                        }
                        break;

                    case "L":
                        for(int i = 1; i <= steps; i++)
                        {
                            hPos = new Coordinate2D(hPos.X - 1, hPos.Y);
                            
                            rope[0] = DoMoveTail(rope[0], hPos);
                            
                            for (int x = 1; x < rope.Length; x++)
                            {
                                rope[x] = DoMoveTail(rope[x], rope[x - 1]);
                            }

                            tPos = DoMoveTail(tPos, rope[7]); 

                            if (!tPosVisited.Contains(tPos))
                                tPosVisited.Add(tPos);
                        }
                        break;
                    case "D":
                        for(int i = 1; i <= steps; i++)
                        {
                            hPos = new Coordinate2D(hPos.X, hPos.Y - 1);
                            
                            rope[0] = DoMoveTail(rope[0], hPos);

                            for (int x = 1; x < rope.Length; x++)
                            {
                                rope[x] = DoMoveTail(rope[x], rope[x-1]);
                            }

                            tPos = DoMoveTail(tPos, rope[7]); 

                            if (!tPosVisited.Contains(tPos))
                                tPosVisited.Add(tPos);
                        }
                        break;
                }
            }

            
            SW.Stop();

            Console.WriteLine("Part 2: Number of Visited Positions {0}, Execution Time: {1}", tPosVisited.Count, StopwatchUtil.getInstance().GetTimestamp(SW));


        }   
        
        private Coordinate2D DoMoveTail(Coordinate2D tail, Coordinate2D head)
        {
            int xDiff = head.X - tail.X;
            int yDiff = head.Y - tail.Y;

            if (Math.Abs(xDiff) <= 1 && Math.Abs(yDiff) <= 1)
                return tail; //no move
            
            Coordinate2D newTail = null;
            //we are out of step in one or more directions.

            if (   (xDiff == 2 && yDiff == 1)
                || (xDiff == 1 && yDiff == 2)
                || (xDiff == 2 && yDiff == 2))
                
            {
                newTail = new Coordinate2D(tail.X + 1, tail.Y + 1);
            }
            else if ( (xDiff == -2 && yDiff == -1)
                 ||   (xDiff == -1 && yDiff == -2)
                 ||   (xDiff == -2 && yDiff == -2) )
            {
                newTail = new Coordinate2D(tail.X - 1, tail.Y - 1);
            }
            else if ( (xDiff == 2 && yDiff == -1)
                ||    (xDiff == 1 && yDiff == -2)
                ||    (xDiff == 2 && yDiff == -2) )
            {
                newTail = new Coordinate2D(tail.X + 1, tail.Y - 1);
            }
            else if ( (xDiff == -2 && yDiff == 1)
                ||    (xDiff == -1 && yDiff == 2)
                ||    (xDiff == -2 && yDiff == 2) )
            {
                newTail = new Coordinate2D(tail.X - 1, tail.Y + 1);
            }
            else if (xDiff == 2)
               newTail = new Coordinate2D(tail.X + 1, tail.Y);
            else if (xDiff == -2)
               newTail = new Coordinate2D(tail.X - 1, tail.Y);
            else if (yDiff == 2)
               newTail = new Coordinate2D(tail.X, tail.Y + 1);
            else if (yDiff == -2)
               newTail = new Coordinate2D(tail.X, tail.Y - 1);

            return newTail;

        }
    }
}
