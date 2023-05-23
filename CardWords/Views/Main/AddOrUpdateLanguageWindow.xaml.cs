using CardWords.Business.Languages;
using CardWords.Core.Helpers;
using CardWords.Core.Validations;
using CardWords.Extensions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CardWords.Views.Main
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdateLanguageWindow.xaml
    /// </summary>
    public partial class AddOrUpdateLanguageWindow : Window
    {
        private readonly ValidationManager validationManager;
        private Language language;

        public AddOrUpdateLanguageWindow(Language editLanguage)
        {
            InitializeComponent();

            var titleName = editLanguage.Id == 0 ? "Добавление" : "Редактирование";
            L_TitleName.Content = titleName;

            language = editLanguage;
            DataContext = editLanguage;

            var errorStyle = Resources["ErrorMessage"] as Style;
            var errorColor = (Color)Resources["ValidationErrorFieldColor"];

            validationManager = new ValidationManager(SP_FieldsWithValidation, errorStyle, errorColor);
        }

        private void Heap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            PrepareToSave();

            SetEnabledWindow(false);

            var isValid = validationManager.Execute(Save);

            if(!isValid)
            {
                SetEnabledWindow(true);
            }
        }

        private void Save()
        {
            var isSaveValid = CommandHelper.GetCommand<ISaveLanguageCommand>().Execute(language);

            if (isSaveValid)
            {
                DialogResult = true;
                Close();
            }
        }

        private void PrepareToSave()
        {
            if(!language.Name.IsNullOrEmpty())
            {
                language.Name = language.Name.Trim();
            }
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
