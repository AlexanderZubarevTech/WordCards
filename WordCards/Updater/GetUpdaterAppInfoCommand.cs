using System.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using WordCards.Core.Commands;
using WordCards.Configurations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using WordCards.Core.Validations;
using WordCards.Core.Exceptions;
using System.Net;
using WordCards.Core.Helpers;
using WordCards.Core.Connection;

namespace WordCards.Updater
{
    public sealed class GetUpdaterAppInfoCommand : EntityCommand, IGetUpdaterAppInfoCommand
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

        private static string GetApiUrl()
        {
            var url = ConfigurationManager.AppSettings.Get(SettingsKeys.ApiUrl);
            var owner = ConfigurationManager.AppSettings.Get(SettingsKeys.Owner);
            var repo = ConfigurationManager.AppSettings.Get(SettingsKeys.Repo);

            url = url.Replace("{" + SettingsKeys.Owner + "}", owner);
            url = url.Replace("{" + SettingsKeys.Repo + "}", repo);

            return url;
        }        

        public async Task<UpdaterAppInfo> Execute()
        {
            var info = new UpdaterAppInfo();

            Initialize(info);

            try
            {
                await SetInfo(info);
            }
            catch (ValidationResultException ex)
            {
                info.ErrorMessage = ex.Message;
            }

            httpClient.Dispose();

            return info;
        }

        private void Initialize(UpdaterAppInfo info)
        {
            if (httpClient == null)
            {
                var socketsHandler = new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                    ConnectTimeout = TimeSpan.FromMinutes(1)
                };

                httpClient = new HttpClient(socketsHandler);
            }            

            Assembly assembly = Assembly.GetExecutingAssembly();            

            info.CurrentVersion = assembly.GetName().Version;
        }

        private async Task SetInfo(UpdaterAppInfo info)
        {
            CheckConnection();

            var isValid = await CkeckOrUpdateAuthorizationToken();

            if (!isValid)
            {
                ValidationResult.ThrowError("Invalid token. Server need update token.");                
            }

            AddHeaders(httpClient.DefaultRequestHeaders, AppConfiguration.Instance.GitHubApiToken);

            info.NewVersion = await GetNewVersion(info.CurrentVersion);
        }

        private static void CheckConnection()
        {
            var success = CommandHelper.GetCommand<ICheckConnectionCommand>().Execute();

            if(!success)
            {
                ValidationResult.ThrowError("Проверьте подключение к интернету.");
            }
        }

        private async Task<bool> CkeckOrUpdateAuthorizationToken()
        {
            var defaultTokenValid = await IsValidToken(AppConfiguration.Instance.GitHubApiToken);

            if(defaultTokenValid != null)
            {
                return defaultTokenValid.Value;
            }            

            var token = await GetRemoteToken();

            var tokenValid = await IsValidToken(token);

            if (tokenValid == true)
            {
                UpdateToken(token);

                return true;
            }

            return false;
        }

        private async Task<bool?> IsValidToken(string token)
        {
            var checkStatus = await CheckAvailable(token);

            return IsValidStatus(checkStatus);
        }

        private static bool? IsValidStatus(System.Net.HttpStatusCode status)
        {
            if(status == HttpStatusCode.OK)
            {
                return true;
            }

            if(status == HttpStatusCode.Unauthorized)
            {
                return null;
            }

            return false;
        }

        private async Task<System.Net.HttpStatusCode> CheckAvailable(string token)
        {
            using HttpRequestMessage checkRequest = new HttpRequestMessage(HttpMethod.Get, GetTagsUrl());

            AddHeaders(checkRequest.Headers, token);            

            using var checkResponse = await httpClient.SendAsync(checkRequest);

            return checkResponse.StatusCode;
        }

        private static string GetTagsUrl()
        {
            var tags = ConfigurationManager.AppSettings.Get(SettingsKeys.TagsURL);

            return apiUrl + tags;
        }

        private static void AddHeaders(HttpRequestHeaders requestHeaders, string token)
        {
            requestHeaders.Add(Headers.UserAgent, ConfigurationManager.AppSettings.Get(SettingsKeys.Owner));
            requestHeaders.Add(Headers.Accept, "application/vnd.github+json");
            requestHeaders.Add(Headers.Authorization, $"token {token}");
        }

        private async Task<string> GetRemoteToken()
        {
            using var configRequest = new HttpRequestMessage(HttpMethod.Get, ConfigurationManager.AppSettings.Get(SettingsKeys.RawConfigUrl));

            using var configResponse = await httpClient.SendAsync(configRequest);

            var content = await configResponse.Content.ReadAsStringAsync();

            return GetTokenFromXml(content);
        }

        private static string GetTokenFromXml(string xmlString)
        {
            var doc = XDocument.Parse(xmlString);

            var tokenNode = doc.Element("configuration")?
                .Element("appSettings")?
                .Elements("add")
                .FirstOrDefault(x => x.Attribute("key")?.Value == SettingsKeys.Token);

            return tokenNode?.Attribute("value")?.Value ?? string.Empty;
        }

        private static void UpdateToken(string token)
        {
            using (var db = new ConfigurationContext())
            {
                var id = AppConfiguration.GetConfigurationId(x => x.GitHubApiToken);

                var entity = db.Configurations.First(x => x.Id == id);

                entity.Value = token;

                db.Update(entity);

                db.SaveChanges();

                AppConfiguration.Refresh();
            }
        }

        private async Task<Version?> GetNewVersion(Version currentVersion)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GetTagsUrl());            

            using var response = await httpClient.SendAsync(request);

            var tags = JsonSerializer.DeserializeAsyncEnumerable<Tag>(response.Content.ReadAsStream());

            var lastVersion = tags.ToBlockingEnumerable()
                .Where(x => x != null)
                .OrderByDescending(x => x.Version)
                .Select(x => x.Version)
                .FirstOrDefault();

            if(lastVersion != null && lastVersion > currentVersion)
            {
                return lastVersion;
            }

            return null;
        }

        private sealed class Tag
        {
            public sealed class CommitInfo
            {
                [JsonPropertyName("sha")]
                public string Sha { get; set; }

                [JsonPropertyName("url")]
                public string Url { get; set; }
            }

            private string name;

            [JsonPropertyName("name")]
            public string Name 
            {
                get { return name; } 
                set
                {
                    name = value;

                    Version = Version.Parse(name.Replace("v", string.Empty));
                } 
            }

            [JsonIgnore]
            public Version Version { get; private set; }

            [JsonPropertyName("commit")]
            public CommitInfo Commit { get; set; }

            [JsonPropertyName("zipball_url")]
            public string ZipBallUrl { get; set; }

            [JsonPropertyName("tarball_url")]
            public string TarBallUrl { get; set; }

            [JsonPropertyName("node_id")]
            public string NodeId { get; set; }
        }
    }
}
