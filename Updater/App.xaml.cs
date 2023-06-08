using System;
using System.Windows;

namespace Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            if(e.Args.Length > 0)
            {
                Settings.Token = e.Args[0];

                if(e.Args.Length > 1)
                {
                    Settings.CurrentVersion = Version.Parse(e.Args[1]);
                }
            }
        }
    }
}
