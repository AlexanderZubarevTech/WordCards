using System.Collections.Generic;

namespace WordCards.Extensions
{
    public static class EnumerableExtensions
    {
        public static void AddTo<T>(this T item, List<T> list)
        {
            list.Add(item);
        }
    }
}
