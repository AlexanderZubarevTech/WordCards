using CardWords.Core.Commands;

namespace CardWords.Configurations
{
    interface ISaveSettingsCommand : IEntityCommand
    {
        public bool Execute(Settings settings);
    }
}
