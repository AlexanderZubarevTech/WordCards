using CardWords.Business.LanguageWords;
using CardWords.Core.Helpers;
using CardWords.Core.Validations;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CardWords.Views.Main
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdateWordWindow.xaml
    /// </summary>
    public partial class AddOrUpdateWordWindow : Window
    {
        private static Color errorColor = Color.FromRgb(108, 36, 33); // #6c2421

        private EditLanguageWord word;
        private ValidationManager validationManager;

        public AddOrUpdateWordWindow(EditLanguageWord editWord)
        {
            InitializeComponent();

            var titleName = editWord.Id == 0 ? "Добавление" : "Редактирование";
            L_TitleName.Content = titleName;

            word = editWord;
            DataContext = editWord;

            var errorStyle = Resources["ErrorMessage"] as Style;

            validationManager = new ValidationManager(SP_FieldsWithValidation, errorStyle, errorColor, Save);
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

            validationManager.Execute();
        }

        private void Save()
        {
            var isSaveValid = CommandHelper.GetCommand<ISaveEditLanguageWordCommand>().Execute(word);

            if (isSaveValid)
            {
                DialogResult = true;
                Close();
            }
        }        

        private void PrepareToSave()
        {
            word.LanguageWordName = word.LanguageWordName.Trim().ToLower();
            word.Transcription = word.Transcription.Trim().ToLower();
            word.Translation = word.Translation.Trim().ToLower();
        }
    }
}
