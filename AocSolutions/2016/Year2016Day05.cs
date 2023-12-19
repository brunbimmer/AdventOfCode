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
    [AdventOfCode(Year = 2016, Day = 5)]
    public class Year2016Day05 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day05()
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


            string doorId = "abbhdwsy";

            _SW.Start();

            string password = FindPassword(doorId);



            _SW.Stop();

            Console.WriteLine("Part 1: Computed Password: {0}, Execution Time: {1}", password, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            string complexPassword = FindComplexPassword(doorId);

            _SW.Stop();

            Console.WriteLine("Part 2: Complex Password: {0}, Execution Time: {1}", complexPassword, StopwatchUtil.getInstance().GetTimestamp(_SW));

        }

        string FindPassword(string doorId)
        {

            string password = "";
            int index = 1;

            do
            {
                string md5 = ComputeMD5Hash(doorId + index);

                if (md5.StartsWith("00000"))
                    password += md5[5]; //sixth position

                index += 1;
            } while (password.Length < 8);

            return password;
        }

        string FindComplexPassword(string doorId)
        {

            var password = Enumerable.Repeat('*', 8).ToArray();
            int index = 1;
            int value = 0;

            do
            {
                string md5 = ComputeMD5Hash(doorId + index);

                if (md5.StartsWith("00000") && int.TryParse(md5[5].ToString(), out value) && value < password.Length && password[value] == '*')
                {
                    password[value] = md5[6]; //sixth position
                }
                    

                index += 1;
            } while (password.Contains('*'));

            return string.Join("", password);
        }


        public string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
