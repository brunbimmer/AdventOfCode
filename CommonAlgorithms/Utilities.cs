using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public static class Utilities
    {
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
