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
using System.Transactions;
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

        public Stopwatch _SW { get; set; }

        public Year2022Day09()
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

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            Dictionary<Coordinate2D, bool> visitedPart1 = new Dictionary<Coordinate2D, bool>();
            Dictionary<Coordinate2D, bool> visitedPart2 = new Dictionary<Coordinate2D, bool>();

            Coordinate2D hPos = new Coordinate2D(0,0);


            hPos = new Coordinate2D(0,0);

            Coordinate2D[] rope = new Coordinate2D[8];

            for (int i = 0; i < 8; i++)                 //in Part 1, rope[0] is the tail as there is only two knots.
                rope[i] = new Coordinate2D(0,0);

            Coordinate2D tPos = new Coordinate2D(0,0);

            visitedPart1[rope[0]] = true;
            visitedPart2[tPos] = true;

            _SW.Restart();    
            
            foreach (string instruction in instructions)
            {
                string direction = instruction.Split(' ').First();
                int steps = Convert.ToInt32(instruction.Split(' ').Last().Trim());

                for(int i = 1; i <= steps; i++)
                {

                    switch (direction)
                    {
                       case "R":
                            hPos = new Coordinate2D(hPos.X + 1, hPos.Y);
                            break;
                        case "L":
                            hPos = new Coordinate2D(hPos.X - 1, hPos.Y);
                            break;
                        case "U":
                            hPos = new Coordinate2D(hPos.X, hPos.Y + 1);
                            break;
                        case "D":
                            hPos = new Coordinate2D(hPos.X, hPos.Y - 1);
                            break;
                    }    
                    
                    bool _bMove;
                    (rope[0], _bMove) = DoMoveTail(rope[0], hPos);
                    visitedPart1[rope[0]] = true;

                    if (_bMove == false) continue;      //continue to next step. No more moves to make with the train.

                    for (int x = 1; x < rope.Length; x++)
                    {
                        (rope[x], _bMove) = DoMoveTail(rope[x], rope[x - 1]);
                        if (_bMove == false) break;  //continue to next step. No more moves to make with the train.
                    }
                        
                    (tPos, _bMove) = DoMoveTail(tPos, rope[7]); 
                            
                    visitedPart2[tPos] = true;
                }
            }
            
            _SW.Stop();

            Console.WriteLine("Part 1: Number of Positions Tail visited (Two Knots) -> {0}", visitedPart1.Count);
            Console.WriteLine("Part 2: Number of Positions Tail visited (10 knots)  -> {0}", visitedPart2.Count);
            Console.WriteLine("  Total Execution Time: {0}" , StopwatchUtil.getInstance().GetTimestamp(_SW));

        }   
        
        private (Coordinate2D, bool) DoMoveTail(Coordinate2D tail, Coordinate2D head)
        {
            (int xDiff, int yDiff) = head.Difference(tail);

            if (Math.Abs(xDiff) <= 1 && Math.Abs(yDiff) <= 1)
                return (tail, false); //no move
            
            Coordinate2D newTail = null;
            //we are out of step in one or more directions.

            if (xDiff >= 1 && yDiff >= 1)
                newTail = new Coordinate2D(tail.X + 1, tail.Y + 1);
            else if (xDiff <= -1 && yDiff <= -1)
                newTail = new Coordinate2D(tail.X - 1, tail.Y - 1);
            else if (xDiff >= 1 && yDiff <= -1)
                newTail = new Coordinate2D(tail.X + 1, tail.Y - 1);
            else if (xDiff <= -1 && yDiff >= 1)
                newTail = new Coordinate2D(tail.X - 1, tail.Y + 1);
            else if (xDiff == 2)
               newTail = new Coordinate2D(tail.X + 1, tail.Y);
            else if (xDiff == -2)
               newTail = new Coordinate2D(tail.X - 1, tail.Y);
            else if (yDiff == 2)
               newTail = new Coordinate2D(tail.X, tail.Y + 1);
            else if (yDiff == -2)
               newTail = new Coordinate2D(tail.X, tail.Y - 1);

            return (newTail, true);

        }
    }
}
