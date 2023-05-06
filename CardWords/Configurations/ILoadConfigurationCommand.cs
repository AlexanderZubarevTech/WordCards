using CardWords.Core;
using CardWords.Core.Commands;
using System.Collections.Generic;

namespace CardWords.Configurations
{
    internal interface ILoadConfigurationCommand : IEntityCommand
    {
        public IReadOnlyDictionary<Id, Configuration> Execute();
    }
}
