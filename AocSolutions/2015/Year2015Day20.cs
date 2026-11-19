using System;
using System.Diagnostics;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 20)]
    public class Year2015Day20 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2015Day20()
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

            //No file...input in the puzzle.

            int presents = 29000000;

            _SW.Restart();
            int min = FindFirstHouse(presents, 10, int.MaxValue);
            _SW.Stop();

            output.WritePart(1, min, trackTime ? _SW.Elapsed : null, "Smallest House #");

            _SW.Restart();

            int min2 = FindFirstHouse(presents, 11, 50);
            _SW.Stop();

            output.WritePart(2, min2, trackTime ? _SW.Elapsed : null, "Smallest House #");
        }

        private int FindFirstHouse(int target, int presentsPerElf, int maxVisits)
        {
            // This house is guaranteed enough presents from its own elf.
            int bestHouse = (target - 1) / presentsPerElf + 1;
            int[] presentsAtHouse = new int[bestHouse];

            for (int elf = 1; elf < bestHouse; elf++)
            {
                int delivered = elf * presentsPerElf;
                for (int house = elf, visits = 0; house < bestHouse && visits < maxVisits; house += elf, visits++)
                {
                    // Compare before adding so accumulated presents cannot overflow.
                    if (presentsAtHouse[house] >= target - delivered)
                        bestHouse = house;
                    else
                        presentsAtHouse[house] += delivered;
                }
            }

            return bestHouse;
        }
    }
}
