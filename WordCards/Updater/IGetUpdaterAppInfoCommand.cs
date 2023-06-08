using WordCards.Core.Commands;

namespace WordCards.Updater
{
    public interface IGetUpdaterAppInfoCommand : IEntityCommand
    {
        public UpdaterAppInfo Execute();
    }
}
