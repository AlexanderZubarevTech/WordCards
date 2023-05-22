using CardWords.Business.LanguageWords;
using CardWords.Configurations;
using CardWords.Core.Helpers;
using CardWords.Core.Validations;
using CardWords.Extensions;
using CardWords.Views.Common;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CardWords.Views.Main
{
    /// <summary>
    /// Логика взаимодействия для LoadLibraryWindow.xaml
    /// </summary>
    public partial class LoadLibraryWindow : Window
    {
        private static Color errorColor = Color.FromRgb(108, 36, 33); // #6c2421

        private ValidationManager validationManager;
        private Dispatcher mainDispatcher;
        private string fullFileName;
        private Settings settings;

        public LoadLibraryWindow()
        {
            InitializeComponent();

            mainDispatcher = Dispatcher.CurrentDispatcher;

            settings = CommandHelper.GetCommand<IGetSettingsCommand>().Execute();

            TB_CurrentLanguageName.Text = settings.CurrentLanguage.Name;
            TB_TranslationLanguageName.Text = settings.TranslationLanguage.Name;

            PB_Progress.Value = 0;

            WP_FoundWords.Visibility = Visibility.Collapsed;
            WP_NewWords.Visibility = Visibility.Collapsed;
            WP_Progress_Percent.Visibility = Visibility.Collapsed;

            Btn_Save.IsEnabled = false;

            var errorStyle = Resources["ErrorMessage"] as Style;

            validationManager = new ValidationManager(SP_MessageWithValidation, errorStyle, errorColor);
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
            validationManager.Execute(Save);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var isValid = validationManager.Execute(Load);

            if(!isValid)
            {
                WP_Progress_Percent.Visibility = Visibility.Collapsed;
                Btn_Save.IsEnabled = false;
                fullFileName = string.Empty;
            }
        }

        private void PB_Progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var percent = Math.Round(PB_Progress.Value / PB_Progress.Maximum * 100, 1);

            TB_Progress_Percent.Text = percent.ToString();
        }

        private async void Load()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".xml"; 
            dialog.Filter = "XML Documents (.xml)|*.xml";

            if (dialog.ShowDialog() == true)
            {
                fullFileName = dialog.FileName;

                Btn_Save.IsEnabled = false;

                WP_FoundWords.Visibility = Visibility.Collapsed;
                WP_NewWords.Visibility = Visibility.Collapsed;
                WP_Progress_Percent.Visibility = Visibility.Visible;

                PB_Progress.Value = 0;

                var info = await CommandHelper.GetCommand<ICheckLibraryFileCommand>().Execute(dialog.FileName, mainDispatcher, PB_Progress);

                TB_FoundWordsCount.Text = info.WordsCount.ToString();
                TB_NewWordsCount.Text = info.NewWordsCount.ToString();

                WP_FoundWords.Visibility = Visibility.Visible;
                WP_NewWords.Visibility = Visibility.Visible;

                WP_Progress_Percent.Visibility = Visibility.Collapsed;

                Btn_Save.IsEnabled = true;
            } 
            else if(fullFileName.IsNullOrEmpty())
            {
                Btn_Save.IsEnabled = false;
            }
        }

        private async void Save()
        {
            Btn_Save.IsEnabled = false;

            WP_FoundWords.Visibility = Visibility.Collapsed;
            WP_NewWords.Visibility = Visibility.Collapsed;
            WP_Progress_Percent.Visibility = Visibility.Visible;

            PB_Progress.Value = 0;

            var info = await CommandHelper.GetCommand<ILoadFileToLibraryCommand>().Execute(fullFileName, mainDispatcher, PB_Progress);

            var messages = new string[]
            {
                $"В библиотеку ",
                $"{settings.CurrentLanguage.Name} - {settings.TranslationLanguage.Name}",
                $"было добавлено слов: {info.NewWordsCount}"
            };

            var messageWindow = new MessageWindow(messages);

            SetEnabledWindow(false);

            messageWindow.ShowDialog();

            DialogResult = true;

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
