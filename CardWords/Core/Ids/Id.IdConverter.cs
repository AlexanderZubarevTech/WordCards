using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using CardWords.Extensions;

namespace CardWords.Core.Ids
{
    public readonly partial struct Id
    {
        public class IdConverter : ValueConverter<Id, int>
        {
            public static readonly IdConverter Instance = new IdConverter();

            public IdConverter()
                : base(
                    v => v.As<int>(),
                    v => Parse(v))
            {
            }
        }
    }
}
