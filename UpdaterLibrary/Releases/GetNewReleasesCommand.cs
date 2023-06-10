using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UpdaterLibrary.Commands;
using UpdaterLibrary.Json;

namespace UpdaterLibrary.Releases
{
    internal sealed class GetNewReleasesCommand : EntityCommand, IGetNewReleasesCommand
    {
        public List<Release> Execute(HttpClient httpClient, string token, Version currentVersion)
        {
            var result = Task.Run(() => GetReleasesAsync(httpClient, token, currentVersion)).GetAwaiter().GetResult();

            httpClient.Dispose();

            return result;
        }

        private async Task<List<Release>> GetReleasesAsync(HttpClient httpClient, string token, Version currentVersion)
        {
            List<Release> result = new List<Release>();

            int page = 1;
            const int pageSize = 50;

            while (true)
            {
                var releases = await GetReleasesAsyncInternal(httpClient, token, currentVersion, page, pageSize);

                if(releases == null || releases.Count == 0)
                {
                    break;
                }

                result.AddRange(releases);

                if(releases.Count < pageSize)
                {
                    break;
                }

                page++;
            }            

            return result.OrderByDescending(x => x.Version).ToList();
        }

        private static async Task<List<Release>> GetReleasesAsyncInternal(HttpClient httpClient, string token, Version currentVersion, int page, int pageSize)
        {
            List<Release> result = null;

            using (var request = new HttpRequestMessage(HttpMethod.Get, ApiHelper.ReleasesUrl))
            {
                ApiHelper.AddHeaders(request.Headers, token);

                ApiHelper.AddReleaseListProperties(request.Properties, page, pageSize);

                using (var response = await httpClient.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var releases = JsonParsing.Parse<Release>(content);

                    result = releases
                        .Where(x => x != null && x.Version > currentVersion)
                        .OrderByDescending(x => x.Version)
                        .ToList();
                }
            }

            return result;
        }
    }
}
