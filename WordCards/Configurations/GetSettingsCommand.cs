using WordCards.Configurations.Contexts;
using WordCards.Core.Commands;
using System.Linq;

namespace WordCards.Configurations
{
    public sealed class GetSettingsCommand : EntityCommand, IGetSettingsCommand
    {        
        public Settings Execute()
        {
            Settings result;

            using (var db = new ConfigurationContext())
            {
                var languages = db.Languages.ToDictionary(x => x.Id);

                result = new Settings(AppConfiguration.Instance, languages);
            }

            return result;
        }

        
    }
}
