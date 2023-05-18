using CardWords.Business.LanguageWords;
using CardWords.Configurations;
using CardWords.Core.Helpers;
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
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CardWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static class BackgroundColor
        {
            public enum ActionType
            {
                Default,
                Active,
                Delete
            }

            public enum ColorType
            {
                Default,
                Line,
                Text,
                Active,
                ActiveText,
                Delete,
                DeleteText
            }

            private static Color defaultColor = Color.FromRgb(93, 167, 168); // #5da7a8
            private static Color defaultLineColor = Color.FromRgb(108, 195, 196); // #6cc3c4
            private static Color defaultTextColor = Color.FromRgb(238, 238, 238); // #DDD
            private static Color defaultActiveMenuColor = Color.FromRgb(77, 39, 139); //#4d278b
            private static Color defaultActiveTextMenuColor = Color.FromRgb(204, 204, 204); //#CCC
            private static Color defaultDeleteColor = Color.FromArgb(0, 255, 255, 255);
            private static Color defaultDeleteTextColor = Color.FromRgb(170, 0, 0); //#A00

            private static Color hoverColor = Color.FromRgb(108, 195, 196); // #6cc3c4
            private static Color hoverLineColor = Color.FromRgb(140, 211, 212); //#8cd3d4
            private static Color hoverTextColor = Color.FromRgb(255, 255, 255); // #FFF
            private static Color hoverActiveMenuColor = Color.FromRgb(93, 55, 155); //#5d379b
            private static Color hoverActiveTextMenuColor = Color.FromRgb(238, 238, 238); //#DDD
            private static Color hoverDeleteColor = Color.FromArgb(255, 170, 0, 0); //#A00
            private static Color hoverDeleteTextColor = Color.FromRgb(255, 255, 255);

            private static readonly Dictionary<bool, Dictionary<ColorType, Color>> colors = new ()
            {
                {false, new Dictionary<ColorType, Color>()
                    {
                        {ColorType.Default, defaultColor},
                        {ColorType.Line, defaultLineColor},
                        {ColorType.Text, defaultTextColor},
                        {ColorType.Active, defaultActiveMenuColor},
                        {ColorType.ActiveText, defaultActiveTextMenuColor},
                        {ColorType.Delete, defaultDeleteColor},
                        {ColorType.DeleteText, defaultDeleteTextColor},
                    }
                },
                {true, new Dictionary<ColorType, Color>()
                    {
                        {ColorType.Default, hoverColor},
                        {ColorType.Line, hoverLineColor},
                        {ColorType.Text, hoverTextColor},
                        {ColorType.Active, hoverActiveMenuColor},
                        {ColorType.ActiveText, hoverActiveTextMenuColor},
                        {ColorType.Delete, hoverDeleteColor},
                        {ColorType.DeleteText, hoverDeleteTextColor},
                    }
                }
            };

            private static readonly Dictionary<ActionType, ColorType> colorByActions = new()
            {
                {ActionType.Default, ColorType.Default},
                {ActionType.Active, ColorType.Active},
                {ActionType.Delete, ColorType.Delete},
            };

            private static readonly Dictionary<ColorType, ColorType> backgroundWithTextColors = new()
            {
                {ColorType.Default, ColorType.Text},
                {ColorType.Active, ColorType.ActiveText},
                {ColorType.Delete, ColorType.DeleteText},
            };

            public static readonly Dictionary<string, ActionType> ActionByTags = new()
            {
                {"delete", ActionType.Delete},
            };

            public static void BeginAnimation(UIElement item, bool isHover, ActionType actionType, Action<DependencyProperty, AnimationTimeline> beginAnimation)
            {
                var animation = GetColorAnimation(item, isHover, actionType);                

                beginAnimation.Invoke(SolidColorBrush.ColorProperty, animation);
            }

            public static void BeginAnimation(Shape item, bool isHover, ActionType actionType)
            {
                var animation = GetColorAnimation(item, isHover, actionType);

                item.Fill.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }

            public static void BeginAnimation(TextBlock item, bool isHover, ActionType actionType)
            {
                var animation = GetColorAnimation(item, isHover, actionType);

                item.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }

            public static void SetMenuColor(Grid grid, ColorType typeColor)
            {
                var textTypeColor = backgroundWithTextColors[typeColor];

                var isHover = typeColor == ColorType.Active;

                for (int i = 0; i < grid.Children.Count; i++)
                {
                    var item = grid.Children[i];

                    if (item is TextBlock)
                    {
                        (item as TextBlock).Foreground = new SolidColorBrush(colors[isHover][textTypeColor]);
                    }

                    if (item is Grid)
                    {
                        ((item as Grid).Children[0] as Rectangle).Fill = new SolidColorBrush(colors[isHover][typeColor]);
                    }
                }
            }

            private static ColorType GetTypeColor(UIElement item, ActionType actionType)
            {
                if(item is Rectangle)
                {
                    return colorByActions[actionType];
                }

                if(item is Polygon)
                {
                    return ColorType.Line;
                }

                if(item is TextBlock)
                {
                    return backgroundWithTextColors[colorByActions[actionType]];
                }

                return ColorType.Default;
            }

            private static ColorAnimation GetColorAnimation(UIElement item, bool isHover, ActionType actionType)
            {
                var typeColor = GetTypeColor(item, actionType);

                var animation = new ColorAnimation();

                animation.AutoReverse = false;
                animation.Duration = TimeSpan.FromSeconds(isHover ? 0.5 : 0.2);
                animation.From = colors[!isHover][typeColor];
                animation.To = colors[isHover][typeColor];


                return animation;
            }            
        }

        public static readonly AppConfiguration AppConfiguration = AppConfiguration.GetInstance();

        private Grid activeMenu;

        private readonly Dictionary<Grid, Grid> menu;

        private ObservableCollection<LanguageWord> LanguageWords;

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
            LanguageWords = CommandHelper.GetCommand<IGetLanguageWordsCommand>().Execute(TBx_Library_Search.Text, CB_Library_Search_WithoutTranscription.IsChecked ?? false);

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
