using WordCards.Core.Commands;
using System.Collections.Generic;
using System.Linq;

namespace WordCards.Configurations
{
    public class LoadConfigurationCommand : EntityCommand, ILoadConfigurationCommand
    {
        public IReadOnlyDictionary<string, Configuration> Execute()
        {
            Dictionary<string, Configuration> result;            

            using(var db = new ConfigurationContext())
            {
                result = db.Configurations.ToDictionary(x => x.Id, x => x);
            }

            return result;
        }
    }
}
