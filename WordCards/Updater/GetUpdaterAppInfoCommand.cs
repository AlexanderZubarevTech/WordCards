﻿using System;
using System.Linq;
using System.Reflection;
using UpdaterLibrary;
using WordCards.Configurations;
using WordCards.Core.Commands;
using WordCards.Core.Exceptions;
using WordCards.Core.Validations;

namespace WordCards.Updater
{
    public sealed class GetUpdaterAppInfoCommand : EntityCommand, IGetUpdaterAppInfoCommand
    {
        public UpdaterAppInfo Execute()
        {
            var info = new UpdaterAppInfo();

            Initialize(info);

            try
            {
                SetInfo(info);
            }
            catch (ValidationResultException ex)
            {
                info.ErrorMessage = ex.Message;
            }

            return info;
        }

        private static void Initialize(UpdaterAppInfo info)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            info.CurrentVersion = assembly.GetName().Version;
        }

        private static void SetInfo(UpdaterAppInfo info)
        {
            CheckConnection();

            var isValid = CkeckOrUpdateAuthorizationToken();

            if (!isValid)
            {
                ValidationResult.ThrowError("Invalid token. Server need update token.");
            }

            info.NewVersion = GetNewVersion(info.CurrentVersion);
        }

        private static void CheckConnection()
        {
            var success = UpdaterProvider.CheckNetworkConnection();

            if (!success)
            {
                ValidationResult.ThrowError("Проверьте подключение к интернету.");
            }

            var successService = UpdaterProvider.CheckServiceConnection();

            if (!successService)
            {
                ValidationResult.ThrowError("Сервис недоступен. Попробуйте позже.");
            }
        }

        private static bool CkeckOrUpdateAuthorizationToken()
        {
            var defaultTokenValid = UpdaterProvider.CheckToken(AppConfiguration.Instance.GitHubApiToken);

            if (defaultTokenValid != null)
            {
                return defaultTokenValid.Value;
            }

            var token = UpdaterProvider.GetRemoteToken();

            var tokenValid = UpdaterProvider.CheckToken(token);

            if (tokenValid == true)
            {
                UpdateToken(token);

                return true;
            }

            return false;
        }

        private static void UpdateToken(string token)
        {
            using (var db = new ConfigurationContext())
            {
                var id = AppConfiguration.GetConfigurationId(x => x.GitHubApiToken);

                var entity = db.Configurations.ToList().First(x => x.Id == id);

                entity.Value = token;

                db.Update(entity);

                db.SaveChanges();

                AppConfiguration.Refresh();
            }
        }

        private static Version? GetNewVersion(Version currentVersion)
        {
            var lastVersion = UpdaterProvider.GetLastVersion(AppConfiguration.Instance.GitHubApiToken);

            if (lastVersion != null && lastVersion > currentVersion)
            {
                return lastVersion;
            }

            return null;
        }
    }
}