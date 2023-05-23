using CardWords.Business.Languages;
using CardWords.Core.Helpers;
using CardWords.Core.Validations;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CardWords.Views.Main
{
    /// <summary>
    /// Логика взаимодействия для DeleteLanguageWindow.xaml
    /// </summary>
    public partial class DeleteLanguageWindow : Window
    {
        private readonly ValidationManager validationManager;
        private readonly Language language;

        public DeleteLanguageWindow(Language language)
        {
            InitializeComponent();

            this.language = language;
            DataContext = language;

            var errorStyle = Resources["ErrorMessage"] as Style;
            var errorColor = (Color)Resources["ValidationErrorFieldColor"];

            validationManager = new ValidationManager(SP_MessageWithValidation, errorStyle, errorColor);
        }

        private void Heap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Delete(object sender, MouseButtonEventArgs e)
        {
            Delete();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Delete();
            }

            if (e.Key == Key.Escape)
            {
                Cancel();
            }
        }

        private void Delete()
        {
            SetEnabledWindow(false);

            var isValid = validationManager.Execute(DeleteInteral);

            if (!isValid)
            {
                SetEnabledWindow(true);

                return;
            }

            DialogResult = true;
            Close();
        }

        private void DeleteInteral()
        {
            CommandHelper.GetCommand<IDeleteLanguageCommand>().Execute(language.Id);
        }

        private void Cancel()
        {
            DialogResult = false;
            Close();
        }

        private void SetEnabledWindow(bool isEnabled)
        {
            if (isEnabled)
            {
                G_DisabledWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                G_DisabledWindow.Visibility = Visibility.Visible;
            }

            IsEnabled = isEnabled;
        }
    }
}
