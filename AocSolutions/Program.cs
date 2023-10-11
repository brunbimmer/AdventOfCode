using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using AdventOfCode;
using Microsoft.Extensions.Configuration;

namespace PuzzleMain
{
    class Program
    {
        private static Dictionary<int, KeyValuePair<AdventOfCodeAttribute, IAdventOfCode>> SolutionCache;

        static void Main(string[] args)
        {
            try
            {
                FileIOHelper.getInstance().Initialize();
                
                LoadSolutionCache();

                if (args.Length != 2)
                {
                    Console.WriteLine("Program usage: AdventOfCode <yyyy> <day>");
                    Console.WriteLine("   Enter the year and correlating day of the month of the Advent of Code Puzzle");
                    Environment.Exit(0);
                }
                else
                {
                    int year = Convert.ToInt32(args[0]);
                    int day = Convert.ToInt32(args[1]);

                    //
                    int solutionIndex = (year - 2015) * 25 + day;

                     if (SolutionCache.ContainsKey(solutionIndex))
                     {
                        Console.WriteLine("");
                        Console.WriteLine("ADVENT OF CODE (December Christmas Puzzles)");
                        Console.WriteLine("For description of Daily puzzles, please visit ==> https://adventofcode.com/");
                        Console.WriteLine("----------------------------------------------------------------------------");
                        Console.WriteLine("");
                        SolutionCache[solutionIndex].Value.GetSolution("daily.txt", true);
                     }
                     else
                        Console.WriteLine("The select year and day does not correlate to a Advent of Code Puzzle");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void LoadSolutionCache()
        {
            if (SolutionCache == null) SolutionCache = new();

            SolutionCache.Clear();
            Type target = typeof(IAdventOfCode);
            Assembly a = Assembly.GetExecutingAssembly();
            IEnumerable<Type> solutions = a.GetTypes().Where(t => target.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            solutions.ToList().ForEach(t =>
            {
                if (Attribute.GetCustomAttribute(t, typeof(AdventOfCodeAttribute)) is AdventOfCodeAttribute ca)
                {
                    IAdventOfCode c = Activator.CreateInstance(t) as IAdventOfCode;
                    if (!SolutionCache.ContainsKey(ca.Index))
                    {
                        SolutionCache.Add(ca.Index, new KeyValuePair<AdventOfCodeAttribute, IAdventOfCode>(ca, c));
                    }
                }
            });
        }
    }
}
