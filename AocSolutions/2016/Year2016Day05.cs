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

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            //Build BasePath and retrieve input. 


            string doorId = "abbhdwsy";

            _SW.Start();

            string password = FindPassword(doorId);



            _SW.Stop();

            output.WritePart(1, password, trackTime ? _SW.Elapsed : null, "Computed Password");

            _SW.Restart();

            string complexPassword = FindComplexPassword(doorId);

            _SW.Stop();

            output.WritePart(2, complexPassword, trackTime ? _SW.Elapsed : null, "Complex Password");

        }

        string FindPassword(string doorId)
        {

            string password = "";
            int index = 1;

            do
            {
                string md5 = Utilities.ComputeMD5Hash(doorId + index);

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
                string md5 = Utilities.ComputeMD5Hash(doorId + index);

                if (md5.StartsWith("00000") && int.TryParse(md5[5].ToString(), out value) && value < password.Length && password[value] == '*')
                {
                    password[value] = md5[6]; //sixth position
                }
                    

                index += 1;
            } while (password.Contains('*'));

            return string.Join("", password);
        }
    }
}
