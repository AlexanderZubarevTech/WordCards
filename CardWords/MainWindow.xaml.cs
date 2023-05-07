using CardWords.Configurations;
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

namespace CardWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly AppConfiguration AppConfiguration = AppConfiguration.GetInstance();

        public MainWindow()
        {
            InitializeComponent();

            mainGrid.Visibility = Visibility.Visible;
            Btn_Start.Visibility = Visibility.Visible;
            startGrid.Visibility = Visibility.Collapsed;
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            startGrid.Visibility = Visibility.Visible;
            Btn_Start.Visibility = Visibility.Collapsed;
        }

        private void Btn_Start_Count_Click(object sender, RoutedEventArgs e)
        {
            startGrid.Visibility = Visibility.Collapsed;
        }

        private void Btn_Main_Menu_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Visibility = Visibility.Visible;
            Btn_Start.Visibility = Visibility.Visible;
            startGrid.Visibility = Visibility.Collapsed;
        }
    }
}
