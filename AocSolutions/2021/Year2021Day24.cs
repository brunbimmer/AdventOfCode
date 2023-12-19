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
    [AdventOfCode(Year = 2021, Day = 24)]
    public class Year2021Day24 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        
        private  List<long> addX = new();
        private  List<long> divZ = new();
        private  List<long> addY = new();
        private  List<long> MaxZAtStep = new();
        private  Dictionary<(int groupNum, long prevZ), List<string>> cache = new();
        private  List<string> ValidModelNumbers;

        public Year2021Day24()
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

            var input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       

            //took some time to visually review the operations. Operation order is similar for all digit locations
            //with only minor, minor differences with the operands. Extract the operations that are unique per
            //position. All other operations that are common performed within the Recursive search algorithm below.
            for (int i = 0; i < 14; i++)
            {
                divZ.Add(int.Parse(input[(18 * i) + 4].Split()[2]));      //4  operation after the inp w operation
                addX.Add(int.Parse(input[(18 * i) + 5].Split()[2]));      //5  operation after the inp w operation
                addY.Add(int.Parse(input[(18 * i) + 15].Split()[2]));     //15 operation after the inp w operation
            }

            //We can only divide by 26 so many times at each step, at some point we can bail early. 
            for (int i = 0; i < divZ.Count; i++)
            {
                MaxZAtStep.Add(divZ.Skip(i).Aggregate(1L, (a, b) => a * b));
            }
            ValidModelNumbers = RecursiveSearch(0, 0).ToList();
            ValidModelNumbers.Sort();

            string largestNumber = ValidModelNumbers.Last();        //Part 1
            string smallestNumber = ValidModelNumbers.First();      //Part 2
           
            _SW.Stop();

            Console.WriteLine("Largest Model Number accepted by Monad:  {0}", largestNumber);
            Console.WriteLine("Smallest Model Number accepted by Monad: {0}", smallestNumber);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       

         private  long RunALUOperation(int modelNumberPosition, long prevZ, long input)
        {
            long z = prevZ;
            long x = addX[modelNumberPosition] + z % 26;
            z /= divZ[modelNumberPosition];
            if (x != input)
            {
                z *= 26;
                z += input + addY[modelNumberPosition];
            }

            return z;
        }

        private  List<string> RecursiveSearch(int modelNumberPosition, long prevZ)
        {
            //We've Been here before...
            if (cache.ContainsKey((modelNumberPosition, prevZ))) return cache[(modelNumberPosition, prevZ)];
            
            //We've gon past the end. Return
            if (modelNumberPosition >= 14)
            {
                if (prevZ == 0) return new() { "" };   //return empty string when prevZ value is 0
                return null;
            }

            //if the prevZ input value is greater then the max possible Zvalue at specified position, we return
            //as we don't care to proceed further
            if (prevZ > MaxZAtStep[modelNumberPosition]) return null; 

            List<string> res = new();

            long nextX = addX[modelNumberPosition] + prevZ % 26;

            List<string> nextStrings;
            long nextZ;

            //if our X value is between 1 and 10, we proceed to next position to
            //and perform another recursive search (this method) with our nextZ
            //result when running the ALUOperation.
            if (0 < nextX && nextX < 10)
            {
                nextZ = RunALUOperation(modelNumberPosition, prevZ, nextX);
                nextStrings = RecursiveSearch(modelNumberPosition + 1, nextZ);
                if (null != nextStrings)
                {
                    foreach (var s in nextStrings)
                    {
                        res.Add($"{nextX}{s}");
                    }
                }
            }
            else
            {
                //X is 0, so we then enumerate between the values of 1 through 9 to
                //get every possible nextZ value from the ALU operation and then
                //pass on each result into "this" method to determine possible outcomes.
                foreach (int i in Enumerable.Range(1, 9))
                {
                    nextZ = RunALUOperation(modelNumberPosition, prevZ, i);
                    nextStrings = RecursiveSearch(modelNumberPosition + 1, nextZ);

                    if (null != nextStrings)
                    {
                        foreach (var s in nextStrings)
                        {
                            res.Add($"{i}{s}");
                        }
                    }
                }
            }
            cache[(modelNumberPosition, prevZ)] = res;
            return res;
        }
    }
}
