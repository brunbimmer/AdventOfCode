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
using static AdventOfCode.Year2015Day16;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 10)]
    public class Year2022Day10 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        int sum = 0;         
        int cycle = 0;
        int x = 1;

        int printPosition = 0;
        
        StringBuilder buffer;
        List<char> currentLine;
        int statusCheck = 20;
        int newLineStart = 41;

        public Year2022Day10()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            //Reset starting parameters if solution is run consecutively in same session. 
            sum = 0;         
            cycle = 0;
            x = 1;
            statusCheck = 20;
            newLineStart = 41;

            printPosition = 0;
        
            buffer = new StringBuilder();
            currentLine = new List<char>();

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();                       

            foreach (string instruction in instructions)
            {
                if (instruction.StartsWith("noop"))
                {
                    DoCycleOperation();                    
                }
                else
                {
                    //Add instruction is a 2-cycle operation, so run it twice.
                    DoCycleOperation();
                    DoCycleOperation();
                    
                    int value = Convert.ToInt32(instruction.Split(' ').Last());                    
                    x += value;     //update the X value.                   
                }
            }
            //add the last line to the buffer.
            buffer.AppendLine(new string(currentLine.ToArray()));

            _SW.Stop();

            output.WritePart(1, sum, trackTime ? _SW.Elapsed : null, "Sum of Signal Strengths");



            output.WritePart(2, buffer.ToString());
        }     
        
        private void DoCycleOperation()
        {
            cycle += 1;
                    
            if (cycle == newLineStart)
            {
                buffer.AppendLine(new string(currentLine.ToArray()));
                currentLine.Clear();
                printPosition = 0;
                newLineStart += 40;
            }

            if (printPosition >= x - 1 && printPosition <= (x + 1)) 
                currentLine.Add('#');
            else 
                currentLine.Add(' ');
                                        
            if (cycle == statusCheck)
            {
                sum = sum + cycle * x;
                statusCheck += 40;
            }
                

            printPosition += 1;
        }
    }
}
