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
    }
}
