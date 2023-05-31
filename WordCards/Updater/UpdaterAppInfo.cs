using System;
using WordCards.Extensions;

namespace WordCards.Updater
{
    public sealed class UpdaterAppInfo
    {
        public Version CurrentVersion { get; set; }

        public Version? NewVersion { get; set; }

        public bool HasNewVersion => NewVersion != null;

        public string? ErrorMessage { get; set; }

        public bool HasError => !ErrorMessage.IsNullOrEmpty();
    }
}
