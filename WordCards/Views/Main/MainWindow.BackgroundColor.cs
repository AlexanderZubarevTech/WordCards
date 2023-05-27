using WordCards.Core.Tags;
using WordCards.Views.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WordCards
{
    public partial class MainWindow
    {
        private static class BackgroundColor
        {
            private const string tag = "BackgroundColor.ActionType";

            public enum ActionType
            {
                Default,
                Active,
            }

            private static readonly Dictionary<string, ActionType> actionNames = new()
            {
                {ActionType.Default.ToString(), ActionType.Default},
                {ActionType.Active.ToString(), ActionType.Active}
            };

            private static readonly Dictionary<ActionType, ThemeColor.ColorType> colorByActions = new()
            {
                {ActionType.Default, ThemeColor.ColorType.Default},
                {ActionType.Active, ThemeColor.ColorType.Active}
            };


            public static void BeginAnimation(Shape item, ResourceDictionary resources, bool isHover, ActionType actionType)
            {
                ReplaceBrush(item);

                BeginAnimation(item, resources, isHover, actionType, item.Fill.BeginAnimation);
            }

            public static void BeginAnimation(TextBlock item, ResourceDictionary resources, bool isHover, ActionType actionType)
            {
                ReplaceBrush(item);

                BeginAnimation(item, resources, isHover, actionType, item.Foreground.BeginAnimation);                
            }

            public static void BeginAnimation(FrameworkElement item, ResourceDictionary resources, bool isHover,
                ActionType actionType, Action<DependencyProperty, AnimationTimeline> beginAnimation)
            {
                var animation = GetColorAnimation(item, resources, isHover, actionType);

                beginAnimation.Invoke(SolidColorBrush.ColorProperty, animation);
            }

            private static void ReplaceBrush(Shape shape)
            {
                shape.Fill = new SolidColorBrush((shape.Fill as SolidColorBrush).Color);
            }

            private static void ReplaceBrush(TextBlock textBlock)
            {
                textBlock.Foreground = new SolidColorBrush((textBlock.Foreground as SolidColorBrush).Color);
            }

            private static ThemeColor.ElementType GetElementType(UIElement item)
            {
                if(item is Rectangle)
                {
                    return ThemeColor.ElementType.Default;
                }

                if (item is Polygon)
                {
                    return ThemeColor.ElementType.Line;
                }

                if (item is TextBlock)
                {
                    return ThemeColor.ElementType.Text;
                }

                return ThemeColor.ElementType.Default;
            }            

            private static ColorAnimation GetColorAnimation(FrameworkElement item, ResourceDictionary resources, bool isHover, ActionType actionType)
            {
                if(item.Tag != null)
                {
                    var tags = ElementTag.ParseTag(item.Tag);

                    if (tags.ContainsKey(tag))
                    {
                        actionType = actionNames[tags[tag].Value];
                    }
                }

                var elementType = GetElementType(item);

                var typeColor = colorByActions[actionType];

                var fromHoverType = isHover ? ThemeColor.HoverType.None : ThemeColor.HoverType.Hover;
                var toHoverType = isHover ? ThemeColor.HoverType.Hover : ThemeColor.HoverType.None;

                var fromColor = ThemeColor.GetColor(resources, typeColor, elementType, fromHoverType);
                var toColor = ThemeColor.GetColor(resources, typeColor, elementType, toHoverType);

                var animation = new ColorAnimation();

                animation.AutoReverse = false;
                animation.Duration = TimeSpan.FromSeconds(isHover ? 0.4 : 0.2);
                animation.From = fromColor;
                animation.To = toColor;


                return animation;
            }

            public static void SetMenuColor(Grid grid, ResourceDictionary resources, ActionType actionType)
            {
                var isHover = actionType == ActionType.Active;
                
                var hoverType = isHover ? ThemeColor.HoverType.Hover : ThemeColor.HoverType.None;

                for (int i = 0; i < grid.Children.Count; i++)
                {
                    var item = grid.Children[i];

                    if (item is TextBlock)
                    {
                        (item as TextBlock).Foreground = ThemeColor.GetBrush(resources, colorByActions[actionType], ThemeColor.ElementType.Text, hoverType);
                    }

                    if (item is Grid)
                    {
                        ((item as Grid).Children[0] as Rectangle).Fill = ThemeColor.GetBrush(resources, colorByActions[actionType], ThemeColor.ElementType.Default, hoverType);
                    }
                }
            }
        }
    }
}
