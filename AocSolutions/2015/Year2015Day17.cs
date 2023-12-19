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

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            int[] containers = FileIOHelper.getInstance().ReadDataToIntArray(file);

            _SW.Start();                       

            var query = CalculateSum(containers);
                        
            _SW.Stop();

            Console.WriteLine("Part 1: Combinations of Containers totaling 150: {0}, Execution Time: {1}", query.Count(), StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            
            int minContainers = query.GroupBy(x => x.Count())
                                    .OrderBy(x => x.Key)
                                    .First()
                                    .Count();
            
            _SW.Stop();

            Console.WriteLine("Part 2: Min # of Containers to fill 150L of Egg Nog: {0}, Execution Time: {1}", minContainers, StopwatchUtil.getInstance().GetTimestamp(_SW));


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
