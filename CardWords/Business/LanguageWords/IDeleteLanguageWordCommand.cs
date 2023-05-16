using CardWords.Core.Commands;

namespace CardWords.Business.LanguageWords
{
    interface IDeleteLanguageWordCommand : IEntityCommand
    {
        public void Execute(int id);
    }
}
