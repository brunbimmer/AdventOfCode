using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 14)]
    public class Year2016Day14 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        // Pre-compile regex for triples
        private static readonly Regex TripleRegex = new Regex(@"(.)\1\1", RegexOptions.Compiled);

        public Year2016Day14()
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
 

            string key = "zpqevtbw";

            _SW.Start();                       

            int part1 = FindIndexOf64thKey(key, false);

            
            _SW.Stop();

            Console.WriteLine("Part 1: {0}, Execution Time: {1}", part1, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            int part2 = FindIndexOf64thKey(key, true);
            
            _SW.Stop();

            Console.WriteLine("Part 2: {0}, Execution Time: {1}", part2, StopwatchUtil.getInstance().GetTimestamp(_SW));

        }   

        int FindIndexOf64thKey(string key, bool useStretching)
        {
            // Pre-compute hashes up to a safe limit to avoid cache misses
            // For Part 2, we need index + 1000 lookahead
            int maxIndex = useStretching ? 28000 : 25000;
            
            // Use array instead of Dictionary for faster parallel access
            string[] hashCache = new string[maxIndex];
            
            // Parallel compute all hashes
            Parallel.For(0, maxIndex, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                string hash = Utilities.ComputeMD5Hash($"{key}{i}");

                if (useStretching)
                {
                    for (int j = 0; j < 2016; j++)
                    {
                        hash = Utilities.ComputeMD5Hash(hash);
                    }
                }

                hashCache[i] = hash;
            });

            int foundKeys = 0;
            int currentIndex = 0;

            while (foundKeys < 64 && currentIndex < maxIndex - 1000)
            {
                string hash = hashCache[currentIndex];
                char? tripleChar = GetTripleChar(hash);

                if (tripleChar.HasValue)
                {
                    char quintupleChar = tripleChar.Value;

                    // Look for quintuple in next 1000 hashes
                    bool found = false;
                    for (int lookahead = currentIndex + 1; lookahead <= currentIndex + 1000; lookahead++)
                    {
                        if (ContainsConsecutive(hashCache[lookahead], quintupleChar, 5))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        foundKeys++;
                    }
                }

                currentIndex++;
            }

            return currentIndex - 1;
        }

        /// <summary>
        /// Finds the first character that appears 3+ times consecutively.
        /// Returns the character if found, null otherwise.
        /// </summary>
        private char? GetTripleChar(string hash)
        {
            for (int i = 0; i < hash.Length - 2; i++)
            {
                if (hash[i] == hash[i + 1] && hash[i + 1] == hash[i + 2])
                {
                    return hash[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if a character appears consecutively n times in the string.
        /// </summary>
        private bool ContainsConsecutive(string hash, char c, int count)
        {
            int consecutive = 0;
            foreach (char ch in hash)
            {
                if (ch == c)
                {
                    consecutive++;
                    if (consecutive >= count)
                        return true;
                }
                else
                {
                    consecutive = 0;
                }
            }
            return false;
        }    
    }
}
