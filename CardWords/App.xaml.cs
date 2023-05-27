using CardWords.Core.Helpers;
using CardWords.Core.ReadmeDeploy;
using System.Windows;

namespace CardWords
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            var needDeploy = CommandHelper.GetCommand<INeedDeployCommand>().Execute();

            if (needDeploy)
            {
                CommandHelper.GetCommand<IDeployCommand>().Execute();
            }
        }
    }
}
