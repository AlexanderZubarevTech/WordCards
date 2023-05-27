using WordCards.Core.Commands;

namespace WordCards.Configurations
{
    interface ISaveSettingsCommand : IEntityCommand
    {
        public bool Execute(Settings settings);
    }
}
