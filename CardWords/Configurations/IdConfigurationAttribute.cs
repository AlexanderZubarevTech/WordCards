using System;

namespace CardWords.Configurations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IdConfigurationAttribute : Attribute
    {
        public IdConfigurationAttribute() { }

        public IdConfigurationAttribute(string id) 
        { 
            Id = id;
        }

        public string Id { get; }
    }
}
