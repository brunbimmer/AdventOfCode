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
    [AdventOfCode(Year = 2016, Day = 15)]
    public class Year2016Day15 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private record Disc(int Positions, int InitialPosition);

        public Year2016Day15()
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
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            List<Disc> discs = ParseInput(input);

            _SW.Start();

            long part1 = SolveCRT(discs);

            _SW.Stop();

            Console.WriteLine("  Part 1: {0}, Execution Time: {1}", part1, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            // Part 2: Add an additional disc with 11 positions starting at position 0
            List<Disc> discsPart2 = new List<Disc>(discs);
            discsPart2.Add(new Disc(11, 0));

            long part2 = SolveCRT(discsPart2);

            _SW.Stop();

            Console.WriteLine("  Part 2: {0}, Execution Time: {1}", part2, StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        /// <summary>
        /// Parses input to extract disc information
        /// Format: Disc #N has M positions; at time=0, it is at position P.
        /// </summary>
        private List<Disc> ParseInput(string[] lines)
        {
            List<Disc> discs = new List<Disc>();
            var regex = new Regex(@"Disc #(\d+) has (\d+) positions; at time=0, it is at position (\d+)\.");

            foreach (string line in lines)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    int positions = int.Parse(match.Groups[2].Value);
                    int initialPosition = int.Parse(match.Groups[3].Value);
                    discs.Add(new Disc(positions, initialPosition));
                }
            }

            return discs;
        }

        /// <summary>
        /// Solves the disc alignment problem using Chinese Remainder Theorem
        /// Each disc gives us a congruence: t ≡ a_i (mod m_i)
        /// where a_i = -(initial_position_i + disc_index_i) mod m_i
        /// </summary>
        private long SolveCRT(List<Disc> discs)
        {
            // Convert each disc constraint to modular form
            // For disc i (0-indexed), capsule arrives at time t+i+1
            // We need: (initial_pos + t + i + 1) % positions == 0
            // Therefore: t ≡ -(initial_pos + i + 1) (mod positions)

            List<(long remainder, long modulus)> congruences = new List<(long, long)>();

            for (int i = 0; i < discs.Count; i++)
            {
                long remainder = (-(discs[i].InitialPosition + i + 1)) % discs[i].Positions;
                if (remainder < 0) remainder += discs[i].Positions;
                congruences.Add((remainder, discs[i].Positions));
            }

            return CRT(congruences);
        }

        /// <summary>
        /// Chinese Remainder Theorem implementation
        /// Solves: x ≡ a_i (mod m_i) for all i
        /// </summary>
        private long CRT(List<(long a, long m)> congruences)
        {
            // Iteratively merge congruences
            long x = congruences[0].a;
            long m = congruences[0].m;

            for (int i = 1; i < congruences.Count; i++)
            {
                long a2 = congruences[i].a;
                long m2 = congruences[i].m;

                // Find t such that:
                // x + m*k ≡ a2 (mod m2)
                
                // Brute force search for k
                long k = -1;
                for (long trial = 0; trial < m2; trial++)
                {
                    if ((x + m * trial) % m2 == a2 % m2)
                    {
                        k = trial;
                        break;
                    }
                }

                if (k == -1)
                {
                    return -1;
                }

                x = x + m * k;
                m = LCM(m, m2);
                x = ((x % m) + m) % m;
            }

            return x;
        }

        /// <summary>
        /// Extended Euclidean Algorithm
        /// Returns (gcd, x) where a*x + b*y = gcd
        /// </summary>
        private (long gcd, long x) ExtendedGCD(long a, long b)
        {
            if (b == 0)
                return (a, 1);

            var (gcd, x1) = ExtendedGCD(b, a % b);
            long x = x1 - (a / b) * x1;
            return (gcd, x);
        }

        /// <summary>
        /// Computes LCM(a, b) = a * b / GCD(a, b)
        /// </summary>
        private long LCM(long a, long b)
        {
            return a / GCD(a, b) * b;
        }

        /// <summary>
        /// Computes GCD using Euclidean algorithm
        /// </summary>
        private long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }
}
