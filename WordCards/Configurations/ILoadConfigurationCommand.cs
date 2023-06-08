using System.Collections.Generic;
using WordCards.Core.Commands;

namespace WordCards.Configurations
{
    internal interface ILoadConfigurationCommand : IEntityCommand
    {
        public IReadOnlyDictionary<string, Configuration> Execute();
    }
}
