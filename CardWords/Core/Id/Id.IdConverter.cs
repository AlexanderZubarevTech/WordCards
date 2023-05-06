using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Reminder.Extensions;

namespace Reminder.Core
{
    public readonly partial struct Id
    {
        public class IdConverter : ValueConverter<Id, int>
        {
            public IdConverter()
                : base(
                    v => v.As<int>(),
                    v => Parse(v))
            {
            }
        }
    }

        
}
