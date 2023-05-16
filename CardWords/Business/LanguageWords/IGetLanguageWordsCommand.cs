using CardWords.Core.Commands;
using System.Collections.ObjectModel;

namespace CardWords.Business.LanguageWords
{
    interface IGetLanguageWordsCommand : IEntityCommand
    {
        public ObservableCollection<LanguageWord> Execute(string name, bool withoutTranscription);
    }
}
