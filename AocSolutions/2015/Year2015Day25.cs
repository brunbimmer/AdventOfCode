using System;
using System.Diagnostics;
using System.Numerics;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 25)]
    public class Year2015Day25 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day25()
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

            SW.Start();

            BigInteger code = CalculateCode(2947, 3029);

            
            SW.Stop();

            Console.WriteLine("Part 1 - Code at Row 2947 and Column 3029 is: {0}, Execution Time: {1}", code, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

           
            
            SW.Stop();

            //Console.WriteLine("Part 2: {0}, Execution Time: {1}", result2, StopwatchUtil.getInstance().GetTimestamp(SW));
        }

        private BigInteger CalculateCode(int row, int col)
        {
            BigInteger first_code = 20151125;
            BigInteger base_value = 252533;
            BigInteger mod_value = 33554393;

            //first calculate the index position of the row and column entry. Based
            //on simple math

            //Table is a triangle and the element we're looking for is on the hypotenuse
            //of an isosceles right angle triangle;

            // First calculate length of the two equal sides.
            BigInteger side = row + col - 1;

            // Calculate how many numbers are in that triangle
            BigInteger quantityOfNumbers = side * (side + 1) / 2;

            // Now get the index (which ends up being zero-based)
            BigInteger index = quantityOfNumbers - row;

            return first_code * BigInteger.ModPow(base_value, index, mod_value) % mod_value;

        }
    }
}
