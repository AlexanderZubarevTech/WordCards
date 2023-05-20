using CardWords.Configurations.Contexts;
using CardWords.Core.Commands;
using System.Linq;

namespace CardWords.Configurations
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
