﻿using WordCards.Business.LanguageWords;
using WordCards.Core.Helpers;
using WordCards.Core.Validations;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WordCards.Views.Main
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdateWordWindow.xaml
    /// </summary>
    public partial class AddOrUpdateWordWindow : Window
    {
        private readonly ValidationManager validationManager;        
        private EditLanguageWord word;

        public AddOrUpdateWordWindow(EditLanguageWord editWord)
        {
            InitializeComponent();

            var titleName = editWord.Id == 0 ? "Добавление" : "Редактирование";
            L_TitleName.Content = titleName;

            Btn_Load.Visibility = editWord.Id == 0 ? Visibility.Visible : Visibility.Collapsed;

            word = editWord;
            DataContext = editWord;

            var errorStyle = Resources["ErrorMessage"] as Style;
            var errorColor = (Color) Resources["ValidationErrorFieldColor"];

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

            if (!isValid)
            {
                SetEnabledWindow(true);
            }
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

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var window = new LoadLibraryWindow();            

            Hide();

            if(window.ShowDialog() == true)
            {                
                Close();

                return;
            }

            ShowDialog();
        }
    }
}
