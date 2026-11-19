using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;
using Microsoft.Extensions.Primitives;
using MoreLinq;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 2)]
    public class Year2023Day02: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2023Day02()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            //Build BasePath and retrieve input. 
 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
           
            _SW.Start();


            int sumofIds = FindPossibleGames(lines);

            _SW.Stop();

            output.WritePart(1, sumofIds, trackTime ? _SW.Elapsed : null, "Some of the IDs of possible games");

            
            _SW.Restart();

            int powerSetSum = FindPowerSetTotal(lines);
            
            _SW.Stop();

            output.WritePart(2, powerSetSum, trackTime ? _SW.Elapsed : null, "Power Set Sum");



        }

        public int FindPossibleGames(string[] lines)
        {
            int redCubesMax = 12;
            int greenCubesMax = 13;
            int blueClubesMax = 14;

            int sum = 0;
            foreach (string line in lines)
            {
                string[] gameplay = line.Split(":");

                int gameID = Convert.ToInt32(gameplay[0].Split(" ")[1]);


                string[] sets = gameplay[1].Split(";");

                bool possible = true;

                foreach (string s in sets)
                {
                    string[] colorValuePairs = s.Split(",");

                    foreach (string colorValuePair in colorValuePairs)
                    {
                        string[] parts = colorValuePair.Trim().Split(" ");

                        string color = parts[1];
                        int value = int.Parse(parts[0]);

                        if (color == "red" && value > redCubesMax)
                            possible = false;

                        if (color == "green" && value > greenCubesMax)
                            possible = false;

                        if (color == "blue" && value > blueClubesMax)
                            possible = false;

                    }
                }

                if (possible)
                    sum += gameID;
            }
            return sum;
        }

        public int FindPowerSetTotal(string[] lines)
        {
            int sum = 0;
            foreach (string line in lines)
            {
                string[] gameplay = line.Split(":");

                string[] sets = gameplay[1].Split(";");

                int blueVal = 0;
                int greenVal = 0;
                int redVal = 0;

                foreach (string s in sets)
                {

                    string[] colorValuePairs = s.Split(",");

                    foreach (string colorValuePair in colorValuePairs)
                    {
                        string[] parts = colorValuePair.Trim().Split(" ");

                        string color = parts[1];
                        int value = int.Parse(parts[0]);

                        if (color == "red" && value > blueVal)
                            blueVal = value;

                        if (color == "green" && value > greenVal)
                            greenVal = value;

                        if (color == "blue" && value > redVal)
                            redVal = value;

                    }
                }

                sum += (redVal * blueVal * greenVal);
            }
            return sum;
        }
    }
}
