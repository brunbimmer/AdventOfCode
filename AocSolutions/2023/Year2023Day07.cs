using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;
using Microsoft.Extensions.Primitives;
using MoreLinq;
using MoreLinq.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 7)]
    public class Year2023Day07: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day07()
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

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

           
           
            _SW.Start();

            long totalWinings = PlayPoker(input, true);

            Console.WriteLine($"  Part 1: Total Winnings: {totalWinings}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");


            _SW.Stop();

            _SW.Reset();
            _SW.Start();

            totalWinings = PlayPoker(input, false);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Total Winnings: {totalWinings}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");
        }

        private long PlayPoker(string[] games, bool part1)
        {

            List<Hand> hands = new List<Hand>();

            foreach (string game in games)
            {
                string hand = game.Split(' ')[0];
                int bid = int.Parse(game.Split(' ')[1]);

                hands.Add(new Hand(hand, bid, part1));
            }

            //sorts lowest to ehighest
            hands.Sort();

            return hands.Select((hand, index) => hand.bid * (index + 1)).Sum();
            
        }
    }


    public class Hand: IComparable
    {
        public int bid;

        public int strength;
        public int[] freqs = new int[13];
        public int[] cards = new int[5];
        public char[] ranks = new char[] { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };

        public Hand(string line, int bid, bool part1)
        {
            if (!part1)
            {
                ranks = new char[] { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' };
            }

            this.bid = bid;
            int numJokers = 0;
    
            int cardIndex = 0;

            foreach (char card in line)
            {
                for (int j = 0; j < ranks.Length; j++)
                {
                    if (card.Equals(ranks[j]))
                    {
                        if (!card.Equals('J') || part1)
                            freqs[j]++;

                        if (card.Equals('J') && !part1)
                            numJokers++;

                        cards[cardIndex] = j;
                    }
                }

                cardIndex++;
            }

            Array.Sort(freqs);
            
            freqs[freqs.Length - 1] += numJokers;
            
            strength = 2 * freqs[freqs.Length - 1];

            if (freqs[freqs.Length - 2] == 2)
                strength += 1;

        }

        public int CompareTo(object obj)
        {
            Hand other = (Hand)obj;

            if (strength != other.strength)
                return strength - other.strength;
            else
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    if (cards[i] != other.cards[i])
                        return other.cards[i] - cards[i];
                }
                return 0;
            }
        }
    }

}
