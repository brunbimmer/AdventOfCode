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

        static int tableWidth = 140;
        static void Main(string[] args)
        {
            try
            {
                FileIOHelper.getInstance().Initialize();
                
                LoadSolutionCache();


                Program.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Run()
        {
            string input = "";

            while (true)
            {
                DisplayInitScreen();
                input = Console.ReadLine();

                if (input.ToLower() == "x") break;

                int index = int.Parse(input);
                
                if (SolutionCache.ContainsKey(index))
                    SolutionCache[index].Value.GetSolution("daily.txt", true);
            }
            return;
        }

        private static void DisplayInitScreen()
        {

            //var years = SolutionCache.Values.GroupBy(x => x.Key.Year).Select(x => x.Key.Year).ToArray();

            var years = SolutionCache.Values.Select(x => x.Key.Year).Distinct().ToList();

            List<string> yearsToPrint = new List<string>();

            foreach (var year in years)
            {
                yearsToPrint.Add(year.ToString());
            }

            Console.WriteLine("ADVENT OF CODE (December Christmas Puzzles)");
            Console.WriteLine("For description of Daily puzzles, please visit ==> https://adventofcode.com/2015/events");
            PrintLine();
            PrintRow(yearsToPrint.ToArray());
            PrintLine();

            foreach(int i in Enumerable.Range(1, 25))
            {
                PrintSolutionRow(i, yearsToPrint.ToArray().Length);
            }

            PrintLine();
            Console.WriteLine("");
            Console.Write("What solution do you want to run (type \"X\" to quit program)? ");

        }

        #region Console Helper Routines

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static void PrintSolutionRow(int day, int columnLength)
        {
            int width = (tableWidth - columnLength) /columnLength;
            string row = "|";
            int year = 2015;
            
            foreach (int i in Enumerable.Range(0, columnLength))
            {
                int index = (year - 2015) * 25 + day;

                if (SolutionCache.ContainsKey(index))
                    row +=  (" " + index + ".  Day " + day).PadRight(width) + "|";
                else
                    row +=  (" ").PadRight(width) + "|";

                year += 1;
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        #endregion

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
