using CardWords.Core.Commands;

namespace CardWords.Business.Languages
{
    interface IGetLanguageCommand : IEntityCommand
    {
        public Language Execute(int id);
    }
}
