using System;
using System.Collections;
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
    [AdventOfCode(Year = 2015, Day = 17)]
    public class Year2015Day17 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private const int _TotalLiters = 150;

        public Year2015Day17()
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

            int[] containers = FileIOHelper.getInstance().ReadDataToIntArray(file);

            _SW.Start();                       

            var query = CalculateSum(containers);
                        
            _SW.Stop();

            output.WritePart(1, query.Count(), trackTime ? _SW.Elapsed : null, "Combinations of Containers totaling 150");

            _SW.Restart();

            
            int minContainers = query.GroupBy(x => x.Count())
                                    .OrderBy(x => x.Key)
                                    .First()
                                    .Count();
            
            _SW.Stop();

            output.WritePart(2, minContainers, trackTime ? _SW.Elapsed : null, "Min # of Containers to fill 150L of Egg Nog");


        }       

        public IEnumerable<IEnumerable<int>> CalculateSum(int[] input)
        {
            var query = Enumerable
            .Range(1, (1 << input.Count()) - 1)
            .Select(index => input.Where((item, idx) => ((1 << idx) & index) != 0))
            .Where(x => x.Sum() == 150);

            return query;
        }
    }
}
