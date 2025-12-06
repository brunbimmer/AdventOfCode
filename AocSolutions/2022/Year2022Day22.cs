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
    // Facing directions for navigation
    public enum Facing
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3
    }

    [AdventOfCode(Year = 2022, Day = 22)]
    public class Year2022Day22 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        // Directions: 0=right, 1=down, 2=left, 3=up
        private static int[][] directions = new int[][]
        {
            new int[] { 0, 1 },   // right
            new int[] { 1, 0 },   // down
            new int[] { 0, -1 },  // left
            new int[] { -1, 0 }   // up
        };

        public Year2022Day22()
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
            string input = FileIOHelper.getInstance().ReadDataAsString(file);

            var (board, instructions) = ParseInput(input);

            _SW.Start();

            long password1 = TraverseBoard(board, instructions, isCube: false);

            _SW.Stop();

            Console.WriteLine($"  Part 1: Final Password: {password1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long password2 = TraverseBoard(board, instructions, isCube: true);

            _SW.Stop();

            Console.WriteLine($"  Part 2: Final Password (Cube): {password2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private (Dictionary<Coordinate2D, char>, List<string>) ParseInput(string input)
        {
            var parts = input.Split(new string[] { "\n\n" }, StringSplitOptions.None);
            var boardLines = parts[0].Split('\n');
            var instructionLine = parts[1].Trim();

            // Build board as Coordinate2D -> char dictionary
            var board = new Dictionary<Coordinate2D, char>();
            for (int row = 0; row < boardLines.Length; row++)
            {
                for (int col = 0; col < boardLines[row].Length; col++)
                {
                    if (boardLines[row][col] != ' ')
                    {
                        board[new Coordinate2D(col, row)] = boardLines[row][col];
                    }
                }
            }

            // Parse instructions: numbers and letters
            var instructions = new List<string>();
            var match = Regex.Matches(instructionLine, @"(\d+|[LR])");
            foreach (Match m in match)
            {
                instructions.Add(m.Value);
            }

            return (board, instructions);
        }

        private long TraverseBoard(Dictionary<Coordinate2D, char> board, List<string> instructions, bool isCube = false)
        {
            // Find starting position (leftmost open tile in top row)
            var startPos = board.Keys.Where(k => k.Y == 0).OrderBy(k => k.X).First();
            int col = startPos.X;
            int row = startPos.Y;

            Facing facing = Facing.Right; // Start facing right

            // Determine cube size from board
            int cubeSize = isCube ? 50 : 0;

            foreach (string instruction in instructions)
            {
                if (instruction == "R")
                {
                    facing = (Facing)(((int)facing + 1) % 4);
                }
                else if (instruction == "L")
                {
                    facing = (Facing)(((int)facing + 3) % 4); // Counterclockwise
                }
                else
                {
                    int steps = int.Parse(instruction);
                    int dr = directions[(int)facing][0];
                    int dc = directions[(int)facing][1];

                    for (int i = 0; i < steps; i++)
                    {
                        int nextRow = row + dr;
                        int nextCol = col + dc;
                        Facing nextFacing = facing;

                        if (isCube)
                        {
                            (nextRow, nextCol, nextFacing) = GetNextPositionCube(col, row, dr, dc, facing, cubeSize, board);
                        }
                        else
                        {
                            // For flat map, handle wrapping
                            var nextCoord = new Coordinate2D(nextCol, nextRow);
                            
                            // If position doesn't exist or is a wall, check for wrapping
                            if (!board.ContainsKey(nextCoord))
                            {
                                (nextRow, nextCol) = WrapFlatMap(col, row, dr, dc, board);
                            }
                        }

                        // Check if next tile is a wall or doesn't exist
                        var checkCoord = new Coordinate2D(nextCol, nextRow);
                        if (!board.ContainsKey(checkCoord) || board[checkCoord] == '#')
                            break;

                        row = nextRow;
                        col = nextCol;
                        facing = nextFacing;
                    }
                }
            }

            // Password: 1000 * row + 4 * col + facing (1-indexed)
            return 1000 * (row + 1) + 4 * (col + 1) + (int)facing;
        }

        private (int, int) WrapFlatMap(int col, int row, int dr, int dc, Dictionary<Coordinate2D, char> board)
        {
            // Moving vertically (up or down)
            if (dc == 0)
            {
                if (dr > 0) // Moving down, wrap to top
                {
                    var topmost = board.Keys.Where(k => k.X == col).OrderBy(k => k.Y).FirstOrDefault();
                    if (topmost != null)
                        return (topmost.Y, col);
                }
                else // Moving up, wrap to bottom
                {
                    var bottommost = board.Keys.Where(k => k.X == col).OrderByDescending(k => k.Y).FirstOrDefault();
                    if (bottommost != null)
                        return (bottommost.Y, col);
                }
            }
            // Moving horizontally (left or right)
            else
            {
                if (dc > 0) // Moving right, wrap to leftmost
                {
                    var leftmost = board.Keys.Where(k => k.Y == row).OrderBy(k => k.X).FirstOrDefault();
                    if (leftmost != null)
                        return (row, leftmost.X);
                }
                else // Moving left, wrap to rightmost
                {
                    var rightmost = board.Keys.Where(k => k.Y == row).OrderByDescending(k => k.X).FirstOrDefault();
                    if (rightmost != null)
                        return (row, rightmost.X);
                }
            }

            return (row, col);
        }

        private (int, int, Facing) GetNextPositionCube(int col, int row, int dr, int dc, Facing facing, int cubeSize, Dictionary<Coordinate2D, char> board)
        {
            int nextCol = col + dc;
            int nextRow = row + dr;
            Facing nextFacing = facing;

            var nextPos = new Coordinate2D(nextCol, nextRow);

            // If we're still on the board, return as-is
            if (board.ContainsKey(nextPos) && board[nextPos] != '#')
                return (nextRow, nextCol, nextFacing);

            // We're wrapping to another face - handle the transition
            (nextRow, nextCol, nextFacing) = WrapCubeFace(col, row, facing, cubeSize);

            return (nextRow, nextCol, nextFacing);
        }

        private int GetFace(int col, int row, int size)
        {
            // Detect which face based on coordinate ranges
            // Face 1: cols 50-99, rows 0-49
            // Face 2: cols 100-149, rows 0-49
            // Face 3: cols 50-99, rows 50-99
            // Face 4: cols 0-49, rows 100-149
            // Face 5: cols 50-99, rows 100-149
            // Face 6: cols 0-49, rows 150-199

            if (col >= 50 && col < 100 && row >= 0 && row < 50) return 1;
            if (col >= 100 && col < 150 && row >= 0 && row < 50) return 2;
            if (col >= 50 && col < 100 && row >= 50 && row < 100) return 3;
            if (col >= 0 && col < 50 && row >= 100 && row < 150) return 4;
            if (col >= 50 && col < 100 && row >= 100 && row < 150) return 5;
            if (col >= 0 && col < 50 && row >= 150 && row < 200) return 6;
            
            return -1;
        }

        private (int, int, Facing) WrapCubeFace(int col, int row, Facing facing, int size)
        {
            int currentFace = GetFace(col, row, size);
            
            // Calculate local coordinates (0-49) within the face
            int localCol = 0, localRow = 0;
            switch (currentFace)
            {
                case 1: localCol = col - 50;   localRow = row - 0; break;
                case 2: localCol = col - 100;  localRow = row - 0; break;
                case 3: localCol = col - 50;   localRow = row - 50; break;
                case 4: localCol = col - 0;    localRow = row - 100; break;
                case 5: localCol = col - 50;   localRow = row - 100; break;
                case 6: localCol = col - 0;    localRow = row - 150; break;
            }

            // Switch on face, then on facing direction
            switch (currentFace)
            {
                case 1:
                    if (facing == Facing.Right) // Face 1 right => Face 2 left
                        return (row, 100, Facing.Right);
                    else if (facing == Facing.Down) // Face 1 bottom => Face 3 top
                        return (50, col, Facing.Down);
                    else if (facing == Facing.Left) // Face 1 left => Face 4 left (opposite, flipped)
                        return (149 - localRow, 0, Facing.Right);
                    else if (facing == Facing.Up) // Face 1 top => Face 6 left (top row becomes left col)
                        return (150 + localCol, 0, Facing.Right);
                    break;

                case 2:
                    if (facing == Facing.Right) // Face 2 right => Face 5 right (opposite, flipped)
                        return (149 - localRow, 99, Facing.Left);
                    else if (facing == Facing.Down) // Face 2 bottom => Face 3 right (bottom row becomes right col)
                        return (50 + localCol, 99, Facing.Left);
                    else if (facing == Facing.Left) // Face 2 left => Face 1 right
                        return (row, 99, Facing.Left);
                    else if (facing == Facing.Up) // Face 2 top => Face 6 bottom (top row becomes bottom row)
                        return (199, localCol, Facing.Up);
                    break;

                case 3:
                    if (facing == Facing.Right) // Face 3 right => Face 2 bottom (right col becomes bottom row)
                        return (49, 100 + localRow, Facing.Up);
                    else if (facing == Facing.Down) // Face 3 bottom => Face 5 top
                        return (100, col, Facing.Down);
                    else if (facing == Facing.Left) // Face 3 left => Face 4 top (left col becomes top row)
                        return (100, localRow, Facing.Down);
                    else if (facing == Facing.Up) // Face 3 top => Face 1 bottom
                        return (49, col, Facing.Up);
                    break;

                case 4:
                    if (facing == Facing.Right) // Face 4 right => Face 5 left
                        return (row, 50, Facing.Right);
                    else if (facing == Facing.Down) // Face 4 bottom => Face 6 top
                        return (150, col, Facing.Down);
                    else if (facing == Facing.Left) // Face 4 left => Face 1 left (opposite, flipped)
                        return (49 - localRow, 50, Facing.Right);
                    else if (facing == Facing.Up) // Face 4 top => Face 3 left (top row becomes left col)
                        return (50 + localCol, 50, Facing.Right);
                    break;

                case 5:
                    if (facing == Facing.Right) // Face 5 right => Face 2 right (opposite, flipped)
                        return (49 - localRow, 149, Facing.Left);
                    else if (facing == Facing.Down) // Face 5 bottom => Face 6 right (bottom row becomes right col)
                        return (150 + localCol, 49, Facing.Left);
                    else if (facing == Facing.Left) // Face 5 left => Face 4 right
                        return (row, 49, Facing.Left);
                    else if (facing == Facing.Up) // Face 5 top => Face 3 bottom
                        return (99, col, Facing.Up);
                    break;

                case 6:
                    if (facing == Facing.Right) // Face 6 right => Face 5 bottom (right col becomes bottom row)
                        return (149, 50 + localRow, Facing.Up);
                    else if (facing == Facing.Down) // Face 6 bottom => Face 2 top
                        return (0, 100 + localCol, Facing.Down);
                    else if (facing == Facing.Left) // Face 6 left => Face 1 top (left col becomes top row)
                        return (0, 50 + localRow, Facing.Down);
                    else if (facing == Facing.Up) // Face 6 top => Face 4 bottom (top row becomes bottom row)
                        return (149, col, Facing.Up);
                    break;
            }

            // Fallback
            return (row, col, facing);
        }
    }
}
