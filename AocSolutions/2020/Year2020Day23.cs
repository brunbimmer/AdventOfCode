using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 23)]
    public class Year2020Day23 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day23()
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

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            int[] cups = ParseCups(FileIOHelper.getInstance().ReadDataAsString(file));

            _SW.Restart();
            int[] next = PlayGame(cups, cups.Length, 100);
            var labels = new StringBuilder();
            for (int cup = next[1]; cup != 1; cup = next[cup])
                labels.Append(cup);
            _SW.Stop();

            output.WritePart(1, labels.ToString(), trackTime ? _SW.Elapsed : null, "Labels clockwise after cup 1");

            _SW.Restart();
            next = PlayGame(cups, 1_000_000, 10_000_000);
            long product = (long)next[1] * next[next[1]];
            _SW.Stop();

            output.WritePart(2, product, trackTime ? _SW.Elapsed : null, "Product of the two cups clockwise after cup 1");
        }

        private int[] ParseCups(string input)
        {
            string labels = input.Trim();
            if (labels.Length < 4 || labels.Any(label => label < '1' || label > '9'))
                throw new FormatException("At least four cup labels are required, each a digit from 1 to 9.");

            int[] cups = labels.Select(label => label - '0').ToArray();
            if (cups.Distinct().Count() != cups.Length || cups.Max() != cups.Length)
                throw new FormatException("Cup labels must be a permutation of 1 through the number of cups.");

            return cups;
        }

        private int[] PlayGame(int[] cups, int totalCups, int moves)
        {
            // Each entry stores the label of the next cup clockwise.
            int[] next = new int[totalCups + 1];
            for (int index = 1; index < cups.Length; index++)
                next[cups[index - 1]] = cups[index];

            int last = cups[^1];
            for (int label = cups.Length + 1; label <= totalCups; label++)
            {
                next[last] = label;
                last = label;
            }
            next[last] = cups[0];

            int current = cups[0];
            for (int move = 0; move < moves; move++)
            {
                int first = next[current];
                int second = next[first];
                int third = next[second];
                next[current] = next[third];

                int destination = current == 1 ? totalCups : current - 1;
                while (destination == first || destination == second || destination == third)
                    destination = destination == 1 ? totalCups : destination - 1;

                next[third] = next[destination];
                next[destination] = first;
                current = next[current];
            }

            return next;
        }
    }
}
