using CardWords.Core.Commands;
using CardWords.Core.Ids;
using System.Collections.Generic;

namespace CardWords.Configurations
{
    internal interface ILoadConfigurationCommand : IEntityCommand
    {
        public IReadOnlyDictionary<Id, Configuration> Execute();
    }
}
