using System.Windows;
using WordCards.Core.Helpers;
using WordCards.Core.ReadmeDeploy;

namespace WordCards
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
