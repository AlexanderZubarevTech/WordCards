using WordCards.Core.Commands;
using System.Collections.ObjectModel;

namespace WordCards.Business.Languages
{
    interface IGetLanguagesCommand : IEntityCommand
    {
        public ObservableCollection<Language> Execute();
    }
}
