using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security;
using UpdaterLibrary.Token;

namespace UpdaterLibrary
{
    internal static class ApiHelper
    {
        public static readonly string ApiUrl = GetApiUrl();
        public static readonly string TagsUrl = GetTagsUrl();
        public static readonly string ReleasesUrl = GetReleasesUrl();

        private static string GetApiUrl()
        {
            var url = UpdaterConfiguration.Get(SettingsKeys.ApiUrl);
            var owner = UpdaterConfiguration.Get(SettingsKeys.Owner);
            var repo = UpdaterConfiguration.Get(SettingsKeys.Repository);

            url = url.Replace("{" + SettingsKeys.Owner + "}", owner);
            url = url.Replace("{" + SettingsKeys.Repository + "}", repo);

            return url;
        }

        private static string GetTagsUrl()
        {
            var tags = UpdaterConfiguration.Get(SettingsKeys.TagsURL);

            return ApiUrl + tags;
        }

        private static string GetReleasesUrl()
        {
            var releases = UpdaterConfiguration.Get(SettingsKeys.ReleaseURL);

            return ApiUrl + releases;
        }

        public static void AddHeaders(HttpRequestHeaders requestHeaders, string encryptedToken)
        {
            var token = Security.Decrypt(encryptedToken, UpdaterConfiguration.Get(SettingsKeys.TokenKey));
            var headerValueToken = string.Format(UpdaterConfiguration.Get(SettingsKeys.HeadersAuthorization), token);

            requestHeaders.Add(HttpHeaders.UserAgent, UpdaterConfiguration.Get(SettingsKeys.Owner));
            requestHeaders.Add(HttpHeaders.Accept, UpdaterConfiguration.Get(SettingsKeys.HeadersAccept));
            requestHeaders.Add(HttpHeaders.Authorization, headerValueToken);
            requestHeaders.Add(HttpHeaders.ApiVersion, UpdaterConfiguration.Get(SettingsKeys.HeadersVersion));
        }

        public static void AddReleaseListProperties(IDictionary<string, object> properties, int page, int pageSize = 50)
        {
            properties.Add(HttpProperties.Release.PageSize, pageSize);
            properties.Add(HttpProperties.Release.Page, page);
        }
    }
}
