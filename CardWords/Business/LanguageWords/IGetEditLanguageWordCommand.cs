using WordCards.Core.Commands;

namespace WordCards.Business.LanguageWords
{
    interface IGetEditLanguageWordCommand : IEntityCommand
    {
        public EditLanguageWord Execute(int? id);
    }
}
