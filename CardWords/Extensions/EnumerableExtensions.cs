using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder.Extensions
{
    public static class EnumerableExtensions
    {
        public static void AddTo<T>(this T item, List<T> list)
        {
            list.Add(item);
        }
    }
}
