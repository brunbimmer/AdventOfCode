using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using CommonAlgorithms;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 11)]
    public class Year2015Day11Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        
        public Year2015Day11Problem()
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

            //simple input, file access not required

            if (trackTime) SW.Start();

            string currentPassword = "cqjxjnds";

            string nextPassword = GenerateNewPassword(currentPassword);
                          

            if (trackTime) SW.Stop();            

            Console.WriteLine("  Part 1: The next password after [{0}]: {1}", currentPassword, nextPassword);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            currentPassword = nextPassword;

            nextPassword = GenerateNewPassword(currentPassword);

            if (trackTime) SW.Stop();

            Console.WriteLine("  Part 2: The next password after [{0}]: {1}", currentPassword, nextPassword);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));
            

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        private string GenerateNewPassword(string input)
        {
            bool validPassword = false;
            string newPassword = "";

            while (validPassword == false)
            {
                input = IncrementAlphaNumericValue(input);

                if (IsValidPassword(input))
                {
                    newPassword = input;
                    validPassword = true;
                }
            }

            return newPassword;
        }


        public string IncrementAlphaNumericValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            if (Regex.IsMatch(value, "^[a-z]+$") == false)
            {
                throw new Exception("Invalid Character: Must be a-z");
            }

            var characterArray = value.ToCharArray();

            for (var characterIndex = characterArray.Length - 1; characterIndex >= 0; characterIndex--)
            {
                var characterValue = Convert.ToInt32(characterArray[characterIndex]);

                if (characterValue != 122)
                {
                    characterArray[characterIndex]++;

                    for (int resetIndex = characterIndex + 1; resetIndex < characterArray.Length; resetIndex++)
                    {
                        characterValue = Convert.ToInt32(characterArray[resetIndex]);
                        if (characterValue == 122)
                        {
                            characterArray[resetIndex] = 'a';
                        }                        
                    }

                    return new string(characterArray);

                }
            }

            //If we got through the Character Loop and were not able to increment anything, we return a NULL string. 
            return null;
        }

        public bool IsValidPassword(string newPassword)
        {
            bool _isValid = false;
            bool _continue = false;
                
            //Step 1: First check if password contains any specific characters (i, o or l)

            if (Regex.IsMatch(newPassword, "^[^iol]+$"))
            {
                _continue = true;
            }

            //Step 2: Check for a sequence of characters in any position such as "abc", "bcd", etc
            if (_continue)
            {
                //Reset continue method while we check for next rule
                _continue = false;
                
                var characterArray = newPassword.ToCharArray();

                for (int i = 0; i < characterArray.Length - 2; i++)
                {
                    if (   ( characterArray[i + 1] == characterArray[i] + 1 )   //check if the next character is one more than the previous
                        && ( characterArray[i + 2] == characterArray[i] + 2 ))  //check if the third character is one character more than 2nd character
                    {
                        _continue = true;
                        break;
                    }
                }
            }

            //Step 3: Check for repeat pattern ("aa" or "bb"), but do not overlap
            if (_continue)
            {
                var characterArray = newPassword.ToCharArray();
                int count = 0;
                for (int i = 0; i < characterArray.Length - 1; i++)
                {
                    if (characterArray[i] == characterArray[i + 1])  //check if the third character is one character more than 2nd character
                    {
                        count++;
                        i++;        //skip to prevent overlapping
                    }
                }

                if (count >= 2)
                    _isValid = true;
            }
                          
            return _isValid;
        }

    }
}
