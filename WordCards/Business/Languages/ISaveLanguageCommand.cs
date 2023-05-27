using WordCards.Core.Commands;

namespace WordCards.Business.Languages
{
    interface ISaveLanguageCommand : IEntityCommand
    {
        public bool Execute(Language entity);
    }
}
