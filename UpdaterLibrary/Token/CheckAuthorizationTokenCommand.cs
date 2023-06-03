using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Token
{
    internal class CheckAuthorizationTokenCommand : EntityCommand, ICheckAuthorizationTokenCommand
    {
        private HttpClient _httpClient;

        public bool? Execute(HttpClient httpClient, string encryptedToken)
        {
            _httpClient = httpClient;            

            var token = Security.Decrypt(encryptedToken, ConfigurationManager.AppSettings.Get(SettingsKeys.TokenKey));

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
                AddHeaders(checkRequest.Headers, token);

                using (var checkResponse = await _httpClient.SendAsync(checkRequest))
                {
                    result = checkResponse.StatusCode;
                }
            }        

            return result;
        }        

        private static void AddHeaders(HttpRequestHeaders requestHeaders, string token)
        {
            requestHeaders.Add(HttpHeaders.UserAgent, ConfigurationManager.AppSettings.Get(SettingsKeys.Owner));
            requestHeaders.Add(HttpHeaders.Accept, ConfigurationManager.AppSettings.Get(SettingsKeys.HeadersAccept));

            var headerValueToken = string.Format(ConfigurationManager.AppSettings.Get(SettingsKeys.HeadersAuthorization), token);

            requestHeaders.Add(HttpHeaders.Authorization, headerValueToken);
        }
    }
}
