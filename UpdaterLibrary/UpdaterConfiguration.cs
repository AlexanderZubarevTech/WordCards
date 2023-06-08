using System.Configuration;

namespace UpdaterLibrary
{
    internal static class UpdaterConfiguration
    {
        private static readonly Configuration config;

        static UpdaterConfiguration()
        {
            config = ConfigurationManager.OpenExeConfiguration("WordCards.dll");            
        }

        public static string Get(string key)
        {
            return config.AppSettings.Settings[key].Value;
        }
    }
}
