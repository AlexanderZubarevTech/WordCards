using WordCards.Core.Commands;
using System.Collections.ObjectModel;

namespace WordCards.Business.LanguageWords
{
    interface IGetLanguageWordsCommand : IEntityCommand
    {
        public ObservableCollection<LanguageWordView> Execute(string name, bool withoutTranscription, WordStatus status);
    }
}
