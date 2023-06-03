using System.Configuration;

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
    }
}
