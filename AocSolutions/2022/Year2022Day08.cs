using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 8)]
    public class Year2022Day08 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2022Day08()
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

            int[][] treeGrid = FileIOHelper.getInstance().GetDataAsDoubleIntArray(file);

            SW.Restart();                       

            (int visibleTrees, int scenicScore) = CalculateVisibleTreesAndScenicScore(treeGrid);

            
            SW.Stop();

            Console.WriteLine("Part 1: Visible Trees {0}, Top Scenic Score {1}, Execution Time: {2}", visibleTrees, scenicScore, StopwatchUtil.getInstance().GetTimestamp(SW));
           
        }       

        private (int, int) CalculateVisibleTreesAndScenicScore(int[][] treeGrid)
        {
            int visibleTrees = 0;
            int scenicScore = 0;

            int maxX = treeGrid.Length;
            int maxY = treeGrid[0].Length;

            for(int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (x == 0 || x == (maxX - 1) || y == 0 || y == (maxY - 1))
                    {
                        visibleTrees += 1;
                        continue;
                    }

                    int height = treeGrid[x][y];

                    int [] horizontalLeft = treeGrid[x].Take(y).Reverse().ToArray();
                    int [] horizontalRight = treeGrid[x].Skip(y + 1).ToArray();

                    int [] verticalTop = treeGrid.Take(x).Reverse().Select(i => i[y]).ToArray();                    
                    int [] verticalBottom = treeGrid.Skip(x + 1).Select( i => i[y]).ToArray();


                    
                    if (    horizontalLeft.Max() < height 
                        ||  horizontalRight.Max() < height
                        ||  verticalTop.Max() < height
                        ||  verticalBottom.Max() < height
                        )
                        visibleTrees += 1; 


                    int _tempScore =   CalculateScenicScore(horizontalLeft, height) 
                                     * CalculateScenicScore(horizontalRight, height)
                                     * CalculateScenicScore(verticalTop, height)
                                     * CalculateScenicScore(verticalBottom, height);                  

                    if (_tempScore > scenicScore)
                        scenicScore = _tempScore;

                }
            }                       

            return (visibleTrees, scenicScore);
        }

        private int CalculateScenicScore(int[] range, int height)
        {
            int score = 0;
            foreach (int i in range)
            {
                score += 1;

                if (i >= height)
                    break;
            }

            return score;
        }
    }
}
