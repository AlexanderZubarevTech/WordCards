using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using CardWords.Extensions;

namespace CardWords.Core
{
    public readonly partial struct Id
    {
        public class IdStringConverter : ValueConverter<Id, string>
        {
            public static readonly IdStringConverter Instance = new IdStringConverter();

            private IdStringConverter()
                : base(
                    v => v.AsString(),
                    v => Parse(v))
            {
            }
        }
    }

        
}
