using System.Net.Http;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Token
{
    internal interface IGetRemoteTokenCommand : IEntityCommand
    {
        string Execute(HttpClient httpClient);
    }
}
