using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WordCards.Views.Common
{
    public static class ThemeColor
    {
        public enum ColorType
        {
            Default = 1,
            NotEnabledButton,
            CancelButton,
            Active,
            Correct,
            Wrong,
            NewWord,
            WordResult,
            ValidationErrorField,
            ValidationError,
            LibraryHeap
        }

        public enum ElementType
        {
            Default = 1,
            Line,
            Text,
            Background
        }

        public enum ResourceType
        {
            Color = 1,
            Brush
        }

        public enum HoverType
        {
            None = 1,
            Hover
        }

        private readonly struct ColorTypeInfo
        {
            public static ColorTypeInfo Create(ColorType cType, ElementType eType, ResourceType rType, HoverType hType)
            {
                return new ColorTypeInfo(cType, eType, rType, hType);
            }

            private ColorTypeInfo(ColorType cType, ElementType eType, ResourceType rType, HoverType hType)
            {
                ColorType = cType;
                ElementType = eType;
                ResourceType = rType;
                HoverType = hType;
            }

            public ColorType ColorType { get; }

            public ElementType ElementType { get; }

            public ResourceType ResourceType { get; }

            public HoverType HoverType { get; }

            public static bool operator ==(ColorTypeInfo left, ColorTypeInfo right)
            {
                return left.ColorType == right.ColorType
                    && left.ElementType == right.ElementType
                    && left.ResourceType == right.ResourceType
                    && left.HoverType == right.HoverType;
            }

            public static bool operator !=(ColorTypeInfo left, ColorTypeInfo right)
            {
                return !(left == right);
            }

            public override bool Equals(object? obj)
            {
                if (obj == null || obj is not ColorTypeInfo)
                {
                    return false;
                }

                var pair = (ColorTypeInfo)obj;

                return this == pair;
            }

            public override int GetHashCode()
            {
                return ColorType.GetHashCode() ^ ElementType.GetHashCode() ^ ResourceType.GetHashCode();
            }

            public override string ToString()
            {
                var hoverName = HoverType == HoverType.None ? string.Empty : HoverType.ToString();

                var elementName = ElementType == ElementType.Default ? string.Empty : ElementType.ToString();

                return $"{hoverName}{ColorType}{elementName}{ResourceType}";
            }
        }

        private static readonly IReadOnlyDictionary<ColorTypeInfo, string> colors = GetColorDictionary();

        private static IReadOnlyDictionary<ColorTypeInfo, string> GetColorDictionary()
        {
            var dictionary = new Dictionary<ColorTypeInfo, string>();

            Add(dictionary, ColorType.Default, ElementType.Default);
            Add(dictionary, ColorType.Default, ElementType.Line);
            Add(dictionary, ColorType.Default, ElementType.Text);
            Add(dictionary, ColorType.Default, ElementType.Background);

            Add(dictionary, ColorType.Default, ElementType.Default, HoverType.Hover);
            Add(dictionary, ColorType.Default, ElementType.Line, HoverType.Hover);
            Add(dictionary, ColorType.Default, ElementType.Text, HoverType.Hover);

            Add(dictionary, ColorType.NotEnabledButton, ElementType.Default);
            Add(dictionary, ColorType.NotEnabledButton, ElementType.Text);

            Add(dictionary, ColorType.CancelButton, ElementType.Default);
            Add(dictionary, ColorType.CancelButton, ElementType.Text);

            Add(dictionary, ColorType.CancelButton, ElementType.Default, HoverType.Hover);

            Add(dictionary, ColorType.Active, ElementType.Default);
            Add(dictionary, ColorType.Active, ElementType.Text);

            Add(dictionary, ColorType.Active, ElementType.Default, HoverType.Hover);
            Add(dictionary, ColorType.Active, ElementType.Text, HoverType.Hover);

            Add(dictionary, ColorType.Correct, ElementType.Default);
            Add(dictionary, ColorType.Correct, ElementType.Line);
            Add(dictionary, ColorType.Correct, ElementType.Text);

            Add(dictionary, ColorType.Wrong, ElementType.Default);
            Add(dictionary, ColorType.Wrong, ElementType.Line);
            Add(dictionary, ColorType.Wrong, ElementType.Text);

            Add(dictionary, ColorType.NewWord, ElementType.Default);
            Add(dictionary, ColorType.NewWord, ElementType.Line);
            Add(dictionary, ColorType.NewWord, ElementType.Text);

            Add(dictionary, ColorType.WordResult, ElementType.Default);
            Add(dictionary, ColorType.WordResult, ElementType.Background);

            Add(dictionary, ColorType.ValidationErrorField, ElementType.Default);

            Add(dictionary, ColorType.ValidationError, ElementType.Text);

            Add(dictionary, ColorType.LibraryHeap, ElementType.Default);

            return dictionary;
        }

        private static void Add(Dictionary<ColorTypeInfo, string> dictionary, ColorType colorType, ElementType elementType, HoverType hoverType = HoverType.None)
        {
            var colorKey = ColorTypeInfo.Create(colorType, elementType, ResourceType.Color, hoverType);
            var brushKey = ColorTypeInfo.Create(colorType, elementType, ResourceType.Brush, hoverType);

            dictionary.Add(colorKey, colorKey.ToString());
            dictionary.Add(brushKey, brushKey.ToString());
        }

        public static Color GetColor(ResourceDictionary resources, ColorType colorType, ElementType elementType,
            HoverType hoverType = HoverType.None)
        {
            var pair = ColorTypeInfo.Create(colorType, elementType, ResourceType.Color, hoverType);

            return (Color)resources[colors[pair]];
        }

        public static SolidColorBrush GetBrush(ResourceDictionary resources, ColorType colorType, ElementType elementType,
            HoverType hoverType = HoverType.None)
        {
            var pair = ColorTypeInfo.Create(colorType, elementType, ResourceType.Brush, hoverType);

            return resources[colors[pair]] as SolidColorBrush;
        }
    }
}
