using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Updater.Core.Commands;
using UpdaterLibrary;
using UpdaterLibrary.Json;

namespace Updater.Business
{
    public class LoadUpdatersCommand : EntityCommand, ILoadUpdatersCommand
    {
        private const string packageFileName = "Package.zip";

        public void Execute()
        {
            var releases = Task.Run(() => { return UpdaterProvider.GetNewReleases(Settings.Token, Settings.CurrentVersion); }).GetAwaiter().GetResult();

            if(releases == null || releases.Count == 0) 
            {
                return;
            }

            var directory = LoadPackages(releases);
        }

        private static string LoadPackages(List<Release> releases)
        {
            var path = CreateTempDirectory();

            foreach (var release in releases)
            {
                LoadPackageByRelease(release, path);
            }

            return path;
        }

        private static string CreateTempDirectory()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), Settings.TempDirectory);

            if(Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            return path;
        }

        private static void LoadPackageByRelease(Release release, string tempPath)
        {
            var directoryPath = Path.Combine(tempPath, release.TagName);

            Directory.CreateDirectory(directoryPath);

            var asset = release.Assets.FirstOrDefault(x => x.Name == packageFileName);

            if(asset == null)
            {
                return;
            }

            var filePath = Path.Combine(directoryPath, asset.Name);

            if(File.Exists(filePath))
            {
                throw new Exception($"Exists file {packageFileName} in {directoryPath}");
            }

            UpdaterProvider.LoadFile(asset.BrowserDownloadUrl, filePath);            
        }
    }
}
