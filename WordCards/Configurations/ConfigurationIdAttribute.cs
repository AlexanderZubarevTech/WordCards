using System;

namespace WordCards.Configurations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConfigurationIdAttribute : Attribute
    {
        public ConfigurationIdAttribute() { }

        public ConfigurationIdAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
