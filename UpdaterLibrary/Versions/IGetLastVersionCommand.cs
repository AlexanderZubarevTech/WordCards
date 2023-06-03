using System;
using System.Net.Http;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Versions
{
    internal interface IGetLastVersionCommand : IEntityCommand
    {
        Version Execute(HttpClient httpClient);
    }
}
