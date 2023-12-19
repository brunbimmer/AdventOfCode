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
    public static class Extensions
    {
	    public static T[] SubArray<T>(this T[] array, int offset, int length)
	    {
		    T[] result = new T[length];
		    Array.Copy(array, offset, result, 0, length);
		    return result;
	    }

        public static T[] Column<T>(this T[][] jaggedArray,int wanted_column)
        {
            T[] columnArray = new T[jaggedArray.Length];
            T[] rowArray;
            for(int i=0;i<jaggedArray.Length;i++)
            {
                rowArray=jaggedArray[i];
                if(wanted_column<rowArray.Length)
                    columnArray[i]=rowArray[wanted_column];
            }
            return columnArray;
        }
    }

    [AdventOfCode(Year = 2022, Day = 8)]
    public class Year2022Day08 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day08()
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

            int[][] treeGrid = FileIOHelper.getInstance().GetDataAsDoubleIntArray(file);

            _SW.Restart();                       

            (int visibleTrees, int scenicScore) = CalculateVisibleTreesAndScenicScore(treeGrid);
            
            _SW.Stop();

            Console.WriteLine("Part 1: Visible Trees {0}, Top Scenic Score {1}, Execution Time: {2}", visibleTrees, scenicScore, StopwatchUtil.getInstance().GetTimestamp(_SW));
           
        }       

        private (int, int) CalculateVisibleTreesAndScenicScore(int[][] treeGrid)
        {
            int visibleTrees = 0;
            int scenicScore = 0;

            int maxX = treeGrid.Length;
            int maxY = treeGrid[0].Length;

            for(int x = 0; x < maxX; x++)
            {                
                if (x == 0 || x == (maxX - 1))
                {
                    visibleTrees += maxY;
                    continue;
                }
                    
                for (int y = 0; y < maxY; y++)
                {
                    if (y == 0 || y == (maxY - 1))
                    {
                        visibleTrees += 1;
                        continue;
                    }

                    int height = treeGrid[x][y];

                    int [] horizontalLeft = DoReversal(treeGrid[x].SubArray(0, y));
                    int [] horizontalRight = treeGrid[x].SubArray(y + 1, (maxY - 1) - y);

                    int [] verticalTop = DoReversal(treeGrid.SubArray(0, x).Column(y));                                                  
                    int [] verticalBottom = treeGrid.SubArray(x + 1, (maxX - 1) - x).Column(y);

                    
                    if (   IsTreeVisibleToEdge(horizontalLeft, height)
                        || IsTreeVisibleToEdge(horizontalRight, height)
                        || IsTreeVisibleToEdge(verticalTop, height)
                        || IsTreeVisibleToEdge(verticalBottom, height))
                    {
                        visibleTrees += 1;
                    }


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

        public int[] DoReversal(int[] theArray)
        {
            int tempHolder = 0;

            if (theArray.Length > 0)
            {
                for (int counter = 0; counter < (theArray.Length / 2); counter++)
                {
                    tempHolder = theArray[counter];                        
                    theArray[counter] = theArray[theArray.Length - counter - 1];   
                    theArray[theArray.Length - counter - 1] = tempHolder;      
                }
            }
            else
            {
                Console.WriteLine("Nothing to reverse");
            }
            return theArray;
        }


        private bool IsTreeVisibleToEdge(int[] range, int height)
        {
            bool visible = true;

            foreach (int i in range)
            {
                if (i >= height)
                {
                    visible = false;
                    break;
                }
            }
            return visible;
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
