using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> hashSet = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (hashSet.Add(keySelector(element))) { yield return element; }
            }
        }
    }
}
