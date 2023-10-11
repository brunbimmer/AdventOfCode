using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 17)]
    public class Year2022Day17 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }


        public class JetFlow
        {
            string pattern;
            int position;

            public JetFlow(string _input)
            {
                this.pattern = _input;
            }

            public int getNext()
            {
                //reset position to zero if we reached the end of pattern buffer
                if (++position >= pattern.Length)
                    position = 0;

                int direction = 0;
                switch (pattern[position])
                {
                    case '<':
                        direction = -1;
                        break;
                    case '>':
                        direction = 1;
                        break;
                    default:
                        direction = 0;
                        break;
                }
                position += 1;
                return direction;
            }
        }

        public Year2022Day17()
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

            String jetflow = FileIOHelper.getInstance().ReadDataAsString(file);

            //set up a few basics for tetris and keeping it simple since we are dealing with characters
            //Array<long,int> tetrisGrid = new System.Array<long, int>();

            //int height = 0;             //there is no height to the tower just yet.




            

            SW.Start();                       



            
            SW.Stop();

            //Console.WriteLine("Part 1: {0}, Execution Time: {1}", result1, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

           
            
            SW.Stop();

            //Console.WriteLine("Part 2: {0}, Execution Time: {1}", result2, StopwatchUtil.getInstance().GetTimestamp(SW));
        }     
           
    }
}
