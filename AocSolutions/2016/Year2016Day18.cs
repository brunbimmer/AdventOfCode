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
    [AdventOfCode(Year = 2016, Day = 18)]
    public class Year2016Day18 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2016Day18()
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
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string input = FileIOHelper.getInstance().ReadDataAsString(file).Trim();
            
            _SW.Start();                       

            int safeTiles = GenerateSafeTiles(input, 40);

            
            _SW.Stop();

            output.WritePart(1, safeTiles, trackTime ? _SW.Elapsed : null, "Number of safe tiles 40 rows");

            _SW.Restart();

            int safeTiles40k = GenerateSafeTiles(input, 400000);           
            
            _SW.Stop();

            output.WritePart(2, safeTiles40k, trackTime ? _SW.Elapsed : null, "Number of safe tiles in 400,000 rows");

        }       

        private int GenerateSafeTiles(string input, int rows)
        {
            int width = input.Length;
            char[,] grid = new char[rows, width];

            // Initialize first row
            for (int i = 0; i < width; i++)
            {
                grid[0, i] = input[i];
            }

            // Generate subsequent rows
            for (int r = 1; r < rows; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    char left = (c > 0) ? grid[r - 1, c - 1] : '.';
                    char center = grid[r - 1, c];
                    char right = (c < width - 1) ? grid[r - 1, c + 1] : '.';

                    // Determine if current tile is a trap
                    if ((left == '^' && center == '^' && right == '.') ||
                        (center == '^' && right == '^' && left == '.') ||
                        (left == '^' && center == '.' && right == '.') ||
                        (right == '^' && center == '.' && left == '.'))
                    {
                        grid[r, c] = '^'; // Trap
                    }
                    else
                    {
                        grid[r, c] = '.'; // Safe
                    }
                }
            }

            // Count safe tiles
            int safeCount = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    if (grid[r, c] == '.')
                    {
                        safeCount++;
                    }
                }
            }

            return safeCount;
        }
    }
}
