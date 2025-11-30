using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{

    public static class BunnyAssemblyParser
    {
        public static Dictionary<string, int> ParseInstructions(Dictionary<string, int> registers, List<string> instructions)
        {
            int instructionPointer = 0;

            while (instructionPointer < instructions.Count)
            {
                string[] parts = instructions[instructionPointer].Split(' ');

                switch (parts[0])
                {
                    case "cpy":

                        // Skip if destination is not a register
                        if (int.TryParse(parts[2], out _))
                        {
                            instructionPointer++;
                            break;
                        }
                        int value = int.TryParse(parts[1], out int val) ? val : registers[parts[1]];
                        registers[parts[2]] = value;
                        instructionPointer++;
                        break;
                    case "inc":
                        registers[parts[1]]++;
                        instructionPointer++;
                        break;

                    case "dec":
                        registers[parts[1]]--;
                        instructionPointer++;
                        break;

                    case "jnz":
                        int checkValue = int.TryParse(parts[1], out int chkVal) ? chkVal : registers[parts[1]];
                        if (checkValue != 0)
                        {
                            int jumpValue = int.TryParse(parts[2], out int jmpVal) ? jmpVal : registers[parts[2]];
                            instructionPointer += jumpValue;
                        }
                        else
                        {
                            instructionPointer++;
                        }
                        break;
                    case "tgl":
                        int tglOffset = int.TryParse(parts[1], out int tglVal) ? tglVal : registers[parts[1]];
                        int targetIndex = instructionPointer + tglOffset;
                        if (targetIndex >= 0 && targetIndex < instructions.Count)
                        {
                            string targetInstruction = instructions[targetIndex];
                            string[] targetParts = targetInstruction.Split(' ');

                            if (targetParts.Length == 2)
                            {
                                // One-argument instruction
                                if (targetParts[0] == "inc")
                                    targetParts[0] = "dec";
                                else
                                    targetParts[0] = "inc";
                            }
                            else if (targetParts.Length == 3)
                            {
                                // Two-argument instruction
                                if (targetParts[0] == "jnz")
                                    targetParts[0] = "cpy";
                                else
                                    targetParts[0] = "jnz";
                            }

                            instructions[targetIndex] = string.Join(" ", targetParts);
                        }
                        instructionPointer++;
                        break;
                
                    case "out":                        
                        int output = int.TryParse(parts[1], out int outVal) ? outVal : registers[parts[1]];
                        Console.Write(output + " ");
                        instructionPointer++;
                        break;            
                    default:
                        instructionPointer++;
                        break;
                }
            }
           
           return registers;
        }
    }

    public static class Utilities
    {
        public static string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public enum Direction
        {
            None,
            N,
            S,
            E,
            W
        }

        public const CompassDirection N = CompassDirection.N;
        public const CompassDirection S = CompassDirection.S;
        public const CompassDirection E = CompassDirection.E;
        public const CompassDirection W = CompassDirection.W;
        public const CompassDirection NE = CompassDirection.NE;
        public const CompassDirection NW = CompassDirection.NW;
        public const CompassDirection SE = CompassDirection.SE;
        public const CompassDirection SW = CompassDirection.SW;

        public enum CompassDirection
        {
            N = 0,
            NE = 45,
            E = 90,
            SE = 135,
            S = 180,
            SW = 225,
            W = 270,
            NW = 315
        }

        public static CompassDirection Flip(this CompassDirection dir)
        {
            return (dir) switch
            {
                N => S,
                S => N,
                E => W,
                W => E,
                NE => SW,
                SW => NE,
                SE => NW,
                NW => SE,
                _ => throw new ArgumentException()
            };
        }

        public static Coordinate2D MoveDirection(this Coordinate2D start, CompassDirection Direction, bool flipY = false, int distance = 1)
        {
            if (flipY)
            {
                return (Direction) switch
                {
                    N => start + (0, -distance),
                    NE => start + (distance, -distance),
                    E => start + (distance, 0),
                    SE => start + (distance, distance),
                    S => start + (0, distance),
                    SW => start + (-distance, distance),
                    W => start + (-distance, 0),
                    NW => start + (-distance, -distance),
                    _ => throw new ArgumentException("Direction is not valid", nameof(Direction))
                };
            }
            else
            {
                return (Direction) switch
                {
                    N => start + (0, distance),
                    NE => start + (distance, distance),
                    E => start + (distance, 0),
                    SE => start + (distance, -distance),
                    S => start + (0, -distance),
                    SW => start + (-distance, -distance),
                    W => start + (-distance, 0),
                    NW => start + (-distance, distance),
                    _ => throw new ArgumentException("Direction is not valid", nameof(Direction))
                };
            }
        }

        public static Coordinate2DLong MoveDirection(this Coordinate2DLong start, CompassDirection Direction, bool flipY = false, long distance = 1)
        {
            if (flipY)
            {
                return (Direction) switch
                {
                    N => start + (0, -distance),
                    NE => start + (distance, -distance),
                    E => start + (distance, 0),
                    SE => start + (distance, distance),
                    S => start + (0, distance),
                    SW => start + (-distance, distance),
                    W => start + (-distance, 0),
                    NW => start + (-distance, -distance),
                    _ => throw new ArgumentException("Direction is not valid", nameof(Direction))
                };
            }
            else
            {
                return (Direction) switch
                {
                    N => start + (0, distance),
                    NE => start + (distance, distance),
                    E => start + (distance, 0),
                    SE => start + (distance, -distance),
                    S => start + (0, -distance),
                    SW => start + (-distance, -distance),
                    W => start + (-distance, 0),
                    NW => start + (-distance, distance),
                    _ => throw new ArgumentException("Direction is not valid", nameof(Direction))
                };
            }
        }

        public static List<string> SplitByNewline(this string input, bool blankLines = false, bool shouldTrim = true)
        {
            return input
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Where(s => blankLines || !string.IsNullOrWhiteSpace(s))
                .Select(s => shouldTrim ? s.Trim() : s)
                .ToList();
        }

        public static List<string> SplitByDoubleNewline(this string input, bool blankLines = false, bool shouldTrim = true)
        {
            return input
                .Split(new[] { "\r\n\r\n", "\r\r", "\n\n" }, StringSplitOptions.None)
                .Where(s => blankLines || !string.IsNullOrWhiteSpace(s))
                .Select(s => shouldTrim ? s.Trim() : s)
                .ToList();
        }

        /// <summary>
        /// Extracts all ints from a string, treats `-` as a negative sign.
        /// </summary>
        /// <param name="str">String to search</param>
        /// <returns>An ordered enumerable of the integers found in the string.</returns>
        public static IEnumerable<int> ExtractInts(this string str)
        {
            return Regex.Matches(str, "-?\\d+").Select(m => int.Parse(m.Value));
        }

        /// <summary>
        /// Extracts all "Words" (including xnoppyt) from a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractWords(this string str)
        {
            return Regex.Matches(str, "[a-zA-z]+").Select(a => a.Value);
        }
    }

    public class Range
    {
        public long Start;
        public long End;
        public long Length => End - Start + 1;

        public Range(long Start, long End)
        {
            this.Start = Start;
            this.End = End;
        }

        //Forced Deep Copy
        public Range(Range other)
        {
            this.Start = other.Start;
            this.End = other.End;
        }

        public override string ToString()
        {
            return $"[{Start}, {End}] ({Length})";
        }
    }

    public class MultiRange
    {
        public List<Range> Ranges = new();

        public MultiRange() { }

        public MultiRange(IEnumerable<Range> Ranges)
        {
            this.Ranges = new(Ranges);
        }

        public MultiRange(MultiRange other)
        {
            foreach (var r in other.Ranges)
            {
                Range n = new(r);
                Ranges.Add(n);
            }
        }

        public long Length => Ranges.Aggregate(1L, (a, b) => a *= b.Length);
    }


    public class DictMultiRange<T>
    {
        public Dictionary<T, Range> Ranges = new();

        public DictMultiRange() { }

        public DictMultiRange(DictMultiRange<T> other)
        {
            foreach (var r in other.Ranges)
            {
                Range n = new(r.Value);
                Ranges[r.Key] = n;
            }
        }

        public long Length => Ranges.Aggregate(1L, (a, b) => a *= b.Value.Length);
    }

}
