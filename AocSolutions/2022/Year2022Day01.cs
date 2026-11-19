using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 1)]
    public class Year2022Day01: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        List<List<int>> calorieCollection = new List<List<int>>();

        public Year2022Day01()
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
            
            ParseInput(lines);

            int maxElfSum = calorieCollection.Max(x => x.Sum());

            _SW.Stop();

            output.WritePart(1, maxElfSum, trackTime ? _SW.Elapsed : null, "Total calories carried by an elf");

            
            _SW.Restart();

            var orderedSumList = calorieCollection.OrderByDescending(x => x.Sum()).ToList().Take(3).ToList();

            int topThreeElfSum = orderedSumList.Sum(x => x.Sum());
            
            _SW.Stop();

            output.WritePart(2, topThreeElfSum, trackTime ? _SW.Elapsed : null, "Total calories carried by three elves");



        }

        private void ParseInput(string[] data)
        {
            List<int> calorieInput = new List<int>();
            foreach(string line in data)
            {
                if (line.Trim() == "")                
                {
                    calorieCollection.Add(calorieInput);
                    calorieInput = new List<int>();
                }
                else
                {
                    calorieInput.Add(Convert.ToInt32(line));
                }
            }
        }
    }
}
