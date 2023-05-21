using CardWords.Core.Commands;
using System.Collections.ObjectModel;

namespace CardWords.Business.Languages
{
    interface IGetLanguagesCommand : IEntityCommand
    {
        public ObservableCollection<Language> Execute();
    }
}
