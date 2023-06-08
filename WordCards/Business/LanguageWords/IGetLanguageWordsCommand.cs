using System.Collections.ObjectModel;
using WordCards.Core.Commands;

namespace WordCards.Business.LanguageWords
{
    interface IGetLanguageWordsCommand : IEntityCommand
    {
        public ObservableCollection<LanguageWordView> Execute(string name, bool withoutTranscription, WordStatus status);
    }
}
