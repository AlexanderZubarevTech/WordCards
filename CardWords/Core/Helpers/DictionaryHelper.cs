using System.Collections.Generic;

namespace Reminder.Core.Helpers
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
