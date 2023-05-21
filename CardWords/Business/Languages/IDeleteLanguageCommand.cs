using CardWords.Core.Commands;

namespace CardWords.Business.Languages
{
    interface IDeleteLanguageCommand : IEntityCommand
    {
        public void Execute(int id);
    }
}
