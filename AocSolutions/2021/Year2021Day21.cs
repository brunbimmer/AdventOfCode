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
    [AdventOfCode(Year = 2021, Day = 21)]
    public class Year2021Day21 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private   Dictionary<int, (int, long)> scores;     //player and position/score match
        private   int dice;
        private   long rolls;

        public Year2021Day21()
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
 

            //Input is simple, no need to read in the filename

            dice = 0;
            rolls = 0;
            scores = new();

            _SW.Start();                       

            scores.Add(1, (6, 0));      //Player 1 (Position 7, Score: 0), but make position 0-based and adjust.
            scores.Add(2, (0, 0));      //Player 2 (Position 1, Score: 0), but make position 0-based and adjust.

            long lowestScore = Part1();
            
            _SW.Stop();

            Console.WriteLine("Part 1: Lowest Score: {0}", lowestScore);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            //actual Data
            int player1Loc = 7;
            int player2Loc = 1;

            SortedDictionary<int, long> result = PlayDirac(0, 0, player1Loc, player2Loc, 1, 0, 3);

            long maxWins = result.Max(x => x.Value);
            
            _SW.Stop();

            Console.WriteLine("Part 2: Number of Universes in which the winning player won: {0}", maxWins);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       

         public   long Part1()
        {
            int moves = 0;

            while (true)
            {
                moves = RollDice();

                (int, long) player1 = scores[1];
                (int, long) player2 = scores[2];

                player1.Item1 = (player1.Item1 + moves) % 10;
                player1.Item2 += (player1.Item1 + 1);           //adjust for 0-based playing board

                scores[1] = player1;
                if (scores[1].Item2 >= 1000)
                    break;

                moves = RollDice();

                player2.Item1 = (player2.Item1 + moves) % 10;
                player2.Item2 += (player2.Item1 + 1);           //adjust for 0-based playing board

                scores[2] = player2;
                if (scores[2].Item2 >= 1000)
                    break;
            }

            return scores.Min(x => x.Value.Item2 * rolls);
        }

        private   int RollDice()
        {
            int moves = 0;

            foreach (int i in Enumerable.Range(1, 3))
            {
                dice += 1;
                moves += dice <= 100 ? dice : 1;

                if (dice > 100) dice = 1;      //roll over to start sequence over. 
                rolls += 1;
            }

            return moves;
        }

          SortedDictionary<string, SortedDictionary<int, long>> GameStateTracker = new SortedDictionary<string, SortedDictionary<int, long>>();
       
        /// <summary>
        /// Play w/ Dirac. Each play is its own universe which carries information from the previous universe. 
        /// </summary>
        private   SortedDictionary<int, long> PlayDirac(int p1_score, int p2_score, int p1_loc, int p2_loc, int turn, int roll_sum, int rolls)
        {
            //keep track of the current stat of the game
            string gameState = string.Format("{0},{1},{2},{3},{4},{5},{6}", p1_score, p2_score, p1_loc, p2_loc, turn, roll_sum, rolls);
            if (GameStateTracker.ContainsKey(gameState)) return GameStateTracker[gameState];

            SortedDictionary<int, long> result = new SortedDictionary<int, long>();

            result.Add(1, 0L);
            result.Add(2, 0L);

            if (rolls > 0)
            {
                //for each of the 3 rolls spawn 3 games (one for side of the dice)
                for (int d = 1; d <= 3; d++)
                {
                    Add(result, PlayDirac(p1_score, p2_score, p1_loc, p2_loc, turn, roll_sum + d, rolls - 1));
                }
            }
            else
            {
                //when rolls exhausted, we move,tally scores and play again if we haven't reach score of 21.
                if (turn == 1)
                {
                    p1_loc += roll_sum;
                    while (p1_loc > 10) p1_loc -= 10;
                    p1_score += p1_loc;
                    if (p1_score >= 21) 
                        Add(result, 1, 1L);   //add a win to player 1 if they reach a score of 21.
                    else 
                        Add(result, PlayDirac(p1_score, p2_score, p1_loc, p2_loc, 2, 0, 3));       //Player 2 rolls next
                }
                else
                {
                    p2_loc += roll_sum;
                    while (p2_loc > 10) p2_loc -= 10;
                    p2_score += p2_loc;
                    if (p2_score >= 21) 
                        Add(result, 2, 1L);  //add a win to player 2 if they reach a score of 21.
                    else 
                        Add(result, PlayDirac(p1_score, p2_score, p1_loc, p2_loc, 1, 0, 3));        //Player 1 rolls next
                }
            }

            GameStateTracker[gameState] = result;

            return result;
        }

        public   void Add(SortedDictionary<int, long> result, int player, long wins)
        {
            result[player] = result[player] + wins;
        }
        public   void Add(SortedDictionary<int, long> result, SortedDictionary<int, long> addResult)
        {
            foreach (int k in addResult.Keys)
            {
                result[k] = result[k] + addResult[k];
            }
        }
    }
}
