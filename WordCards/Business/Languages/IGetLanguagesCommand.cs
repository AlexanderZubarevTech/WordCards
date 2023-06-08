using System.Collections.ObjectModel;
using WordCards.Core.Commands;

namespace WordCards.Business.Languages
{
    interface IGetLanguagesCommand : IEntityCommand
    {
        public ObservableCollection<Language> Execute();
    }
}
