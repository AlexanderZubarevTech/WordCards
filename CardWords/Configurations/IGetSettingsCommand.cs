using CardWords.Core.Commands;

namespace CardWords.Configurations
{
    interface IGetSettingsCommand : IEntityCommand
    {
        public Settings Execute();
    }
}
