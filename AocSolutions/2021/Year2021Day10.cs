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
    [AdventOfCode(Year = 2021, Day = 10)]
    public class Year2021Day10 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2021Day10()
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

            String[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            List<string> corruptedList;
            int score;

            SW.Start();                       
             (corruptedList, score) = GetSolution1(input.ToList());                       
            SW.Stop();

            Console.WriteLine("Part 1: Total Syntax Score: {0}", score);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            List<string> incompleteList = input.Except(corruptedList).ToList();

            long midPointScore = GetSolution2(incompleteList);
            
            SW.Stop();

            Console.WriteLine("Part 2: Total MidPoint Score: {0}", midPointScore);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("\n===========================================\n");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }        
       
        private   (List<string>, int) GetSolution1(List<string> lines)
        {
           //Find corrupt lines
           List<string> corruptedList = new List<string>();
           int score = 0;

           lines.ForEach(line =>
           {
               char[] input = line.ToCharArray();
               Stack<char> characterStack = new Stack<char>();
               score += ProcessCharacters(characterStack, line, corruptedList, true);
           });
           return (corruptedList, score);
        }

        private   long GetSolution2(List<string> lines)
        {
            var scores = new List<long>();

            lines.ForEach(line =>
            {
                long score = 0;
                var input = line.ToCharArray();
                var characterStack = new Stack<char>();

                ProcessCharacters(characterStack, line);

                while (characterStack.Count > 0)
                {
                    var popChar = characterStack.Pop();
                    score = (5 * score) + GetAutoCompleteScore(popChar);
                }
                scores.Add(score);

            });

            var sortedScores = scores.OrderBy(x => x).ToList();
            var midPointScore = sortedScores[(sortedScores.Count - 1)/2];
            return midPointScore;
        }

        private   int ProcessCharacters(Stack<char> characterStack, string line, List<string> corruptedList = null, bool checkForCorruption = false)
        {
            var input = line.ToArray();
            var score = 0;
            foreach (var c in input)
            {
                if (Regex.IsMatch(c.ToString(), "[\\]})>]"))
                {
                    var popChar = characterStack.Pop();
                    if (checkForCorruption && popChar != GetOpeningChar(c))
                    {
                        if (corruptedList != null) corruptedList.Add(line);
                        score = GetCorruptScore(c);
                        break;
                    }
                }
                else
                    characterStack.Push(c);
            }
            return score;
        }

        private   char GetOpeningChar(char closingCharacter)
        {
            return closingCharacter switch
            {
                '}' => '{',
                ')' => '(',
                '>' => '<',
                ']' => '[',
                _ => ' ',
            };
        }

        private   int GetCorruptScore(char closingCharacter)
        {
            return closingCharacter switch
            {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137,
                _ => 0,
            };
        }

        private   int GetAutoCompleteScore(char openingCharacter)
        {
            return openingCharacter switch
            {
                '(' => 1,
                '[' => 2,
                '{' => 3,
                '<' => 4,
                _ => 0,
            };
        }
    }
}
