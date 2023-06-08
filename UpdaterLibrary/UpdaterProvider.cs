using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UpdaterLibrary.Commands;
using UpdaterLibrary.Connection;
using UpdaterLibrary.Json;
using UpdaterLibrary.Load;
using UpdaterLibrary.Releases;
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
            var hostName = UpdaterConfiguration.Get(SettingsKeys.ServiceAddress);

            return CommandHelper.GetCommand<ICheckConnectionCommand>().Execute(hostName);
        }

        public static Version GetLastVersion(string token)
        {
            return CommandHelper.GetCommand<IGetLastVersionCommand>().Execute(httpClientFactory.CreateClient(), token);
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
            return Security.Encrypt(text, UpdaterConfiguration.Get(SettingsKeys.TokenKey));
        }

        public static List<Release> GetNewReleases(string token, Version currentVersion)
        {
            return CommandHelper.GetCommand<IGetNewReleasesCommand>().Execute(httpClientFactory.CreateClient(), token, currentVersion);
        }

        public static void LoadFile(string url, string path)
        {
            CommandHelper.GetCommand<ILoadFileCommand>().Execute(httpClientFactory.CreateClient(), url, path);
        }
    }
}
