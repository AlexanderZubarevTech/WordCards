using WordCards.Core.Commands;
using System.Collections.Generic;

namespace WordCards.Configurations
{
    internal interface ILoadConfigurationCommand : IEntityCommand
    {
        public IReadOnlyDictionary<string, Configuration> Execute();
    }
}
