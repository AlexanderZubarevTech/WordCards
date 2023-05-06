using System.Collections.Generic;

namespace CardWords.Extensions
{
    public static class DictionaryExtensions
    {
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            return dictionary;
        }        
    }
}
