using CardWords.Business.Languages;
using CardWords.Business.LanguageWords;
using CardWords.Configurations;
using CardWords.Core.Helpers;
using CardWords.Core.Validations;
using CardWords.Views.Cards;
using CardWords.Views.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CardWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Color errorColor = Color.FromRgb(108, 36, 33); // #6c2421

        private readonly ValidationManager validationManager;

        private readonly Dictionary<Grid, Grid> menu;

        private Grid activeMenu;

        private ObservableCollection<LanguageWordView> LanguageWords;

        private ObservableCollection<Language> Languages;

        private Settings settings;

        public MainWindow()
        {
            InitializeComponent();

            menu = new Dictionary<Grid, Grid>
            {
                {G_Menu_Main, G_Main},
                {G_Menu_Library, G_Library},
                {G_Menu_Settings, G_Settings}
            };

            SetActiveMenu(G_Menu_Main);
            
            G_Start.Visibility = Visibility.Visible;
            G_Start_Count.Visibility = Visibility.Collapsed;

            IsRunCards = false;

            CbBx_Library_Search_WordStatus.ItemsSource = WordStatus.Items.Values;
            CbBx_Library_Search_WordStatus.SelectedItem = WordStatus.Any;

            settings = CommandHelper.GetCommand<IGetSettingsCommand>().Execute();

            DataContext = settings;

            var errorStyle = Resources["ErrorMessage"] as Style;

            validationManager = new ValidationManager(SP_Settings_FieldsAndValidation, errorStyle, errorColor);            
        }

        public bool IsRunCards { get; set; }

        #region Events

        #region General

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Heap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void G_Button_MouseEnter(object sender, MouseEventArgs e)
        {
            StartHoverAnimation(sender, true);
        }

        private void G_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            StartHoverAnimation(sender, false);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.IsRepeat)
            {
                return;
            }

            if (activeMenu == G_Menu_Main)
            {
                if (G_Start.Visibility == Visibility.Visible && e.Key == Key.Enter)
                {
                    G_Start_MouseLeftButtonDown(sender, e);
                }

                if (G_Start_Count.Visibility == Visibility.Visible && !IsRunCards)
                {
                    if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                    {
                        StartCards(10);
                    }

                    if (e.Key == Key.D2 || e.Key == Key.NumPad2)
                    {
                        StartCards(20);
                    }

                    if (e.Key == Key.D3 || e.Key == Key.NumPad3)
                    {
                        StartCards(25);
                    }

                    if (e.Key == Key.D4 || e.Key == Key.NumPad4)
                    {
                        StartCards(50);
                    }

                    if (e.Key == Key.D5 || e.Key == Key.NumPad5)
                    {
                        StartCards(75);
                    }

                    if (e.Key == Key.D6 || e.Key == Key.NumPad6)
                    {
                        StartCards(100);
                    }
                }
            }

            if (activeMenu == G_Menu_Library)
            {
                if (e.Key == Key.Enter)
                {
                    LibrarySearch();
                }
            }
        }

        private void G_Menu_Main_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetActiveMenu(G_Menu_Main);

            G_Start.Visibility = Visibility.Visible;
            G_Start_Count.Visibility = Visibility.Collapsed;
        }

        private void G_Menu_Library_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetActiveMenu(G_Menu_Library);
        }

        private void G_Menu_Settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetActiveMenu(G_Menu_Settings);
        }

        #endregion

        #region Main

        private void G_Start_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            G_Start_Count.Visibility = Visibility.Visible;
            G_Start.Visibility = Visibility.Collapsed;
        }

        private void G_Start_Count_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var textBlock = GetCountBlock(sender);
            var wordCount = Convert.ToInt32(textBlock.Text);

            StartCards(wordCount);
        }

        #endregion

        #region Library

        private void G_Library_Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LibrarySearch();
        }

        private void DeleteWord(object sender, MouseButtonEventArgs e)
        {
            var id = Convert.ToInt32((sender as Grid).Tag.ToString());

            var word = LanguageWords.First(x => x.Id == id);

            var deleteWindow = new DeleteWordWindow(word);

            SetEnabledWindow(false);

            if (deleteWindow.ShowDialog() == true)
            {
                CommandHelper.GetCommand<IDeleteLanguageWordCommand>().Execute(id);

                LibrarySearch();
            }

            SetEnabledWindow(true);
        }

        private void AddWord(object sender, MouseButtonEventArgs e)
        {
            var newWord = CommandHelper.GetCommand<IGetEditLanguageWordCommand>().Execute(null);

            var editWindow = new AddOrUpdateWordWindow(newWord);

            SetEnabledWindow(false);

            if (editWindow.ShowDialog() == true && DG_Word.ItemsSource != null)
            {
                LibrarySearch();
            }

            SetEnabledWindow(true);
        }

        private void EditWord(object sender, MouseButtonEventArgs e)
        {
            var id = Convert.ToInt32((sender as Image).Tag.ToString());

            var editWord = CommandHelper.GetCommand<IGetEditLanguageWordCommand>().Execute(id);

            var editWindow = new AddOrUpdateWordWindow(editWord);

            SetEnabledWindow(false);

            if (editWindow.ShowDialog() == true)
            {
                LibrarySearch();
            }

            SetEnabledWindow(true);
        }

        #endregion

        #region Settings

        #region General

        private void ChBx_Settings_WordCard_Timer_Checked(object sender, RoutedEventArgs e)
        {
            G_Settings_WordCard_Timer_Dutation.Visibility = Visibility.Visible;
        }

        private void ChBx_Settings_WordCard_Timer_Unchecked(object sender, RoutedEventArgs e)
        {
            G_Settings_WordCard_Timer_Dutation.Visibility = Visibility.Collapsed;
        }

        private void G_Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            G_Settings.Focus();
        }

        private void G_Settings_General_Save_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var isValid = validationManager.Execute(SettingsSave);

            if (isValid)
            {
                ShowSuccessSave(G_Settings_Save_Message);
            }            
        }

        private void TBx_Settings_WordCard_Timer_Duration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val) && e.Text != "-")
            {
                e.Handled = true; // отклоняем ввод
            }
        }

        private void TBx_Settings_WordCard_Timer_Duration_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }

        #endregion

        #region Languages

        private void G_Settings_Languages_Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LanguageSearch();
        }

        private void DeleteLanguage(object sender, MouseButtonEventArgs e)
        {
            var id = Convert.ToInt32((sender as Grid).Tag.ToString());

            var language = Languages.First(x => x.Id == id);

            var deleteWindow = new DeleteLanguageWindow(language);

            SetEnabledWindow(false);

            if (deleteWindow.ShowDialog() == true)
            {
                ReloadSettings();

                LanguageSearch();
            }

            SetEnabledWindow(true);
        }

        private void AddLanguage(object sender, MouseButtonEventArgs e)
        {
            var editWindow = new AddOrUpdateLanguageWindow(new Language());

            SetEnabledWindow(false);

            if (editWindow.ShowDialog() == true) 
            {
                ReloadSettings();

                if (DG_Language.ItemsSource != null)
                {
                    LanguageSearch();
                }

                ShowSuccessSave(G_Settings_Language_Save_Message);
            }

            SetEnabledWindow(true);
        }

        private void EditLanguage(object sender, MouseButtonEventArgs e)
        {
            var id = Convert.ToInt32((sender as Image).Tag.ToString());

            var editLanguage = CommandHelper.GetCommand<IGetLanguageCommand>().Execute(id);

            var editWindow = new AddOrUpdateLanguageWindow(editLanguage);

            SetEnabledWindow(false);

            if (editWindow.ShowDialog() == true)
            {
                ReloadSettings();

                LanguageSearch();

                ShowSuccessSave(G_Settings_Language_Save_Message);
            }

            SetEnabledWindow(true);
        }

        #endregion

        #endregion

        #endregion

        #region General

        private void SetActiveMenu(Grid grid)
        {
            activeMenu = grid;

            if (grid != G_Menu_Library)
            {
                ClearLibrarySource();
            }

            foreach (var item in menu)
            {
                var actionType = item.Key == activeMenu ? BackgroundColor.ActionType.Active : BackgroundColor.ActionType.Default;
                var visibility = item.Key == activeMenu ? Visibility.Visible : Visibility.Collapsed;

                item.Value.Visibility = visibility;
                BackgroundColor.SetMenuColor(item.Key, Resources, actionType);
            }
        }

        private void StartHoverAnimation(object sender, bool isHover)
        {
            var grid = sender as Grid;

            var actionType = GetColorActionType(grid);

            for (int i = 0; i < grid.Children.Count; i++)
            {
                var item = grid.Children[i];

                if (item is TextBlock)
                {
                    BackgroundColor.BeginAnimation((item as TextBlock), Resources, isHover, actionType);
                }

                if (item is Grid)
                {
                    var backgroundGrid = item as Grid;

                    for (int k = 0; k < backgroundGrid.Children.Count; k++)
                    {
                        var backgroundItem = backgroundGrid.Children[k] as Shape;

                        BackgroundColor.BeginAnimation(backgroundItem, Resources, isHover, actionType);
                    }
                }
            }
        }

        private BackgroundColor.ActionType GetColorActionType(Grid grid)
        {
            if (grid == activeMenu)
            {
                return BackgroundColor.ActionType.Active;
            }

            return BackgroundColor.ActionType.Default;
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

        #endregion

        #region Main

        private TextBlock? GetCountBlock(object sender)
        {
            var grid = sender as Grid;

            for (int i = 0; i < grid.Children.Count; i++)
            {
                if (grid.Children[i] is TextBlock)
                {
                    return grid.Children[i] as TextBlock;
                }
            }

            return null;
        }

        private void StartCards(int wordCount)
        {
            G_Start.Visibility = Visibility.Visible;
            G_Start_Count.Visibility = Visibility.Collapsed;

            IsRunCards = true;

            var cardsWindow = new CardsWindow(wordCount);

            cardsWindow.Owner = this;

            cardsWindow.Show();

            Hide();
        }

        #endregion

        #region Library

        private void ClearLibrarySource()
        {
            LanguageWords?.Clear();
            TBx_Library_Search.Clear();
            TB_Library_Search_Result.Visibility = Visibility.Collapsed;
            G_Library_NoResult.Visibility = Visibility.Collapsed;
        }

        private void LibrarySearch()
        {
            LanguageWords = CommandHelper.GetCommand<IGetLanguageWordsCommand>().Execute(TBx_Library_Search.Text, ChBx_Library_Search_WithoutTranscription.IsChecked ?? false, CbBx_Library_Search_WordStatus.SelectedItem as WordStatus);

            DG_Word.ItemsSource = LanguageWords;

            if (LanguageWords.Count == 0)
            {
                TB_Library_Search_Result.Visibility = Visibility.Collapsed;
                G_Library_NoResult.Visibility = Visibility.Visible;
            }
            else
            {
                TB_Library_Search_ResultCount.Text = LanguageWords.Count.ToString();
                TB_Library_Search_Result.Visibility = Visibility.Visible;
                G_Library_NoResult.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Settings

        private void SettingsSave()
        {
            G_Settings.Focus();

            CommandHelper.GetCommand<ISaveSettingsCommand>().Execute(settings);
        }

        private static async void ShowSuccessSave(UIElement element)
        {
            await ShowSuccessSaveAsync(element);
        }

        private static async Task ShowSuccessSaveAsync(UIElement element)
        {
            element.Visibility = Visibility.Visible;

            await Task.Delay(2000);

            element.Visibility = Visibility.Collapsed;
        }

        private void LanguageSearch()
        {
            Languages = CommandHelper.GetCommand<IGetLanguagesCommand>().Execute();

            DG_Language.ItemsSource = Languages;

            if (Languages.Count == 0)
            {
                WP_Settings_Languages_Search_Result.Visibility = Visibility.Collapsed;
                G_Settings_Languages_NoResult.Visibility = Visibility.Visible;
            }
            else
            {
                TB_Settings_Languages_Search_ResultCount.Text = Languages.Count.ToString();
                WP_Settings_Languages_Search_Result.Visibility = Visibility.Visible;
                G_Settings_Languages_NoResult.Visibility = Visibility.Collapsed;
            }
        }

        private void ReloadSettings()
        {
            settings = CommandHelper.GetCommand<IGetSettingsCommand>().Execute();

            DataContext = settings;
        }

        #endregion        
    }
}
