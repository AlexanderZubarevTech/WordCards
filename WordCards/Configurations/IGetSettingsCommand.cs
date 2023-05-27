using WordCards.Core.Commands;

namespace WordCards.Configurations
{
    interface IGetSettingsCommand : IEntityCommand
    {
        public Settings Execute();
    }
}
