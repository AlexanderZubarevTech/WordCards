using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CardWords
{
    public partial class MainWindow
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

            private static Color defaultBacgroundColor = Color.FromRgb(233, 233, 233); // #e9e9e9

            private static Color defaultColor = Color.FromRgb(0, 121, 169); // #0079a9
            private static Color defaultLineColor = Color.FromRgb(0, 151, 211); // #0097d3
            private static Color defaultTextColor = Color.FromRgb(238, 238, 238); // #DDD

            private static Color defaultActiveMenuColor = Color.FromRgb(78, 49, 130); //#4e3182
            private static Color defaultActiveLineColor = Color.FromRgb(106, 68, 174); //#6a44ae
            private static Color defaultActiveTextMenuColor = Color.FromRgb(204, 204, 204); //#CCC

            private static Color hoverColor = Color.FromRgb(0, 151, 211); // #0097d3
            private static Color hoverLineColor = Color.FromRgb(41, 186, 244); // #29baf4
            private static Color hoverTextColor = Color.FromRgb(255, 255, 255); // #FFF

            private static Color hoverActiveMenuColor = Color.FromRgb(106, 68, 174); //#6a44ae
            private static Color hoverActiveLineColor = Color.FromRgb(132, 83, 220); //#8453dc
            private static Color hoverActiveTextMenuColor = Color.FromRgb(221, 221, 221); //#DDD

            private static Color correctColor = Color.FromRgb(3, 132, 36); // #038424
            private static Color correctLineColor = Color.FromRgb(53, 162, 81); //#35a251
            private static Color correctTextColor = Color.FromRgb(221, 221, 221); // #DDD

            private static Color wrongtColor = Color.FromRgb(108, 36, 33); // #6c2421
            private static Color wrongLineColor = Color.FromRgb(134, 58, 55); // #863a37
            private static Color wrongTextColor = Color.FromRgb(221, 221, 221); // #DDD

            private static Color newWordColor = Color.FromRgb(78, 49, 130); // #4e3182
            private static Color newWordLineColor = Color.FromRgb(106, 68, 174); // #6a44ae
            private static Color newWordTextColor = Color.FromRgb(221, 221, 221); // #DDD

            private static Color wordResultColor = Color.FromRgb(0, 136, 189); // #0088bd
            private static Color wordResultBacgroundColor = Color.FromRgb(0, 101, 141); // #00658d
            private static Color wordResultTextColor = Color.FromRgb(221, 221, 221); // #DDD

            private static Color validationErrorFieldColor = Color.FromRgb(108, 36, 33); // #6c2421
            private static Color validationErrorFieldTextColor = Color.FromRgb(238, 238, 238); // #EEE

            private static Color validationErrorTextColor = Color.FromRgb(108, 36, 33); // #6c2421

            private static readonly Dictionary<bool, Dictionary<ColorType, Color>> colors = new ()
            {
                {false, new Dictionary<ColorType, Color>()
                    {
                        {ColorType.Default, defaultColor},
                        {ColorType.Line, defaultLineColor},
                        {ColorType.Text, defaultTextColor},
                        {ColorType.Active, defaultActiveMenuColor},
                        {ColorType.ActiveText, defaultActiveTextMenuColor},                        
                    }
                },
                {true, new Dictionary<ColorType, Color>()
                    {
                        {ColorType.Default, hoverColor},
                        {ColorType.Line, hoverLineColor},
                        {ColorType.Text, hoverTextColor},
                        {ColorType.Active, hoverActiveMenuColor},
                        {ColorType.ActiveText, hoverActiveTextMenuColor},                        
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
    }
}
