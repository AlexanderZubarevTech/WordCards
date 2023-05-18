using CardWords.Core.Commands;

namespace CardWords.Business.LanguageWords
{
    interface ISaveEditLanguageWordCommand : IEntityCommand
    {
        public bool Execute(EditLanguageWord editWord);
    }
}
