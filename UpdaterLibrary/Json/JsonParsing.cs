using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
