﻿using System;
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
    }
}