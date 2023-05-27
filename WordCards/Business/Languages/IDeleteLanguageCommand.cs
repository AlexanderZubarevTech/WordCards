using WordCards.Core.Commands;

namespace WordCards.Business.Languages
{
    interface IDeleteLanguageCommand : IEntityCommand
    {
        public void Execute(int id);
    }
}
