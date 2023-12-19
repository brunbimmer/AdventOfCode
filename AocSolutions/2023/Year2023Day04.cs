using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;
using Microsoft.Extensions.Primitives;
using MoreLinq;
using MoreLinq.Extensions;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 4)]
    public class Year2023Day04: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

       

        public Stopwatch _SW { get; set; }



        public Year2023Day04()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        private int[] scratchCardList;

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] cards = FileIOHelper.getInstance().ReadDataAsLines(file);

            scratchCardList = Enumerable.Repeat(1, cards.Length).ToArray();
           
            _SW.Start();


            int total = ProcessCardList(cards);


            _SW.Stop();


            Console.WriteLine($"  Part 1: Scratch Card Value:          {total}");
            Console.WriteLine($"  Part 2: Number of scratch cards won: {scratchCardList.Sum()}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");
        }

        int ProcessCardList(string[] cards)
        {
            int cardIndex = 0;
            int sum = 0;
            foreach (string card in cards)
            {
                int[] intersection = GetIntersection(card);


                sum += FindCardTotal(intersection);

                int numCardsToProcess = scratchCardList[cardIndex];

                for (int cardCopies = 1; cardCopies <= numCardsToProcess; cardCopies++)
                {

                    for (int i = 1; i <= intersection.Length; i++)
                    {
                        if (i < scratchCardList.Length) 
                            scratchCardList[cardIndex + i] += 1;
                    }
                }
                cardIndex += 1;
            }

            return sum;
        }


        int FindCardTotal(int[] intersection)
        {
            int subTotal = 0;

            switch (intersection.Length)
            {
                case 1:
                    subTotal = 1;
                    break;
                case >= 2:
                {
                    subTotal = 1;
                    for (int i = 2; i <= intersection.Length; i++)
                    {
                        subTotal = subTotal * 2;
                    }

                    break;
                }
            }

            return subTotal;
        }

        int[] GetIntersection(string card)
        {
            string[] numbers = Regex.Split(card.Split(":", StringSplitOptions.TrimEntries)[1], @"\s+\|\s+");
            int[] group1 = Array.ConvertAll(numbers[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries), int.Parse);
            int[] group2 = Array.ConvertAll(numbers[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries), int.Parse);

            return group1.Intersect(group2).ToArray();
        }

    }
}
