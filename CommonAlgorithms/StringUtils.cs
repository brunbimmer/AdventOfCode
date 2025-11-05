using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonAlgorithms
{
    public class StringUtils
    {
        public static int CountCharacters(string line, char value)
        {
            return (line.ToCharArray().Count(x => x == value));
        }

        public static IEnumerable<int> AllIndexesOf(string str, string searchString)
        {
            int minIndex = str.IndexOf(searchString);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchString, minIndex + searchString.Length);
            }
        }
    }

    static class StringExtensions
    {
        static public long[] ToLongs(this string input, string splitter)
            => [.. input.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse)];

    }
}
