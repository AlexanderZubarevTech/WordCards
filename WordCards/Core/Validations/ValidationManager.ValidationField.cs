using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WordCards.Core.Validations
{
    public sealed partial class ValidationManager
    {
        public sealed class ValidationField
        {
            public ValidationField(string fieldPropertyName, string fieldName, FrameworkElement fieldValueElement)
            {
                FieldPropertyName = fieldPropertyName;
                FieldName = fieldName;
                FieldValueElement = fieldValueElement;
            }

            public ValidationField(string fieldPropertyName, string fieldName, FrameworkElement fieldValueElement, FrameworkElement? backgroundElement, Color? errorBackgroundColor)
            {
                FieldPropertyName = fieldPropertyName;
                FieldName = fieldName;
                FieldValueElement = fieldValueElement;
                BackgroundElement = backgroundElement;
                DefaultColor = GetDefaultColor(backgroundElement);
                ErrorBackgroundColor = errorBackgroundColor;
            }

            public string FieldPropertyName { get; set; }

            public string FieldName { get; set; }

            public FrameworkElement FieldValueElement { get; set; }

            public FrameworkElement? BackgroundElement { get; set; }

            public Color? DefaultColor { get; set; }

            public Color? ErrorBackgroundColor { get; set; }

            private static Color? GetDefaultColor(FrameworkElement? backgroundElement)
            {
                if (backgroundElement == null)
                {
                    return null;
                }

                if (backgroundElement is Shape shape && shape.Fill != null && shape.Fill is SolidColorBrush fillBrush)
                {
                    return fillBrush.Color;
                }

                if (backgroundElement is Panel panel && panel.Background != null && panel.Background is SolidColorBrush backgroundBrush)
                {
                    return backgroundBrush.Color;
                }

                return null;
            }

            public void SetError()
            {
                SetColor(ErrorBackgroundColor);
            }

            public void SetDefault()
            {
                SetColor(DefaultColor);
            }

            private void SetColor(Color? color)
            {
                if (BackgroundElement == null || color == null)
                {
                    return;
                }

                if (BackgroundElement is Shape shape)
                {
                    shape.Fill = new SolidColorBrush(color.Value);
                }

                if (BackgroundElement is Panel panel)
                {
                    panel.Background = new SolidColorBrush(color.Value);
                }
            }
        }


    }
}
