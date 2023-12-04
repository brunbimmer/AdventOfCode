using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 7)]
    public class Year2016Day07 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2016Day07()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();

            int _supportTlS = FindTlsEntries(lines);


            
            SW.Stop();

            Console.WriteLine("Part 1: Number of IPs supporting TLS {0}, Execution Time: {1}", _supportTlS, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            int _supportSSL= FindSupportingSSL(lines);
            
            SW.Stop();

            Console.WriteLine("Part 2: Number of IPs supporting SSL {0}, Execution Time: {1}", _supportSSL, StopwatchUtil.getInstance().GetTimestamp(SW));

        }

        int FindTlsEntries(string[] input)
        {
            int sum = 0;

            foreach (string s in input)
            {
                if (SupportsTLS(s))
                    sum += 1;
            }
            return sum;
        }

        bool SupportsTLS(string input)
        {
            // Check in hypernet
            foreach (Match m in Regex.Matches(input, @"\[(\w*)\]"))
            {
                if (checkABBA(m.Value))
                {
                    return false;
                }
            }

            string[] ipv7 = Regex.Split(input, @"\[[^\]]*\]");
            foreach (var v in ipv7)
            {
                if (checkABBA(v))
                    return true;
            }

            return false;
        }

        int FindSupportingSSL(string[] input)
        {
            int sum = 0;

            foreach (string s in input)
            {

                if (SupportsSSL(s))
                    sum += 1;

            }

            return sum;
        }

        bool SupportsSSL(string input)
        {
            string[] ipv7 = Regex.Split(input, @"\[[^\]]*\]");
            foreach (string ip in ipv7)
            {
                List<string> aba = checkABA(ip);
                foreach (var val in aba)
                {
                    string bab = val[1].ToString() + val[0].ToString() + val[1].ToString();
                    foreach (Match m in Regex.Matches(input, @"\[(\w*)\]"))
                    {
                        if (m.Value.Contains(bab))
                            return true;
                    }

                }
            }

            return false;
        }

        List<string> checkABA(string input)
        {
            List<string> lst = new List<string>();
            for (int i = 0; i < input.Length - 2; i++)
            {
                if (input[i] == input[i + 2] && input[i] != input[i + 1])
                    lst.Add(input[i].ToString() + input[i + 1].ToString() + input[i + 2].ToString());
            }

            return lst;
        }

        static bool checkABBA(string input)
        {
            for (int i = 0; i < input.Length - 3; i++)
            {
                if (input[i] == input[i + 3] && input[i + 1] == input[i + 2] && input[i] != input[i + 1])
                    return true;
            }

            return false;
        }
    }
}
