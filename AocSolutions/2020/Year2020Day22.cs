using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 22)]
    public class Year2020Day22 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day22()
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

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            (int[] player1, int[] player2) = ParseDecks(FileIOHelper.getInstance().ReadDataAsLines(file));

            _SW.Restart();
            long score = GetWinningScore(player1, player2, false);
            _SW.Stop();

            output.WritePart(1, score, trackTime ? _SW.Elapsed : null, "Winning score in Combat");

            _SW.Restart();
            score = GetWinningScore(player1, player2, true);
            _SW.Stop();

            output.WritePart(2, score, trackTime ? _SW.Elapsed : null, "Winning score in Recursive Combat");
        }

        private (int[], int[]) ParseDecks(string[] lines)
        {
            string[] notes = lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => line.Trim()).ToArray();
            int secondPlayer = Array.IndexOf(notes, "Player 2:");
            if (notes.Length == 0 || notes[0] != "Player 1:" || secondPlayer < 2 || secondPlayer == notes.Length - 1)
                throw new FormatException("Both players must have a labelled, non-empty deck.");

            int[] player1 = notes.Skip(1).Take(secondPlayer - 1).Select(int.Parse).ToArray();
            int[] player2 = notes.Skip(secondPlayer + 1).Select(int.Parse).ToArray();
            int[] cards = player1.Concat(player2).ToArray();
            if (cards.Any(card => card <= 0) || cards.Distinct().Count() != cards.Length)
                throw new FormatException("Card values must be positive and distinct across both decks.");

            return (player1, player2);
        }

        private long GetWinningScore(int[] startingPlayer1, int[] startingPlayer2, bool recursive)
        {
            var player1 = new Queue<int>(startingPlayer1);
            var player2 = new Queue<int>(startingPlayer2);
            int winner = PlayGame(player1, player2, recursive);
            Queue<int> winningDeck = winner == 1 ? player1 : player2;

            long score = 0;
            int multiplier = winningDeck.Count;
            foreach (int card in winningDeck)
            {
                score += (long)card * multiplier;
                multiplier--;
            }

            return score;
        }

        private int PlayGame(Queue<int> player1, Queue<int> player2, bool recursive)
        {
            var previousRounds = new HashSet<string>(StringComparer.Ordinal);
            while (player1.Count > 0 && player2.Count > 0)
            {
                string configuration = string.Join(",", player1) + "|" + string.Join(",", player2);
                if (!previousRounds.Add(configuration))
                {
                    if (recursive)
                        return 1;
                    throw new InvalidOperationException("Combat entered a repeating configuration and has no winner.");
                }

                int card1 = player1.Dequeue();
                int card2 = player2.Dequeue();
                int winner;
                if (recursive && player1.Count >= card1 && player2.Count >= card2)
                {
                    winner = PlayGame(new Queue<int>(player1.Take(card1)), new Queue<int>(player2.Take(card2)), true);
                }
                else
                {
                    winner = card1 > card2 ? 1 : 2;
                }

                if (winner == 1)
                {
                    player1.Enqueue(card1);
                    player1.Enqueue(card2);
                }
                else
                {
                    player2.Enqueue(card2);
                    player2.Enqueue(card1);
                }
            }

            return player1.Count > 0 ? 1 : 2;
        }
    }
}
