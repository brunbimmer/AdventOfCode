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
    [AdventOfCode(Year = 2021, Day = 18)]
    public class Year2021Day18 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2021Day18()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            List<string> snailFish = FileIOHelper.getInstance().ReadDataAsLines(file).ToList();

            int magnitude = 0;
            string finalString = "";

            SW.Start();                       

            (magnitude, finalString) = Part1(snailFish);

            
            SW.Stop();

            Console.WriteLine("Part 1 - Magnitude of addition problem: ");
            Console.WriteLine("    Solution Answer: {0}", finalString);
            Console.WriteLine("    Final Magnitude: {0}", magnitude);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

           int magnitudeMax = Part2(snailFish);
            
            SW.Stop();

            Console.WriteLine("Part 2: Max Magnitude from Permutations: {0}", magnitudeMax);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


        }    
        
        private   (int, string) Part1(List<string> snailFish)
        {
            string final = snailFish.Aggregate((currentLine, nextLine) => Add(currentLine, nextLine));
            return (Magnitude(final), final);                          
        }

        private   int Part2(List<string> snailFish)
        {
            List<int> magnitudes = new List<int>();

            foreach(var permutation in GetPermutations(snailFish, 2))
            {
                magnitudes.Add(Magnitude(Add(permutation.ElementAt(0), permutation.ElementAt(1))));
            }

            return magnitudes.Max();

        }

        private   IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1).SelectMany(t => list.Where(o => !t.Contains(o)), (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        private   int Magnitude(string x)
        {
            int i = 0;
            if (int.TryParse(x, out i)) return i;

            //separate left and right pair
            string left, right = "";

            (left, right) = SplitToPairs(x);


            return 3 * Magnitude(left) + 2 * Magnitude(right);
        }

        private   string Add(string line1, string line2)
        {
            //Step 1: Add the new lines together

            string line = "[" + line1 + "," + line2 + "]";

            bool didExplode = false;
            bool didSplit = false;

            while (true)
            {
                (didExplode, _, line, _ ) = Explode(line);

                if (didExplode) continue; //loop back

                (didSplit, line) = Split(line);

                if (didSplit) continue; //loop back

                break;
            }

            return line;
        }

        private   (bool, string, string, string) Explode(string x, int depth = 4)
        {
            bool result = false;
            //don't care what the value is, return;
            if (int.TryParse(x, out _)) return (false, null, x, null);
            

            if (depth == 0)
            {
                string _left, _right = "";
                (_left, _right) = SplitToPairs(x);
                return (true, _left, "0", _right);
            }

            string a = "";
            string b = "";
            
            (a, b) = SplitToPairs(x);
            
            string left = "";
            string right = "";

            (result, left, a, right) = Explode(a, depth - 1);

            if (result)
                return (true, left, "[" + a + "," + AddLeft(b, right) + "]", null);

            (result, left, b, right) = Explode(b, depth - 1);

            if (result)
                return (true, null, "[" + AddRight(a, left) + "," + b + "]", right);

            return (false, null, x, null);
        }

        private   (bool, string) Split(string x)
        {
            bool result = false;

            int xValue = 0;
            if (int.TryParse(x, out xValue))
            {
                if (xValue >= 10)
                    return (true, "[" + (xValue / 2) + "," + (int)Math.Ceiling((decimal)xValue / 2) + "]");
                else
                    return (false, x);
            }

            string left, right = "";
            (left, right) = SplitToPairs(x);

            (result, left) = Split(left);

            if (result) return (true, "[" + left + "," + right + "]");

            (result, right) = Split(right);

            return (result, "[" + left + "," + right + "]");

        }

        private   string AddLeft(string x, string n)
        {
            if (n == null) return x;

            int xValue = 0;
            int nValue = int.Parse(n);

            if (int.TryParse(x, out xValue)) return (xValue + nValue).ToString();

            string left, right = "";
            (left, right) = SplitToPairs(x);


            return "[" + AddLeft(left, n) + "," + right + "]";
        }

        private   string AddRight(string x, string n)
        {
            if (n == null) return x;

            int xValue = 0;
            int nValue = int.Parse(n);
            if (int.TryParse(x, out xValue)) return (xValue + nValue).ToString();

            string left, right = "";
            (left, right) = SplitToPairs(x);

            return "[" + left + "," + AddRight(right, n) + "]";
        }

        private   (string, string) SplitToPairs(string line)
        {
            string leftPair = "";
            string rightPair = "";
            int openBr = 0;

            for (int i = 1; i < (line.Length - 1); i++)
            {
                if (line[i] == '[')
                    openBr += 1;
                else if (line[i] == ']')
                    openBr -= 1;

               if (openBr == 0)
                {
                    int posComma = line[i];
                    if (line[i] != ',')
                        posComma = line.IndexOf(",", i);

                    leftPair = line.Substring(1, posComma - 1);
                    rightPair = line.Substring(posComma + 1, (line.Length - 1) - (posComma  + 1));
                    break;
                }
            }

            return (leftPair, rightPair);
        }
    }
}
