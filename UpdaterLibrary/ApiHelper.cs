using System.Configuration;
using System.Net.Http.Headers;
using UpdaterLibrary.Token;

namespace UpdaterLibrary
{
    internal static class ApiHelper
    {
        public static readonly string ApiUrl = GetApiUrl();
        public static readonly string TagsUrl = GetTagsUrl();

        private static string GetApiUrl()
        {
            var url = ConfigurationManager.AppSettings.Get(SettingsKeys.ApiUrl);
            var owner = ConfigurationManager.AppSettings.Get(SettingsKeys.Owner);
            var repo = ConfigurationManager.AppSettings.Get(SettingsKeys.Repository);

            url = url.Replace("{" + SettingsKeys.Owner + "}", owner);
            url = url.Replace("{" + SettingsKeys.Repository + "}", repo);

            return url;
        }

        private static string GetTagsUrl()
        {
            var tags = ConfigurationManager.AppSettings.Get(SettingsKeys.TagsURL);

            return ApiUrl + tags;
        }

        public static void AddHeaders(HttpRequestHeaders requestHeaders, string encryptedToken)
        {
            var token = Security.Decrypt(encryptedToken, ConfigurationManager.AppSettings.Get(SettingsKeys.TokenKey));
            var headerValueToken = string.Format(ConfigurationManager.AppSettings.Get(SettingsKeys.HeadersAuthorization), token);

            requestHeaders.Add(HttpHeaders.UserAgent, ConfigurationManager.AppSettings.Get(SettingsKeys.Owner));
            requestHeaders.Add(HttpHeaders.Accept, ConfigurationManager.AppSettings.Get(SettingsKeys.HeadersAccept));
            requestHeaders.Add(HttpHeaders.Authorization, headerValueToken);
        }
    }
}
