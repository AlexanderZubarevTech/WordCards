using CardWords.Configurations;
using CardWords.Views.Cards;
using System;
using System.Collections.Generic;
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
            public enum TypeColor
            {
                Defalut,
                Line,
                Text,
                Active,
                ActiveText,
            }

            private static Color defaultColor = Color.FromRgb(93, 167, 168); // #5da7a8
            private static Color defaultLineColor = Color.FromRgb(108, 195, 196); // #6cc3c4
            private static Color defaultTextColor = Color.FromRgb(238, 238, 238); // #DDD
            private static Color defaultActiveMenuColor = Color.FromRgb(77, 39, 139); //#4d278b
            private static Color defaultActiveTextMenuColor = Color.FromRgb(204, 204, 204); //#CCC

            private static Color hoverColor = Color.FromRgb(108, 195, 196); // #6cc3c4
            private static Color hoverLineColor = Color.FromRgb(140, 211, 212); //#8cd3d4
            private static Color hoverTextColor = Color.FromRgb(255, 255, 255); // #FFF
            private static Color hoverActiveMenuColor = Color.FromRgb(93, 55, 155); //#5d379b
            private static Color hoverActiveTextMenuColor = Color.FromRgb(238, 238, 238); //#DDD

            private static readonly Dictionary<bool, Dictionary<TypeColor, Color>> colors = new ()
            {
                {false, new Dictionary<TypeColor, Color>()
                    {
                        {TypeColor.Defalut, defaultColor},
                        {TypeColor.Line, defaultLineColor},
                        {TypeColor.Text, defaultTextColor},
                        {TypeColor.Active, defaultActiveMenuColor},
                        {TypeColor.ActiveText, defaultActiveTextMenuColor},
                    }
                },
                {true, new Dictionary<TypeColor, Color>()
                    {
                        {TypeColor.Defalut, hoverColor},
                        {TypeColor.Line, hoverLineColor},
                        {TypeColor.Text, hoverTextColor},
                        {TypeColor.Active, hoverActiveMenuColor},
                        {TypeColor.ActiveText, hoverActiveTextMenuColor},
                    }
                }
            };

            private static readonly Dictionary<TypeColor, TypeColor> backgroundWithTextColors = new()
            {
                {TypeColor.Defalut, TypeColor.Text},
                {TypeColor.Active, TypeColor.ActiveText},
            };

            public static void BeginAnimation(Shape item, bool isHover, bool isActive)
            {
                var typeColor = isActive ? TypeColor.Active : GetTypeColor(item);

                var animation = GetColorAnimation(isHover, typeColor);

                item.Fill.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }

            public static void BeginAnimation(TextBlock item, bool isHover, bool isActive)
            {
                var typeColor = isActive ? TypeColor.ActiveText : GetTypeColor(item);

                var animation = GetColorAnimation(isHover, typeColor);

                item.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }

            public static void SetMenuColor(Grid grid, TypeColor typeColor)
            {
                var textTypeColor = backgroundWithTextColors[typeColor];

                var isHover = typeColor == TypeColor.Active;

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

            private static TypeColor GetTypeColor(UIElement item)
            {
                if(item is Rectangle)
                {
                    return TypeColor.Defalut;
                }

                if(item is Polygon)
                {
                    return TypeColor.Line;
                }

                if(item is TextBlock)
                {
                    return TypeColor.Text;
                }

                return TypeColor.Defalut;
            }

            private static ColorAnimation GetColorAnimation(bool isHover, TypeColor typeColor)
            {
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

        private readonly List<Grid> menu;

        public MainWindow()
        {
            InitializeComponent();

            mainGrid.Visibility = Visibility.Visible;
            G_Start.Visibility = Visibility.Visible;
            startGrid.Visibility = Visibility.Collapsed;

            BackgroundColor.SetMenuColor(G_Menu_Main, BackgroundColor.TypeColor.Active);
            BackgroundColor.SetMenuColor(G_Menu_Library, BackgroundColor.TypeColor.Defalut);
            BackgroundColor.SetMenuColor(G_Menu_Settings, BackgroundColor.TypeColor.Defalut);

            menu = new List<Grid>
            {
                G_Menu_Main,
                G_Menu_Library,
                G_Menu_Settings
            };

            activeMenu = G_Menu_Main;
        }

        #region Events

        private void G_Start_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            startGrid.Visibility = Visibility.Visible;
            G_Start.Visibility = Visibility.Collapsed;
        }

        private void G_Start_Count_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            G_Start.Visibility = Visibility.Visible;
            startGrid.Visibility = Visibility.Collapsed;

            var textBlock = GetCountBlock(sender);

            var wordCount = Convert.ToInt32(textBlock.Text);

            var cardsWindow = new CardsWindow(wordCount);

            cardsWindow.Owner = this;

            cardsWindow.Show();

            Hide();
        }

        private void G_Menu_Main_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainGrid.Visibility = Visibility.Visible;
            G_Start.Visibility = Visibility.Visible;
            startGrid.Visibility = Visibility.Collapsed;

            SetActiveMenu(G_Menu_Main);
        }

        private void G_Menu_Library_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainGrid.Visibility = Visibility.Collapsed;

            SetActiveMenu(G_Menu_Library);
        }

        private void G_Menu_Settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainGrid.Visibility = Visibility.Collapsed;

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

        private void G_Start_MouseEnter(object sender, MouseEventArgs e)
        {
            StartHoverAnimation(sender, true);
        }

        private void G_Start_MouseLeave(object sender, MouseEventArgs e)
        {
            StartHoverAnimation(sender, false);
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
                var typeColor = item == grid ? BackgroundColor.TypeColor.Active : BackgroundColor.TypeColor.Defalut;
                BackgroundColor.SetMenuColor(item, typeColor);
            }
        }

        private void StartHoverAnimation(object sender, bool isHover)
        {
            var grid = sender as Grid;

            var isActiveMenu = grid == activeMenu;            

            for (int i = 0; i < grid.Children.Count; i++)
            {
                if (grid.Children[i] is TextBlock)
                {
                    BackgroundColor.BeginAnimation(grid.Children[i] as TextBlock, isHover, isActiveMenu);
                }

                if (grid.Children[i] is Grid)
                {
                    var backgroundGrid = grid.Children[i] as Grid;

                    for (int k = 0; k < backgroundGrid.Children.Count; k++)
                    {
                        var item = backgroundGrid.Children[k];

                        BackgroundColor.BeginAnimation(item as Shape, isHover, isActiveMenu);
                    }
                }
            }
        }
    }
}
