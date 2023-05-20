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
        private Grid activeMenu;

        private readonly Dictionary<Grid, Grid> menu;

        private ObservableCollection<LanguageWord> LanguageWords;

        private static Color errorColor = Color.FromRgb(108, 36, 33); // #6c2421

        private Settings settings;
        private ValidationManager validationManager;

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
        }

        public bool IsRunCards { get; set; }

        #region Events

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

        private void G_Library_Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Search();
        }

        #endregion

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

        private void SetActiveMenu(Grid grid)
        {
            activeMenu = grid;

            foreach(var item in menu)
            {
                var typeColor = item.Key == activeMenu ? BackgroundColor.ColorType.Active : BackgroundColor.ColorType.Default;
                var visibility = item.Key == activeMenu ? Visibility.Visible : Visibility.Collapsed;

                item.Value.Visibility = visibility;
                BackgroundColor.SetMenuColor(item.Key, typeColor);
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
                    BackgroundColor.BeginAnimation(item, isHover, actionType, (item as TextBlock).Foreground.BeginAnimation);
                }

                if (item is Grid)
                {
                    var backgroundGrid = item as Grid;

                    for (int k = 0; k < backgroundGrid.Children.Count; k++)
                    {
                        var backgroundItem = backgroundGrid.Children[k];

                        BackgroundColor.BeginAnimation(backgroundItem, isHover, actionType, (backgroundItem as Shape).Fill.BeginAnimation);
                    }
                }
            }
        }

        private BackgroundColor.ActionType GetColorActionType(Grid grid)
        {
            if(grid == activeMenu)
            {
                return BackgroundColor.ActionType.Active;
            }

            if(grid.Tag != null && grid.Tag is string)
            {
                if(BackgroundColor.ActionByTags.ContainsKey(grid.Tag.ToString()))
                {
                    return BackgroundColor.ActionByTags[grid.Tag.ToString()];
                }
            }

            return BackgroundColor.ActionType.Default;
        }

        private void Search()
        {
            LanguageWords = CommandHelper.GetCommand<IGetLanguageWordsCommand>().Execute(TBx_Library_Search.Text, ChBx_Library_Search_WithoutTranscription.IsChecked ?? false);

            LBx_Word.ItemsSource = LanguageWords;

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

        private void DeleteWord(object sender, MouseButtonEventArgs e)
        {
            var id = Convert.ToInt32((sender as Grid).Tag.ToString());

            var word = LanguageWords.First(x => x.Id == id);

            var deleteWindow = new DeleteWordWindow(word);

            SetEnabledWindow(false);

            if(deleteWindow.ShowDialog() == true)
            {
                CommandHelper.GetCommand<IDeleteLanguageWordCommand>().Execute(id);

                Search();
            }

            SetEnabledWindow(true);
        }

        private void AddWord(object sender, MouseButtonEventArgs e)
        {
            var newWord = CommandHelper.GetCommand<IGetEditLanguageWordCommand>().Execute(null);

            var editWindow = new AddOrUpdateWordWindow(newWord);

            SetEnabledWindow(false);

            if (editWindow.ShowDialog() == true && LBx_Word.ItemsSource != null)
            {
                Search();
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
                Search();
            }

            SetEnabledWindow(true);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(activeMenu == G_Menu_Main)
            {
                if(G_Start.Visibility == Visibility.Visible && e.Key == Key.Enter)
                {
                    G_Start_MouseLeftButtonDown(sender, e);
                }

                if(G_Start_Count.Visibility == Visibility.Visible && !IsRunCards)
                {
                    if(e.Key == Key.D1 || e.Key == Key.NumPad1)
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

            if(activeMenu == G_Menu_Library)
            {
                if(e.Key == Key.Enter)
                {
                    Search();
                }
            }
        }

        private void SetEnabledWindow(bool isEnabled)
        {
            if(isEnabled)
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
