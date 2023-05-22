using CardWords.Core.Commands;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CardWords.Business.LanguageWords
{
    interface ILoadFileToLibraryCommand : IEntityCommand
    {
        public Task<LibraryFileInfo> Execute(string fullFileName, Dispatcher mainDispatcher, ProgressBar progress);
    }
}
