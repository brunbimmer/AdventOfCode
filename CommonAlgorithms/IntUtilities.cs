using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<long> RangeLong(long start, long count)
        {
            return Enumerable.Range(0, (int)count).Select(i => start + i);
        }

        public static IEnumerable<IEnumerable<T>> SplitWhen<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var sublist = new List<T>();
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    if (sublist.Any())
                    {
                        yield return sublist;
                        sublist = new List<T>();
                    }
                }
                else
                {
                    sublist.Add(item);
                }
            }

            if (sublist.Any())
            {
                yield return sublist;
            }
        }
    }

    public static class IntUtilities
    {

        public static int[,] TrimArray(this int[,] originalArray, int rowToRemove, int columnToRemove)
        {
            int[,] result = new int[originalArray.GetLength(0) - 1, originalArray.GetLength(1) - 1];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }
    }
}
