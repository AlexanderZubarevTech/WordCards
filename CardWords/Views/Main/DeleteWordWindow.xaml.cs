using CardWords.Business.LanguageWords;
using System.Windows;
using System.Windows.Input;

namespace CardWords.Views.Main
{
    /// <summary>
    /// Логика взаимодействия для DeleteWordWindow.xaml
    /// </summary>
    public partial class DeleteWordWindow : Window
    {
        public DeleteWordWindow(LanguageWordView word)
        {
            InitializeComponent();

            DataContext = word;
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Delete(object sender, MouseButtonEventArgs e)
        {
            Delete();
        }        

        private void Delete()
        {
            DialogResult = true;
            Close();
        }

        private void Cancel()
        {
            DialogResult = false;
            Close();
        }

        private void Heap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Delete();
            }

            if(e.Key == Key.Escape)
            {
                Cancel();
            }
        }
    }
}
