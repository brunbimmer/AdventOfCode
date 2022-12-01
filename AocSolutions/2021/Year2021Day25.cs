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
    [AdventOfCode(Year = 2021, Day = 25)]
    public class Year2021Day25 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        record Coordinate(int X, int Y); 

        private  Dictionary<Coordinate, int> MasterCucumberGrid = new();

        private  int MaxX = 0;
        private  int MaxY = 0;


        public Year2021Day25()
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

            InitializeDataSet(file);

            int steps = Part1();
            
            SW.Stop();

            Console.WriteLine("SeaCucumbers stop moving at Step #:  {0}", steps);     
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("\n===========================================\n");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }       

        private  void InitializeDataSet(string filename)
        {
            string[] dataInput = File.ReadAllLines(filename);

            MaxX = dataInput[0].Length - 1;     //zero-based length
            MaxY = dataInput.Length - 1;        //zero-based length

            var cucumberGrid = dataInput.Select(x => x.ToArray()).ToArray();

            for (int y = 0; y < cucumberGrid.Length; y++)
                for (int x = 0; x < cucumberGrid[y].Length; x++)
                {
                    var value = cucumberGrid[y][x] switch
                    {
                        '>' => 1,                           //value of 1 represents horizontal cucumber
                        'v' => 2,                           //value of 2 represents vertical cucumber
                        _ => 0,                             //default value, nothing
                    };
                    MasterCucumberGrid.Add(new Coordinate(x, y), value);
                }                   
        }

        private  int Part1()
        {
            //make a copy so we can keep the original.
            var cucumberGrid = new Dictionary<Coordinate, int>(MasterCucumberGrid);
            int steps = 0;
            while (true)
            {
                steps += 1;
                bool movedHeard1 = false;
                bool movedHeard2 = false;
                //move ">" (i.e., cucumberType 1);
                (movedHeard1, cucumberGrid) = MoveCucumbers(cucumberGrid, 1);

                //move "v" (i.e., cucumberType 2);
                (movedHeard2, cucumberGrid) = MoveCucumbers(cucumberGrid, 2);

                if (movedHeard1 == false && movedHeard2 == false) 
                    break;                
            }
            return steps;
        }


        private  (bool, Dictionary<Coordinate, int>) MoveCucumbers(Dictionary<Coordinate, int> cucumberGrid, int cucumberType)
        {
            bool canMove = false;
            var newCucumberGrid = new Dictionary<Coordinate, int>(cucumberGrid);

            //find all cucumbers by their cucumber types.
            var seaCucumberList = cucumberGrid.Where(x => x.Value == cucumberType).ToList();
            Parallel.ForEach(seaCucumberList, seaCucumber =>
               {
                   var originalCoord = seaCucumber.Key;
                   int target;
                   Coordinate targetCoord;

                    //cumberType of 1 is represents we move horizontal
                    if (cucumberType == 1)
                   {
                       target = (seaCucumber.Key.X == MaxX) ? 0 : seaCucumber.Key.X + 1;
                       targetCoord = new Coordinate(target, seaCucumber.Key.Y);
                   }
                   else
                   {
                       target = (seaCucumber.Key.Y == MaxY) ? 0 : seaCucumber.Key.Y + 1;
                       targetCoord = new Coordinate(seaCucumber.Key.X, target);
                   }

                    //is position occupied?
                    if (cucumberGrid[targetCoord] == 0)
                   {
                       canMove = true;
                       newCucumberGrid[originalCoord] = 0;
                       newCucumberGrid[targetCoord] = cucumberType;
                   }
               });
            return (canMove, newCucumberGrid);
        }
    }
}