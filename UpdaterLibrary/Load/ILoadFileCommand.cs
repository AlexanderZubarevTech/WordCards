using System.Net.Http;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Load
{
    internal interface ILoadFileCommand : IEntityCommand
    {
        void Execute(HttpClient httpClient, string url, string path);
    }
}
