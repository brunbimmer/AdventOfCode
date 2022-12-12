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
    [AdventOfCode(Year = 2015, Day = 19)]
    public class Year2015Day19 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day19()
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

            string input = FileIOHelper.getInstance().ReadDataAsString(file);

            string instructions = input.Split(new string[] { "\n\n" }, StringSplitOptions.None).First();
            string molecule = input.Split(new string[] { "\n\n" }, StringSplitOptions.None).Last().Trim();

            string[] instructionSet = instructions.Split(new string[] { "\n" }, StringSplitOptions.None);

            //make a backup copy for part 2
            string original_molecule = new string(molecule);

            SW.Start();                       

            int distinctMolecules = CalculateDistinctMolecules(instructionSet, molecule);

            
            SW.Stop();

            Console.WriteLine("Part 1: Number of Distinct Molecules {0}, Execution Time: {1}", distinctMolecules, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            int fewestSteps = SearchForMedicine(instructionSet, original_molecule);
            
            SW.Stop();

            Console.WriteLine("Part 2: Fewest steps to make medicine {0}, Execution Time: {1}", fewestSteps, StopwatchUtil.getInstance().GetTimestamp(SW));


        }     
        
        private int CalculateDistinctMolecules(string[] instructionSet, string molecule)
        {
            List<string> newMolecules = new List<string>();

            foreach(string instruction in instructionSet)
            {
                string key = instruction.Split("=>", StringSplitOptions.TrimEntries).First();
                string val = instruction.Split("=>", StringSplitOptions.TrimEntries).Last();

                foreach(Match m in Regex.Matches(molecule, key, RegexOptions.Compiled))
                {
                    string s = molecule.Remove(m.Index, key.Length).Insert(m.Index, val);
                    newMolecules.Add(s);
                }                               
            }

            return newMolecules.Distinct().Count();
        }

        private int SearchForMedicine(string[] instructionSet, string molecule)
        {
            int cnt = 0;
            while (!molecule.Equals("e"))
            {
                foreach(string instruction in instructionSet)
                {
                    string key = instruction.Split("=>", StringSplitOptions.TrimEntries).First();
                    string val = instruction.Split("=>", StringSplitOptions.TrimEntries).Last();

                    if (molecule.Contains(val))
                    {
                        Regex regex = new Regex(val);
                        molecule = regex.Replace(molecule, key, 1);
                        cnt++;
                    }
                }
            }
            return cnt;
        }
    }
}
