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

        public Stopwatch _SW { get; set; }

        public Year2015Day25()
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

            //Build BasePath and retrieve input. 

            _SW.Start();

            BigInteger code = CalculateCode(2947, 3029);

            
            _SW.Stop();

            output.WritePart(1, code, trackTime ? _SW.Elapsed : null, "Code at Row 2947 and Column 3029 is");

            _SW.Restart();

           
            
            _SW.Stop();

            //Console.WriteLine("Part 2: {0}, Execution Time: {1}", result2, StopwatchUtil.getInstance().GetTimestamp(_SW));
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
