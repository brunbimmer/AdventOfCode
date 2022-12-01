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
    [AdventOfCode(Year = 2021, Day = 4)]
    public class Year2021Day4 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        private class BingoCard
        {
            public int?[,] Card { get; set; }

            public bool[,] CardValidator { get; set; }
        }

        private   List<int?[,]> BingoCardPass2 = new List<int?[,]>();

        private   List<BingoCard> BingoCards = new List<BingoCard>();

        private   List<int> draw = new List<int>();

        private   int lastNumberCalled;
        private   int sumOfUnmarkedNumbers;

        public Year2021Day4()
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

            ReadFile(file);

            SW.Start();
            PlayBingoGame();
            SW.Stop();



            Console.WriteLine("Part 1: Sum of Unmarked Cards in Winner Board ({0}) * Last Number Called ({1}): {2}", sumOfUnmarkedNumbers, lastNumberCalled, sumOfUnmarkedNumbers * lastNumberCalled);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            ResetBingoValidation();

            PlayBingoGame(true);

            SW.Stop();

            Console.WriteLine("Part 2: Sum of Unmarked Cards in Last Winning Board ({0}) * Last Number Called ({1}): {2}", sumOfUnmarkedNumbers, lastNumberCalled, sumOfUnmarkedNumbers * lastNumberCalled);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("\n===========================================\n");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }

        private void ReadFile(string filename)
        {
            try
            {
                string line;
                using (TextReader reader = File.OpenText(filename))
                {
                    //read the first line to create the draw number Array
                    line = reader.ReadLine();
                    draw = Array.ConvertAll(line.Split(','), s => int.Parse(s)).ToList();

                    while ((line = reader.ReadLine()) != null)
                    {

                        if (line.Length == 0)
                        {
                            //skip processing, we are in between bingo cards
                            continue;
                        }

                        int?[,] newBoard = new int?[5, 5];
                        bool[,] newBoardValidator = new bool[5, 5];

                        int rowIndex = 0;

                        while (rowIndex < 5)
                        {
                            //strip whitespace from middle sequence
                            string[] integers = Regex.Replace(line.Trim(), @"\s+", " ").Split(' ');
                            int[] row = Array.ConvertAll(integers, s => int.Parse(s));

                            //process next five lines into one board listing
                            for (int col = 0; col < 5; col++)
                            {
                                newBoard[rowIndex, col] = row[col];
                                newBoardValidator[rowIndex, col] = false;
                            }

                            rowIndex = rowIndex + 1;

                            //read next line
                            line = reader.ReadLine();
                        }

                        BingoCard card = new BingoCard();

                        card.Card = newBoard;
                        card.CardValidator = newBoardValidator;

                        BingoCards.Add(card);
                        BingoCardPass2.Add(newBoard);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private   void ResetBingoValidation()
        {
            foreach (BingoCard card in BingoCards)
            {
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 5; col++)
                    {
                        card.CardValidator[row, col] = false;
                    }
                }
            }
        }

        private   void PlayBingoGame(bool lastToWin = false)
        {
            bool bingo;
            BingoCard card;

            foreach (int number in draw)
            {
                (bingo, card) = PlayAndCheckBingo(number, lastToWin);

                if (bingo)
                {
                    sumOfUnmarkedNumbers = GetSumBingoCard(card);
                    lastNumberCalled = number;
                    break;
                }
            }
        }

        private   (bool, BingoCard) PlayAndCheckBingo(int number, bool lastToWin)
        {
            bool bingo = false;
            BingoCard winningCard = null;

            foreach (BingoCard card in BingoCards)
            {
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 5; col++)
                    {
                        if (card.Card[row, col] == number)
                        {
                            card.CardValidator[row, col] = true;
                        }
                    }
                }
            }

            List<BingoCard> cardsToRemove = new List<BingoCard>();

            foreach (BingoCard card in BingoCards)
            {
                (bingo, winningCard) = CheckForBingo(card);

                if (bingo && lastToWin)
                {
                    cardsToRemove.Add(winningCard);
                }
                else if (bingo)
                    break;
            }

            if (BingoCards.Count > 1 && lastToWin && cardsToRemove.Count > 0)
            {
                foreach(BingoCard card in cardsToRemove)
                {
                    BingoCards.Remove(card);
                    bingo = false;
                    winningCard = null;
                }

                cardsToRemove.Clear();
            }

            return (bingo, winningCard);
        }
        private   (bool, BingoCard) CheckForBingo(BingoCard card)
        {
            //check each row
            for (int row = 0; row < 5; row++)
            {

                if (   card.CardValidator[row,0] 
                    && card.CardValidator[row,1] 
                    && card.CardValidator[row,2] 
                    && card.CardValidator[row,3] 
                    && card.CardValidator[row,4])
                    
                    return (true, card);
            }

            //check the columns
            for (int col = 0; col < 5; col++)
            {
                if (   card.CardValidator[0,col] 
                       && card.CardValidator[1,col] 
                       && card.CardValidator[2,col] 
                       && card.CardValidator[3,col] 
                       && card.CardValidator[4,col])
                    
                    return (true, card);
            }

            return (false, null);
        }
        private   int GetSumBingoCard(BingoCard card)
        {
            int sum = 0;
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (card.CardValidator[row,col] == false)
                    {
                        sum += card.Card[row,col] ?? 0;
                    }
                }
            }

            return sum;
        }

        #region Trial code with Lambda Expressions (incomplete)

        //private   void PlayBingoGameLinqOrLambdaStyle()
        //{
        //    bool firstWin = false;

        //    foreach (int number in draw)
        //    {
        //        BingoCardPass2.
        //    }

        //}


        //private   (bool, int [,]) CheckForBingoLambdaStyle(int [,] card)
        //{

        //    return (false, null);
        //}

        //private   int GetSumBingoCardLambdaStyle(int[,] card)
        //{
        //    int sum = 0;

        //    return sum;
        //}


        #endregion
    }
}
