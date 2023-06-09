﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace WordCards.Core.ReadmeDeploy
{
    public static class DeployFilesHelper
    {
        private static readonly string directoryPath = Path.Combine($"{Directory.GetCurrentDirectory()}", "ReadmeDeploy");

        public static IReadOnlyDictionary<string, string> GetNewFiles(DbSet<ReadmeDeploy> deploys)
        {
            var deploysByIds = deploys.ToDictionary(x => x.Id);

            var files = GetFiles();

            var newIds = files.Keys.Except(deploysByIds.Keys).ToList();

            return files
                .Where(x => newIds.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static Dictionary<string, string> GetFiles()
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            var files = GetAllFiles(directoryInfo)
                .OrderBy(x => x.FullName)
                .ToArray();

            var result = new Dictionary<string, string>(files.Length);

            foreach (var file in files)
            {
                var doc = new XmlDocument();
                doc.Load(file.OpenText());

                var root = doc.DocumentElement;

                if (root != null)
                {
                    var id = root.Attributes.GetNamedItem("id")?.Value;

                    result.Add(id, file.FullName);
                }
            }

            return result;
        }

        private static List<FileInfo> GetAllFiles(DirectoryInfo directoryInfo)
        {
            var files = directoryInfo.GetFiles("*.xml");

            var resultList = new List<FileInfo>();

            if (files != null)
            {
                resultList.AddRange(files);
            }

            var directories = directoryInfo.GetDirectories();

            foreach (var directory in directories)
            {
                var result = GetAllFiles(directory);

                resultList.AddRange(result);
            }

            return resultList;
        }
    }
}
