using System;
using System.Linq;

namespace CommonAlgorithms
{
    public static class KnotHash
    {
        public static byte[] Compute(string input)
        {
            if (input.Any(character => character > 127))
                throw new FormatException("Knot-hash input must be ASCII text.");
            int[] lengths = input.Select(character => (int)character)
                .Concat(new[] { 17, 31, 73, 47, 23 }).ToArray();
            int[] sparse = Twist(lengths, 256, 64);
            var dense = new byte[16];
            for (int block = 0; block < dense.Length; block++)
                for (int offset = 0; offset < 16; offset++)
                    dense[block] ^= (byte)sparse[block * 16 + offset];
            return dense;
        }

        public static int[] Twist(int[] lengths, int size, int rounds)
        {
            if (size < 1 || rounds < 0 || lengths.Any(length => length < 0 || length > size))
                throw new ArgumentException("Lengths must fit the circular list and rounds must be non-negative.");
            int[] values = Enumerable.Range(0, size).ToArray();
            int position = 0;
            int skip = 0;
            for (int round = 0; round < rounds; round++)
            {
                foreach (int length in lengths)
                {
                    for (int offset = 0; offset < length / 2; offset++)
                    {
                        int first = (position + offset) % size;
                        int second = (position + length - 1 - offset) % size;
                        (values[first], values[second]) = (values[second], values[first]);
                    }
                    position = (int)(((long)position + length + skip) % size);
                    skip++;
                }
            }
            return values;
        }
    }
}
