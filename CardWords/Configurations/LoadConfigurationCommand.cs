using CardWords.Configurations.Contexts;
using CardWords.Core.Commands;
using CardWords.Core.Ids;
using System.Collections.Generic;
using System.Linq;

namespace CardWords.Configurations
{
    public class LoadConfigurationCommand : EntityCommand, ILoadConfigurationCommand
    {
        public IReadOnlyDictionary<Id, Configuration> Execute()
        {
            Dictionary<Id, Configuration> result;            

            using(var db = new ConfigurationContext())
            {
                result = db.Configurations.ToDictionary(x => x.Id, x => x);
            }

            return result;
        }
    }
}
