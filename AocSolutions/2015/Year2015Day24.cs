using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;
using LINQPad;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 24)]
    public class Year2015Day24 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2015Day24()
        {
            //Get Attributes
            AdventOfCodeAttribute ca =
                (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

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

            int[] inputs = FileIOHelper.getInstance().ReadDataToIntArray(file);

            int sum = inputs.Sum();
            _SW.Start();

            var answer = FindBestQE(inputs, 3);

            _SW.Stop();

            Console.WriteLine("Part 1 => Minimum Viable QE with 3 groups: {0}, Execution Time: {1}", answer, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            var answer2 = FindBestQE(inputs, 4);

            _SW.Stop();

            Console.WriteLine("Part 2 => Minimum Viable QE with 4 groups: {0}, Execution Time: {1}", answer2, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }
        
        long FindBestQE(int[] inputs, int groups)
        {
            var totalWeight = inputs.Sum();
            var weightPerSet = totalWeight / groups;
            bestSoFar = 1 + inputs.Length / groups;
            var bestSet = Distribute(new List<long>(), inputs.ToList(), (int)weightPerSet)
                .Select(g => new { g.Count, QE = g.Aggregate((a, b) => a * b) })
                .OrderBy(g => g.Count)
                .ThenBy(g => g.QE)
                .First();
            bestSet.Dump();
            return bestSet.QE;
        }

        int bestSoFar = Int32.MaxValue;
        IEnumerable<List<long>> Distribute(List<long> used, List<int> pool, int amount)
        {
            if (used.Count >= bestSoFar) yield break;
    
            var remaining = amount - used.Sum();
            for (int n = 0; n < pool.Count; n++)
            {
                var s = pool[n];
                if (pool[n] > remaining) continue;
                var x = used.ToList();
                x.Add(s);
                if (s == remaining)
                {
                    if (x.Count < bestSoFar)
                        bestSoFar = x.Count;
                    yield return x;
                }
                else
                {
                    var y = pool.Skip(n+1).ToList();
                    foreach (var d in Distribute(x, y, amount))
                    {
                        yield return d;
                    }
                }
            }
        }
        
    }
}
