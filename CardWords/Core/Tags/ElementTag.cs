using WordCards.Core.Helpers;
using WordCards.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace WordCards.Core.Tags
{
    public sealed class ElementTag
    {
        private const string separatorTag = ",";
        private const string separatorValue = "=";

        public static Dictionary<string, ElementTag> ParseTag(object? tag)
        {
            if(tag == null)
            {
                return DictionaryHelper.Empty<string, ElementTag>();
            }

            var tagAsString = tag.ToString();

            if(tagAsString.IsNullOrEmptyOrWhiteSpace())
            {
                return DictionaryHelper.Empty<string, ElementTag>();
            }

            var tags = tagAsString.Split(separatorTag);

            var result = new List<ElementTag>();

            for (int i = 0; i < tags.Length; i++)
            {
                var item = tags[i];

                if(item.IsNullOrEmptyOrWhiteSpace())
                {
                    continue;
                }

                if(!item.Contains(separatorValue))
                {
                    new ElementTag(item).AddTo(result);

                    continue;
                }

                var valuePair = item.Split(separatorValue);

                new ElementTag(valuePair[0], valuePair[1]).AddTo(result);
            }

            return result.ToDictionary(x => x.Name);
        }

        private ElementTag(string name) 
            : this(name, string.Empty)
        {            
        }

        private ElementTag(string name, string value)
        {
            Name = name.Trim();
            Value = value.Trim();
        }

        public string Name { get; }

        public string Value { get; }
    }
}
