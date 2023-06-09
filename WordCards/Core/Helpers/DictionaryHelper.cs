﻿using System.Collections.Generic;

namespace WordCards.Core.Helpers
{
    public static class DictionaryHelper
    {
        public static Dictionary<TKey, TValue> Empty<TKey, TValue>()
            where TKey : notnull
        {
            return new Dictionary<TKey, TValue>(0);
        }        
    }
}
