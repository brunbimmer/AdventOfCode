using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 16)]
    public class Year2015Day16 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        internal class Sue
        {
            public int children;
            public int cats;
            public int samoyeds;
            public int pomeranians;
            public int akitas;
            public int vizslas;
            public int goldfish;
            public int trees;
            public int cars;
            public int perfumes;

            public Sue(int children, int cats, int samoyeds, int pomeranians, int akitas, int vizslas, int goldfish, int trees, int cars, int perfumes)
            {
                this.children = children;
                this.cats = cats;
                this.samoyeds = samoyeds;
                this.pomeranians = pomeranians;
                this.akitas = akitas;
                this.vizslas = vizslas;
                this.goldfish = goldfish;
                this.trees = trees;
                this.cars = cars;
                this.perfumes = perfumes;
            }

            public int MatchProbability(Sue sue)
            {
                int p = 0;

                if (sue.children == children)
                    p++;
                else if (children != -1)
                    p--;
                
                if (sue.trees == trees)
                    p++;
                else if (trees != -1)
                    p--;
                
                if (sue.goldfish == goldfish)
                    p++;
                else if (goldfish != -1)
                    p--;
                
                if (sue.cats == cats)
                    p++;
                else if (cats != -1)
                    p--;                
                
                if (sue.samoyeds == samoyeds)
                    p++;
                else if (samoyeds != -1)
                    p--;
                                
                if (sue.pomeranians == pomeranians)
                    p++;
                else if (pomeranians != -1)
                    p--;
                                
                if (sue.akitas == akitas)
                    p++;
                else if (akitas != -1)
                    p--;
                
                if (sue.vizslas == vizslas)
                    p++;
                else if (vizslas != -1)
                    p--;

                if (sue.cars == cars)
                    p++;
                else if (cars != -1)
                    p--;

                if (sue.perfumes == perfumes)
                    p++;
                else if (perfumes != -1)
                    p--;
                
                return p;
            }

            public int MatchProbability2(Sue sue)
            {
                int p = 0;

                if (sue.children == children)
                    p++;
                else if (children != -1)
                    p--;
                
                if (sue.trees < trees)
                    p++;
                else if (trees != -1)
                    p--;
                
                if (sue.goldfish > goldfish)
                    p++;
                else if (goldfish != -1)
                    p--;
                
                if (sue.cats < cats)
                    p++;
                else if (cats != -1)
                    p--;                
                
                if (sue.samoyeds == samoyeds)
                    p++;
                else if (samoyeds != -1)
                    p--;
                                
                if (sue.pomeranians > pomeranians)
                    p++;
                else if (pomeranians != -1)
                    p--;
                                
                if (sue.akitas == akitas)
                    p++;
                else if (akitas != -1)
                    p--;
                
                if (sue.vizslas == vizslas)
                    p++;
                else if (vizslas != -1)
                    p--;

                if (sue.cars == cars)
                    p++;
                else if (cars != -1)
                    p--;

                if (sue.perfumes == perfumes)
                    p++;
                else if (perfumes != -1)
                    p--;
                
                return p;
            }
        }


        public Year2015Day16()
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

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       

            Sue perfectSue = new Sue(3,7,2,3,0,0,5,3,2,1);

            Sue[] allSues = new Sue[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                string[] data = input[i].Split(' ');

                int children = -1;
                int cats = -1;
                int samoyeds = -1;
                int akitas = -1;
                int vizslas = -1;
                int goldfish = -1;
                int trees = -1;
                int cars = -1;
                int perfumes = -1;
                int pomeranians = -1;

                // etc.
                for (int j = 0; j < data.Length; j++)
                {
                    switch (data[j])
                    {
                        case "children:":
                            children = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "cats:":
                            cats = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "samoyeds:":
                            samoyeds = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "akitas:":
                            akitas = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "vizslas:":
                            vizslas = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "goldfish:":
                            goldfish = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "trees:":
                            cats = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "cars:":
                            cars = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "perfumes:":
                            perfumes = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;
                        case "pomeranians:":
                            pomeranians = int.Parse(new string(data[j + 1]
                                .TakeWhile(c => char.IsDigit(c)).ToArray()));
                            break;

                        
                    }
                }
                allSues[i] = new Sue(children, cats, samoyeds, pomeranians, akitas, vizslas, goldfish, trees, cars, perfumes);
            }

            int topPoints = 0;
            int topPoints2 = 0;

            int perfectSuePart1 = 0;
            int perfectSuePart2 = 0;
            for (int i = 0; i < allSues.Length; i++)
            {
                int points = allSues[i].MatchProbability(perfectSue);
                int points2 = allSues[i].MatchProbability2(perfectSue);
                if (points > topPoints)
                {
                    topPoints = points;
                    perfectSuePart1 = i + 1;
                }

                if (points2 > topPoints2)
                {
                    topPoints2 = points2;
                    perfectSuePart2 = i + 1;
                }
            }
                        
            _SW.Stop();

            Console.WriteLine("Part 1: Matched up Sue {0}, Execution Time: {1}", perfectSuePart1, StopwatchUtil.getInstance().GetTimestamp(_SW));
            Console.WriteLine("Part 2: Matched up Sue {0}, Execution Time: {1}", perfectSuePart2, StopwatchUtil.getInstance().GetTimestamp(_SW));         
            
            _SW.Stop();


        }       
    }
}
