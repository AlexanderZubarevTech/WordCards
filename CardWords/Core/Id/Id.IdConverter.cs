using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using CardWords.Extensions;

namespace CardWords.Core
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
