using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DailyPuzzle
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Count() == 1)
                {
                    var filename = args[0];
                    Program.Run(filename).Wait();
                }
                else
                {
                    Console.WriteLine("Invalid Arguments. Please specify input filename only.");
                    Console.WriteLine("   DailyPuzzle input.txt");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        private static async Task Run(string filename)
        {
            //Step 1: Read in the file
            Console.WriteLine("Loading Input file");

            List<string> input = (await File.ReadAllLinesAsync(filename)).ToList();

            List<string> corruptedList;
            int score;
            
            (corruptedList, score) = GetSolution1(input);

            List<string> incompleteList = input.Except(corruptedList).ToList();

            long midPointScore = GetSolution2(incompleteList);

            Console.WriteLine("Part 1: Total Syntax Score: {0}", score);
            Console.WriteLine("Part 2: Total MidPoint Score: {0}", midPointScore);
        }

        private static char GetOpeningChar(char closingCharacter)
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

        private static int GetCorruptScore(char closingCharacter)
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

        private static int GetAutoCompleteScore(char openingCharacter)
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

        private static (List<string>, int) GetSolution1(List<string> lines)
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

        private static long GetSolution2(List<string> lines)
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

        private static int ProcessCharacters(Stack<char> characterStack, string line, List<string> corruptedList = null, bool checkForCorruption = false)
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
    }
}
