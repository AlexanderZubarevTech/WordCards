using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UpdaterLibrary.Commands;
using UpdaterLibrary.Json;

namespace UpdaterLibrary.Versions
{
    internal sealed class GetLastVersionCommand : EntityCommand, IGetLastVersionCommand
    {
        public Version Execute(HttpClient httpClient)
        {
            return Task.Run(() => GetVersionAsync(httpClient)).GetAwaiter().GetResult();
        }

        private async Task<Version> GetVersionAsync(HttpClient httpClient)
        {
            Version result = null;

            using (var request = new HttpRequestMessage(HttpMethod.Get, ApiHelper.TagsUrl))
            {
                using (var response = await httpClient.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var tags = JsonParsing.Parse<Tag>(content);

                    result = tags
                        .Where(x => x != null)
                        .OrderByDescending(x => x.Version)
                        .Select(x => x.Version)
                        .FirstOrDefault();
                }
            }

            return result;
        }
    }
}
