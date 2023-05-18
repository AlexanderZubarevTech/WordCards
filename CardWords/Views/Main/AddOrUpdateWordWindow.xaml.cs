using CardWords.Business.LanguageWords;
using CardWords.Core.Helpers;
using CardWords.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CardWords.Views.Main
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdateWordWindow.xaml
    /// </summary>
    public partial class AddOrUpdateWordWindow : Window
    {
        private static Color errorColor = Color.FromRgb(108, 36, 33); // #6c2421
        private static Color defaultColor = Color.FromRgb(77, 39, 139);

        private EditLanguageWord word;

        public AddOrUpdateWordWindow(EditLanguageWord editWord)
        {
            InitializeComponent();

            var titleName = editWord.Id == 0 ? "Добавление" : "Редактирование";
            L_TitleName.Content = titleName;

            word = editWord;
            DataContext = editWord;
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

            var isValid = Validate();

            if (isValid)
            {
                var isSaveValid = CommandHelper.GetCommand<ISaveEditLanguageWordCommand>().Execute(word);

                if(isSaveValid)
                {
                    DialogResult = true;
                    Close();
                }

                if(!isSaveValid)
                {
                    ExistErrorMessage();
                }
            }            
        }

        private bool Validate()
        {
            SP_ErrorMessages.Children.Clear();

            foreach (var item in SP_Fields.Children)
            {
                var grid = item as Grid;

                var isError = false;

                if(grid.Tag != null && grid.Tag.ToString() == "required")
                {
                    var textBox = (grid.Children[1] as Grid).Children[0] as TextBox;

                    if(textBox.Text.IsNullOrEmptyOrWhiteSpace())
                    {
                        var fieldName = (((grid.Children[0] as Grid).Children[0] as StackPanel).Children[0] as TextBlock).Text;

                        RequiredErrorMessage(fieldName);

                        isError = true;
                    }
                }

                var color = isError ? errorColor : defaultColor;

                (((grid.Children[0] as Grid).Children[1] as Grid).Children[0] as Rectangle).Fill = new SolidColorBrush(color);
            }

            return SP_ErrorMessages.Children.Count == 0;
        }

        private void RequiredErrorMessage(string fieldName)
        {
            var block = new TextBlock();
            block.Style = Resources["ErrorMessage"] as Style;
            block.Text = $"Поле \"{fieldName}\" обязательно для заполнения";

            SP_ErrorMessages.Children.Add(block);
        }

        private void ExistErrorMessage()
        {
            var block = new TextBlock();
            block.Style = Resources["ErrorMessage"] as Style;
            block.Text = $"Слово уже существует в библиотеке!";

            SP_ErrorMessages.Children.Add(block);
        }

        private void PrepareToSave()
        {
            word.LanguageWordName = word.LanguageWordName.Trim().ToLower();
            word.Transcription = word.Transcription.Trim().ToLower();
            word.Translation = word.Translation.Trim().ToLower();
        }
    }
}
