using System;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 18)]
    public class Year2020Day18 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day18()
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
            string[] expressions = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Restart();
            long sum = Part1(expressions);
            _SW.Stop();

            output.WritePart(1, sum, trackTime ? _SW.Elapsed : null, "Sum of expression values");


            _SW.Restart();
            sum = Part2(expressions);
            _SW.Stop();

            output.WritePart(2, sum, trackTime ? _SW.Elapsed : null, "Sum with addition evaluated first");

        }

        private long Part1(string[] expressions)
        {
            return expressions.Where(expression => !string.IsNullOrWhiteSpace(expression))
                .Sum(expression => EvaluateExpression(expression, additionFirst: false));
        }

        private long Part2(string[] expressions)
        {
            return expressions.Where(expression => !string.IsNullOrWhiteSpace(expression))
                .Sum(expression => EvaluateExpression(expression, additionFirst: true));
        }

        private long EvaluateExpression(string expression, bool additionFirst)
        {
            int position = 0;
            long result = ParseExpression(expression, ref position, additionFirst);
            if (position != expression.Length)
                throw new FormatException("Unexpected characters after the expression.");

            return result;
        }

        private long ParseExpression(string expression, ref int position, bool additionFirst)
        {
            long result = ParseOperand(expression, ref position, additionFirst);
            long product = 1;
            while (true)
            {
                SkipWhitespace(expression, ref position);
                if (position == expression.Length || expression[position] == ')')
                    return checked(product * result);

                char operation = expression[position++];
                if (operation != '+' && operation != '*')
                    throw new FormatException($"Unknown operator: {operation}");

                long operand = ParseOperand(expression, ref position, additionFirst);
                if (additionFirst && operation == '*')
                {
                    // Finish this addition group and begin the next factor.
                    product = checked(product * result);
                    result = operand;
                }
                else
                {
                    result = operation == '+' ? checked(result + operand) : checked(result * operand);
                }
            }
        }

        private long ParseOperand(string expression, ref int position, bool additionFirst)
        {
            SkipWhitespace(expression, ref position);
            if (position < expression.Length && expression[position] == '(')
            {
                position++;
                long result = ParseExpression(expression, ref position, additionFirst);
                if (position == expression.Length || expression[position] != ')')
                    throw new FormatException("Missing closing parenthesis.");

                position++;
                return result;
            }

            int start = position;
            while (position < expression.Length && char.IsAsciiDigit(expression[position]))
                position++;
            if (position == start)
                throw new FormatException("Expected a number or parenthesized expression.");

            return long.Parse(expression.AsSpan(start, position - start));
        }

        private void SkipWhitespace(string expression, ref int position)
        {
            while (position < expression.Length && char.IsWhiteSpace(expression[position]))
                position++;
        }
    }
}
