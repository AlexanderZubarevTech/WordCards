using WordCards.Core.Commands;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WordCards.Business.LanguageWords
{
    interface ICheckLibraryFileCommand : IEntityCommand
    {
        public Task<LibraryFileInfo> Execute(string fullFileName, Dispatcher mainDispatcher, ProgressBar progress);
    }
}
