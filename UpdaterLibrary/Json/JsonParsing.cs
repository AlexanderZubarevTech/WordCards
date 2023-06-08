using System.Collections.Generic;
using System.Text.Json;

namespace UpdaterLibrary.Json
{
    internal static class JsonParsing
    {
        public static IEnumerable<TJsonEntity> Parse<TJsonEntity>(string text)
            where TJsonEntity : IJsonEntity
        {
            return JsonSerializer.Deserialize<IEnumerable<TJsonEntity>>(text);
        }
    }
}
