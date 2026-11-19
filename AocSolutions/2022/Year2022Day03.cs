using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using Microsoft.VisualBasic;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 3)]
    public class Year2022Day03 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day03()
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

            int sum = CalculateSumOfPriorities(lines);
            
            _SW.Stop();

            output.WritePart(1, sum, trackTime ? _SW.Elapsed : null, "Sum of the priorities");

            _SW.Restart();

            sum = CalculateSumOfGroups(lines);
            
            _SW.Stop();

            output.WritePart(2, sum, trackTime ? _SW.Elapsed : null, "Sum of the priorities");


        }      
        
        private int CalculateSumOfPriorities(string[] lines)
        {


            int sum = 0;
            string bar = "|";
            string[] splitStr = new string[] {bar};
            foreach (string line in lines)
            {
                string[] parts = line.Insert(line.Length / 2, bar).Split(splitStr,StringSplitOptions.RemoveEmptyEntries);
                
                char intersection = parts[0].Intersect(parts[1]).First();
                sum += CalculateLetterValue(intersection);
            }
            return sum;
        }

        private int CalculateSumOfGroups(string[] lines)
        {
            int numOfGroups = lines.Length / 3;

            int sum = 0;
            
            for(int group = 1; group <= numOfGroups; group++)
            {
                int index = (group * 3) - 3;  //convert to 0 based indexing
                
                char intersection = lines[index].Intersect(lines[index + 1 ].Intersect(lines[index + 2])).First();                
                sum += CalculateLetterValue(intersection);
            }
            
            return sum;
        }

        private int CalculateLetterValue(char character)
        {
            int ascii = (int)character;
            if (Char.IsUpper(character)) 
                return (ascii - 38);
            else
                return (ascii - 96);
        }
    }
}
