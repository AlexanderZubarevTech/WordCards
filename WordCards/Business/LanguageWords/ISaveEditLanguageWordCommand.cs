using WordCards.Core.Commands;

namespace WordCards.Business.LanguageWords
{
    interface ISaveEditLanguageWordCommand : IEntityCommand
    {
        public bool Execute(EditLanguageWord editWord);
    }
}
