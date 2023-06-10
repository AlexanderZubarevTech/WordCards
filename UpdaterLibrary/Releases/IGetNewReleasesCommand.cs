using System;
using System.Collections.Generic;
using System.Net.Http;
using UpdaterLibrary.Commands;
using UpdaterLibrary.Json;

namespace UpdaterLibrary.Releases
{
    internal interface IGetNewReleasesCommand : IEntityCommand
    {
        List<Release> Execute(HttpClient httpClient, string token, Version currentVersion);
    }
}
