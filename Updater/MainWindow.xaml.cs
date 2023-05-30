using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Updater.Business;
using Updater.Core.Helpers;

namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Owner.Closed += Start;
        }

        private void CloseOwner()
        {
            Owner.Close();
        }

        private void Start(object? sender, EventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        private async void GetData()
        {
            TB_Result.Text = string.Empty;

            var res = await CommandHelper.GetCommand<ILoadUpdatersCommand>().Execute();

            TB_Result.Text = res;
        }
    }
}
