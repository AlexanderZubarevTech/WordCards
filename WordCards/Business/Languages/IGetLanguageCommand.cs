using WordCards.Core.Commands;

namespace WordCards.Business.Languages
{
    interface IGetLanguageCommand : IEntityCommand
    {
        public Language Execute(int id);
    }
}
