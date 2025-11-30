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
    [AdventOfCode(Year = 2016, Day = 21)]
    public class Year2016Day21 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day21()
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

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       

            string scrambledPassword = ScramblePassword("abcdefgh", instructions);

            
            _SW.Stop();

            Console.WriteLine("Part 1 - Scrambled Password: {0}, Execution Time: {1}", scrambledPassword, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            string unscrambledPassword = UnscramblePassword("fbgdceah", instructions);
            
            _SW.Stop();

            Console.WriteLine("Part 2 - Unscrambled Password: {0}, Execution Time: {1}", unscrambledPassword, StopwatchUtil.getInstance().GetTimestamp(_SW));

        }       

        string ScramblePassword(string password, string[] instructions)
        {
            char[] pass = password.ToCharArray();
            
            foreach (string instruction in instructions)
            {
                pass = ProcessInstruction(pass, instruction, reverse: false);
            }
            
            return new string(pass);
        }

        string UnscramblePassword(string password, string[] instructions)
        {
            char[] pass = password.ToCharArray();
            
            foreach (string instruction in instructions.Reverse())
            {
                pass = ProcessInstruction(pass, instruction, reverse: true);
            }
            
            return new string(pass);
        }

        char[] ProcessInstruction(char[] pass, string instruction, bool reverse)
        {
            var parts = instruction.Split();
            
            if (instruction.StartsWith("swap position"))
            {
                int x = int.Parse(parts[2]);
                int y = int.Parse(parts[5]);
                (pass[x], pass[y]) = (pass[y], pass[x]);
            }
            else if (instruction.StartsWith("swap letter"))
            {
                char x = parts[2][0];
                char y = parts[5][0];
                pass = SwapLetters(pass, x, y);
            }
            else if (instruction.StartsWith("rotate left"))
            {
                int steps = int.Parse(parts[2]);
                pass = reverse ? RotateRight(pass, steps) : RotateLeft(pass, steps);
            }
            else if (instruction.StartsWith("rotate right"))
            {
                int steps = int.Parse(parts[2]);
                pass = reverse ? RotateLeft(pass, steps) : RotateRight(pass, steps);
            }
            else if (instruction.StartsWith("rotate based"))
            {
                char letter = parts[6][0];
                pass = reverse ? ReverseRotateBased(pass, letter) : RotateBased(pass, letter);
            }
            else if (instruction.StartsWith("reverse"))
            {
                int x = int.Parse(parts[2]);
                int y = int.Parse(parts[4]);
                System.Array.Reverse(pass, x, y - x + 1);
            }
            else if (instruction.StartsWith("move"))
            {
                int x = int.Parse(parts[2]);
                int y = int.Parse(parts[5]);
                pass = reverse ? Move(pass, y, x) : Move(pass, x, y);
            }
            
            return pass;
        }

        char[] SwapLetters(char[] pass, char x, char y)
        {
            return pass.Select(c => c == x ? y : c == y ? x : c).ToArray();
        }

        char[] RotateBased(char[] pass, char letter)
        {
            int pos = System.Array.IndexOf(pass, letter);
            int steps = 1 + pos + (pos >= 4 ? 1 : 0);
            return RotateRight(pass, steps % pass.Length);
        }

        char[] ReverseRotateBased(char[] pass, char letter)
        {
            // To reverse "rotate based on position of letter":
            // Find where the letter is now, and determine where it came from
            int currentPos = System.Array.IndexOf(pass, letter);
            
            // The letter was at position `originalPos` before the rotation
            // After rotation, it's at position (originalPos + 1 + originalPos + (originalPos >= 4 ? 1 : 0)) % length
            // We need to find which originalPos leads to currentPos
            
            for (int originalPos = 0; originalPos < pass.Length; originalPos++)
            {
                int rotatedPos = (originalPos + 1 + originalPos + (originalPos >= 4 ? 1 : 0)) % pass.Length;
                if (rotatedPos == currentPos)
                {
                    // Found it! Now rotate left to undo the right rotation
                    int steps = 1 + originalPos + (originalPos >= 4 ? 1 : 0);
                    return RotateLeft(pass, steps % pass.Length);
                }
            }
            
            return pass;
        }

        char[] Move(char[] pass, int from, int to)
        {
            var list = new List<char>(pass);
            char c = list[from];
            list.RemoveAt(from);
            list.Insert(to, c);
            return list.ToArray();
        }

        char[] RotateLeft(char[] arr, int steps)
        {
            steps = steps % arr.Length;
            char[] result = new char[arr.Length];
            System.Array.Copy(arr, steps, result, 0, arr.Length - steps);
            System.Array.Copy(arr, 0, result, arr.Length - steps, steps);
            return result;
        }

        char[] RotateRight(char[] arr, int steps)
        {
            steps = steps % arr.Length;
            return RotateLeft(arr, arr.Length - steps);
        }
    }
}
