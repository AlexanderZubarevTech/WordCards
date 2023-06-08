using System;

namespace Updater
{
    internal static class Settings
    {
        public const string TempDirectory = "temp_update";

        public static string Token { get; set; }

        public static Version CurrentVersion { get; set; }
    }
}
