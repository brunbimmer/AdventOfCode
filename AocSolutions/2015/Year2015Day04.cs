using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 4)]
    public class Year2015Day4Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day4Problem()
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

            //no need to retrieve input from AOC. The input is a simple string

            //string md5Test1 = "abcdef";
            //string md5Test2 = "pqrstuv";

            string md5Secret = "bgvyzdsv";

            //Build BasePath and retrieve input. 
            if (trackTime) SW.Start();

            int lowestNumber = Part1(md5Secret);

            if (trackTime) SW.Stop();

            Console.WriteLine("  Part 1: Lowest Possible Number for Key [{0}] to produce MD5 has with five leading zero's: {1}", md5Secret, lowestNumber);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            lowestNumber = Part2(md5Secret);

            Console.WriteLine("  Part 1: Lowest Possible Number for Key [{0}] to produce MD5 has with six leading zero's: {1}", md5Secret, lowestNumber);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        public int Part1(string secret)
        {
            int i = 0;
            string input = "";
            while (true)
            {
                i++;
                input = CreateMD5(secret + i);

                if (input.StartsWith("00000"))
                    break;                
            }


            return i;
        }

        public int Part2(string secret)
        {
            int i = 0;
            string input = "";
            while (true)
            {
                i++;
                input = CreateMD5(secret + i);

                if (input.StartsWith("000000"))
                    break;
            }


            return i;
        }


        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
