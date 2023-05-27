using WordCards.Core.Commands;

namespace WordCards.Business.LanguageWords
{
    interface IDeleteLanguageWordCommand : IEntityCommand
    {
        public void Execute(int id);
    }
}
