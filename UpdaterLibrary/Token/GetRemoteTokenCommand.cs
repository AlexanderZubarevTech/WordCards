using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Token
{
    internal class GetRemoteTokenCommand : EntityCommand, IGetRemoteTokenCommand
    {
        private HttpClient _httpClient;

        public string Execute(HttpClient httpClient) 
        {
            _httpClient = httpClient;

            var result = GetRemoteToken();

            _httpClient.Dispose();

            return result;
        }

        private string GetRemoteToken()
        {
            return Task.Run(() => GetRemoteTokenAsync()).GetAwaiter().GetResult();
        }

        private async Task<string> GetRemoteTokenAsync()
        {
            var result = string.Empty;

            var rawUrl = ConfigurationManager.AppSettings.Get(SettingsKeys.RawConfigUrl);

            using (var configRequest = new HttpRequestMessage(HttpMethod.Get, rawUrl))
            {
                using (var configResponse = await _httpClient.SendAsync(configRequest))
                {
                    var content = await configResponse.Content.ReadAsStringAsync();

                    result = GetTokenFromXml(content);
                }
            }

            return result;
        }

        private static string GetTokenFromXml(string xmlString)
        {
            var doc = XDocument.Parse(xmlString);

            var tokenName = ConfigurationManager.AppSettings.Get(SettingsKeys.TokenName);

            var tokenNode = doc.Element("configuration")?
                .Element("appSettings")?
                .Elements("add")
                .FirstOrDefault(x => x.Attribute("key")?.Value == tokenName);

            return tokenNode?.Attribute("value")?.Value ?? string.Empty;
        }
    }
}
