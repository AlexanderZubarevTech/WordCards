using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Net.Http;
using UpdaterLibrary.Commands;
using UpdaterLibrary.Connection;
using UpdaterLibrary.Token;
using UpdaterLibrary.Versions;

namespace UpdaterLibrary
{
    public static class UpdaterProvider
    {
        private static IHttpClientFactory httpClientFactory;

        static UpdaterProvider()
        {            
            var services = new ServiceCollection();            
            services.AddHttpClient();            
            var serviceProvider = services.BuildServiceProvider();            
            httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        }

        public static bool CheckNetworkConnection()
        {
            return CommandHelper.GetCommand<ICheckConnectionCommand>().Execute();
        }

        public static bool CheckServiceConnection() 
        {
            var hostName = ConfigurationManager.AppSettings.Get(SettingsKeys.ServiceAddress);

            return CommandHelper.GetCommand<ICheckConnectionCommand>().Execute(hostName);
        }

        public static Version GetLastVersion()
        {
            return CommandHelper.GetCommand<IGetLastVersionCommand>().Execute(httpClientFactory.CreateClient());
        }

        /// <summary>
        /// </summary>
        /// <param name="encryptedToken"></param>
        /// <returns>null  - Unauthorized</returns>
        public static bool? CheckToken(string token)
        {
            return CommandHelper.GetCommand<ICheckAuthorizationTokenCommand>().Execute(httpClientFactory.CreateClient(), token);
        }

        /// <summary>
        /// Get remote token from service
        /// </summary>
        /// <returns>Encrypted token</returns>
        public static string GetRemoteToken()
        {
            return CommandHelper.GetCommand<IGetRemoteTokenCommand>().Execute(httpClientFactory.CreateClient());
        }

        public static string Encrypt(string text)
        {
            return Security.Encrypt(text, ConfigurationManager.AppSettings.Get(SettingsKeys.TokenKey));
        }
    }
}
