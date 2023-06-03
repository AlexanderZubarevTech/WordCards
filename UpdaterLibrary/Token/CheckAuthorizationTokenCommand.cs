using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Token
{
    internal class CheckAuthorizationTokenCommand : EntityCommand, ICheckAuthorizationTokenCommand
    {
        private HttpClient _httpClient;

        public bool? Execute(HttpClient httpClient, string token)
        {
            _httpClient = httpClient;           

            var result = IsValidToken(token);

            _httpClient.Dispose();

            return result;
        }      

        private bool? IsValidToken(string token)
        {
            var checkStatus = Task.Run(() => CheckAvailable(token)).GetAwaiter().GetResult();

            return IsValidStatus(checkStatus);
        }

        private static bool? IsValidStatus(HttpStatusCode status)
        {
            if (status == HttpStatusCode.OK)
            {
                return true;
            }

            if (status == HttpStatusCode.Unauthorized)
            {
                return null;
            }

            return false;
        }

        private async Task<HttpStatusCode> CheckAvailable(string token)
        {
            HttpStatusCode result = HttpStatusCode.NotFound;

            using (HttpRequestMessage checkRequest = new HttpRequestMessage(HttpMethod.Get, ApiHelper.TagsUrl))
            {
                ApiHelper.AddHeaders(checkRequest.Headers, token);

                using (var checkResponse = await _httpClient.SendAsync(checkRequest))
                {
                    result = checkResponse.StatusCode;
                }
            }

            return result;
        }
    }
}
