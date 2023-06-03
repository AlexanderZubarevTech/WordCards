using System.Net.Http;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Token
{
    internal interface ICheckAuthorizationTokenCommand : IEntityCommand
    {
        bool? Execute(HttpClient httpClient, string encryptedToken);
    }
}
