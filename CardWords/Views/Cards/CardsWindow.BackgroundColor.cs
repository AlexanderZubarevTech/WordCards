using System.Windows.Media;
using System.Windows.Shapes;

namespace CardWords.Views.Cards
{
    public partial class CardsWindow
    {
        private static class BackgroundColor
        {
            public enum ColorType
            {
                Default = 1,
                Correct = 2,
                Wrong = 3,
                NewWord = 4
            }

            private static Color defaultColor = Color.FromRgb(93, 167, 168); // #5da7a8
            private static Color defaultLineColor = Color.FromRgb(108, 195, 196); // #6cc3c4
            private static Color correctColor = Color.FromRgb(3, 132, 36); // #038424
            private static Color correctLineColor = Color.FromRgb(53, 162, 81); //#35a251
            private static Color wrongColor = Color.FromRgb(108, 36, 33); // #6c2421
            private static Color wrongLineColor = Color.FromRgb(134, 58, 55); // #863a37
            private static Color newWordColor = Color.FromRgb(77, 39, 139);
            private static Color newWordLineColor = Color.FromRgb(98, 44, 185);

            public static void SetColor(Rectangle rectangle, ColorType type)
            {
                rectangle.Fill = GetBrush(type);
            }

            public static void SetLineColor(Polygon polygon, ColorType type)
            {
                polygon.Fill = GetBrush(type, true);
            }

            private static Brush GetBrush(ColorType type, bool isLine = false)
            {
                var color = GetColor(type, isLine);

                return new SolidColorBrush(color);
            }

            private static Color GetColor(ColorType type, bool isLine)
            {
                switch (type)
                {
                    case ColorType.Default:
                        {
                            return isLine ? defaultLineColor : defaultColor;
                        }
                    case ColorType.Correct:
                        {
                            return isLine ? correctLineColor : correctColor;
                        }
                    case ColorType.Wrong:
                        {
                            return isLine ? wrongLineColor : wrongColor;
                        }
                    case ColorType.NewWord:
                        {
                            return isLine ? newWordLineColor : newWordColor;
                        }
                    default:
                        {
                            return Color.FromRgb(0, 0, 0);
                        }
                }
            }
        }
    }
}
