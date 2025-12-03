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
    [AdventOfCode(Year = 2022, Day = 20)]
    public class Year2022Day20 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private const long DECRYPTION_KEY = 811589153L;
        private const int PART2_ROUNDS = 10;

        public Year2022Day20()
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

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            var numbers = lines.Select(long.Parse).ToList();
            long part1 = GetGroveCoordinates(numbers, 1);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Grove Coordinates Sum: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            var decryptedNumbers = numbers.Select(n => n * DECRYPTION_KEY).ToList();
            long part2 = GetGroveCoordinates(decryptedNumbers, PART2_ROUNDS);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Decrypted Grove Coordinates Sum: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        long GetGroveCoordinates(List<long> numbers, int rounds)
        {
            int n = numbers.Count;
            // Create a list of (value, original index) to track movements
            var elements = numbers.Select((val, idx) => (val, idx)).ToList();

            // Perform mixing rounds
            for (int round = 0; round < rounds; round++)
            {
                // Mix in original order
                for (int originalIdx = 0; originalIdx < n; originalIdx++)
                {
                    // Find current position of element with this original index
                    int currentPos = elements.FindIndex(e => e.idx == originalIdx);
                    var element = elements[currentPos];
                    elements.RemoveAt(currentPos);

                    // Calculate new position with wrapping
                    // Modulo by (n-1) because we've removed one element
                    long moveAmount = element.val % (n - 1);
                    int newPos = (int)((currentPos + moveAmount) % (n - 1));
                    if (newPos < 0) newPos += (n - 1);

                    elements.Insert(newPos, element);
                }
            }

            // Find position of 0
            int zeroPos = elements.FindIndex(e => e.val == 0);

            // Get the three grove coordinates
            long coord1 = elements[(zeroPos + 1000) % n].val;
            long coord2 = elements[(zeroPos + 2000) % n].val;
            long coord3 = elements[(zeroPos + 3000) % n].val;

            return coord1 + coord2 + coord3;
        }
    }
}
