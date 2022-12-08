using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 5)]
    public class Year2022Day05 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2022Day05()
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

            Dictionary<int, Stack<char>> stacks = SetupStacks();

            string stackTops = RunOperation(stacks, lines);
            
            SW.Stop();

            Console.WriteLine("Part 1: Crates at top of each stack {0}, Execution Time: {1}", stackTops, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            Dictionary<int, Stack<char>> newStacks = SetupStacks();

            stackTops = RunOperationPart2(newStacks, lines);
            
            SW.Stop();

            Console.WriteLine("Part 2: Crates at top of each stack {0}, Execution Time: {1}", stackTops, StopwatchUtil.getInstance().GetTimestamp(SW));


        }       

        private Dictionary<int, Stack<char>> SetupStacks()
        {
            Stack<char> col1 = new Stack<char>(new Char[] {'J','H','P','M','S','F','N','V'});
            Stack<char> col2 = new Stack<char>(new Char[] {'S','R','L','M','J','D','Q'});
            Stack<char> col3 = new Stack<char>(new Char[] {'N','Q','D','H','C','S','W','B'});
            Stack<char> col4 = new Stack<char>(new Char[] {'R','S','C','L',});
            Stack<char> col5 = new Stack<char>(new Char[] {'M','V','T','P','F','B'});
            Stack<char> col6 = new Stack<char>(new Char[] {'T','R','Q','N','C'});
            Stack<char> col7 = new Stack<char>(new Char[] {'G','V','R'});
            Stack<char> col8 = new Stack<char>(new Char[] {'C','Z','S','P','D','L','R'});
            Stack<char> col9 = new Stack<char>(new Char[] {'D','S','J','V','G','P','B','F'});

            Dictionary<int, Stack<char>> stack = new Dictionary<int, Stack<char>>();
            stack[0] = col1;
            stack[1] = col2;
            stack[2] = col3;    
            stack[3] = col4;    
            stack[4] = col5;                    
            stack[5] = col6;    
            stack[6] = col7;
            stack[7] = col8;
            stack[8] = col9;

            return stack;
        }

        private string RunOperation(Dictionary<int, Stack<char>> stacks, string[] instructions)
        {
            //instructions start at line 12 (i.e., line 11)

            for (int i = 10; i < instructions.Length; i++)
            {
                string[] steps = instructions[i].Split(' ');

                int moves = Convert.ToInt32(steps[1]);
                Stack<char> fromStack = stacks[Convert.ToInt32(steps[3]) - 1];
                Stack<char> toStack = stacks[Convert.ToInt32(steps[5]) - 1];

                for (int j = 0; j < moves; j++)
                {
                    char _temp = fromStack.Pop();
                    toStack.Push(_temp);
                }                
            }

            StringBuilder sbOutput = new StringBuilder();
            
            for (int c = 0; c < 9; c++)
                sbOutput.Append(stacks[c].Peek());

            return sbOutput.ToString();
        }

         private string RunOperationPart2(Dictionary<int, Stack<char>> stacks, string[] instructions)
        {
            //instructions start at line 12 (i.e., line 11)

            for (int i = 10; i < instructions.Length; i++)
            {
                string[] steps = instructions[i].Split(' ');

                int moves = Convert.ToInt32(steps[1]);
                Stack<char> fromStack = stacks[Convert.ToInt32(steps[3]) - 1];
                Stack<char> toStack = stacks[Convert.ToInt32(steps[5]) - 1];

                char[] stacksToMove = new char[moves];               
                    
                for (int j = moves - 1; j >= 0; j--)
                    stacksToMove[j] = fromStack.Pop();

                for (int j = 0; j < moves; j++)
                    toStack.Push(stacksToMove[j]);

            }

            StringBuilder sbOutput = new StringBuilder();
            
            for (int c = 0; c < 9; c++)
                sbOutput.Append(stacks[c].Peek());

            return sbOutput.ToString();
        }
    }
}
