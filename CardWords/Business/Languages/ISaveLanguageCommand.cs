using CardWords.Core.Commands;

namespace CardWords.Business.Languages
{
    interface ISaveLanguageCommand : IEntityCommand
    {
        public bool Execute(Language entity);
    }
}
