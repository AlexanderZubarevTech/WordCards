using CardWords.Core.Commands;
using System.Collections.Generic;

namespace CardWords.Configurations
{
    internal interface ILoadConfigurationCommand : IEntityCommand
    {
        public IReadOnlyDictionary<string, Configuration> Execute();
    }
}
