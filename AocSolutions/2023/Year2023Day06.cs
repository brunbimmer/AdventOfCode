using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;
using Microsoft.Extensions.Primitives;
using MoreLinq;
using MoreLinq.Extensions;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 6)]
    public class Year2023Day06: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day06()
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

            List<(long, long)> races = new List<(long, long)>();

            races.Add((46, 347));
            races.Add((82, 1522));
            races.Add((84, 1406));
            races.Add((79, 1471));

            _SW.Start();

            long winCombo = CalculateWaysToWin(races);
            
            _SW.Stop();
            Console.WriteLine($"  Part 1: WinCombinations: {winCombo}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Reset();
            _SW.Start();

            winCombo = CalculateWaysToWin2(races);

            _SW.Stop();

            Console.WriteLine($"  Part 1: WinCombinations2: {winCombo}");
            Console.WriteLine($"   Execution2 Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Reset();
            _SW.Start();

            races.Clear();
            races.Add((46828479, 347152214061471));

            winCombo = CalculateWaysToWin(races);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Lowest Location: {winCombo}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

            _SW.Reset();
            _SW.Start();

            winCombo = CalculateWaysToWin2(races);

            _SW.Stop();

            Console.WriteLine($"  Part 2: WinCombinations2: {winCombo}");
            Console.WriteLine($"   Execution2 Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");

        }

        long CalculateWaysToWin(List<(long, long)> races)
        {
            long winCombos = 1;

            foreach (var race in races)
            {
                long wins = 0;
                long time = race.Item1;
                long record = race.Item2;

                for (long i = 1; i < time; i++)
                {
                    long distance = i * (time - i);

                    if (distance > record)
                        wins += 1;
                }

                if (wins > 0)
                {
                    winCombos *= wins;
                }
            }

            return winCombos;
        }

        long CalculateWaysToWin2(List<(long, long)> races)
        {
            return races.Select(race =>
                {
                    long wins = Enumerable.Range(1, (int)race.Item1 - 1)
                        .Count(i => i * (race.Item1 - i) > race.Item2);

                    return wins > 0 ? wins : 1;
                })
                .Aggregate(1L, (acc, wins) => acc * wins);
        }

    }
}
