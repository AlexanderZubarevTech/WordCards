using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Updater.Core.Commands;

namespace Updater.Business
{
    public class LoadUpdatersCommand : EntityCommand, ILoadUpdatersCommand
    {
        private static class SettingsKeys
        {
            public const string ApiUrl = "apiURL";
            public const string Owner = "owner";
            public const string Repo = "repo";
            public const string TagsURL = "tagsURL";
            public const string Token = "WC_TOKEN";
            public const string RawConfigUrl = "Raw_Config_URL";
        }

        private static class Headers
        {
            public const string UserAgent = "User-Agent";
            public const string Accept = "Accept";
            public const string Authorization = "Authorization";
        }

        private static readonly string apiUrl = GetApiUrl();

        private HttpClient httpClient;

        public async Task<string> Execute()
        {
            Initialize();

            var isValid = await CkeckOrUpdateAuthorizationToken();

            if(!isValid)
            {
                return "Invalid token. Server need update token.";
            }

            AddHeaders(httpClient.DefaultRequestHeaders);

            // определяем данные запроса
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GetTagsUrl());
            
            // выполняем запрос
            using var response = await httpClient.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                using var newRequest = new HttpRequestMessage(HttpMethod.Get, ConfigurationManager.AppSettings.Get(SettingsKeys.RawConfigUrl));

                var resp = await httpClient.SendAsync(newRequest);
            }

            var res = await response.Content.ReadAsStringAsync();

            httpClient.Dispose();

            Assembly assembly = Assembly.GetExecutingAssembly();

            var version = assembly.GetName().Version;

            

            return res;
        }

        private void Initialize()
        {
            if (httpClient == null)
            {
                var socketsHandler = new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(2)
                };

                httpClient = new HttpClient(socketsHandler);
            }
        }

        private async Task<bool> CkeckOrUpdateAuthorizationToken()
        {
            using HttpRequestMessage checkRequest = new HttpRequestMessage(HttpMethod.Get, GetTagsUrl());

            AddHeaders(checkRequest.Headers);            

            using var checkResponse = await httpClient.SendAsync(checkRequest);

            if (checkResponse.StatusCode == System.Net.HttpStatusCode.Forbidden 
                || checkResponse.StatusCode ==System.Net.HttpStatusCode.NotFound)
            {
                using var configRequest = new HttpRequestMessage(HttpMethod.Get, ConfigurationManager.AppSettings.Get(SettingsKeys.RawConfigUrl));

                using var configResponse = await httpClient.SendAsync(configRequest);

                var content = await configResponse.Content.ReadAsStringAsync();

                var token = GetToken(content);

                checkRequest.Headers.Remove(Headers.Authorization);
                checkRequest.Headers.Add(Headers.Authorization, GetAuthoruzation(token));

                using var secondCkeckResponse = await httpClient.SendAsync(checkRequest);

                if (secondCkeckResponse.StatusCode == System.Net.HttpStatusCode.Forbidden
                || secondCkeckResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                ConfigurationManager.AppSettings.Set(SettingsKeys.Token, token);
            }

            return true;
        }

        private static void AddHeaders(HttpRequestHeaders requestHeaders)
        {
            requestHeaders.Add(Headers.UserAgent, ConfigurationManager.AppSettings.Get(SettingsKeys.Owner));
            requestHeaders.Add(Headers.Accept, "application/vnd.github+json");
            requestHeaders.Add(Headers.Authorization, GetAuthoruzation());
        }

        private static string GetApiUrl()
        {
            var url = ConfigurationManager.AppSettings.Get(SettingsKeys.ApiUrl);
            var owner = ConfigurationManager.AppSettings.Get(SettingsKeys.Owner);
            var repo = ConfigurationManager.AppSettings.Get(SettingsKeys.Repo);

            url = url.Replace("{" + SettingsKeys.Owner + "}", owner);
            url = url.Replace("{" + SettingsKeys.Repo + "}", repo);

            return url;
        }

        private static string GetTagsUrl()
        {
            var tags = ConfigurationManager.AppSettings.Get(SettingsKeys.TagsURL);

            return apiUrl + tags;
        }

        private static string GetAuthoruzation()
        {
            return GetAuthoruzation(ConfigurationManager.AppSettings.Get(SettingsKeys.Token) ?? string.Empty);            
        }

        private static string GetAuthoruzation(string token)
        {
            return $"token {token}";
        }

        private static string GetToken(string xmlString)
        {
            var doc = XDocument.Parse(xmlString);

            var tokenNode = doc.Element("configuration")?
                .Element("appSettings")?
                .Elements("add")
                .FirstOrDefault(x => x.Attribute("key")?.Value == SettingsKeys.Token);

            return tokenNode?.Attribute("value")?.Value ?? string.Empty;
        }
    }
}
