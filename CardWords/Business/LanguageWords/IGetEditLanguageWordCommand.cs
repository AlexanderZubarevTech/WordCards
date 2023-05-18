using CardWords.Core.Commands;

namespace CardWords.Business.LanguageWords
{
    interface IGetEditLanguageWordCommand : IEntityCommand
    {
        public EditLanguageWord Execute(int? id);
    }
}
