using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace WordCards.Views.Cards
{
    public partial class CardsWindow
    {
        private static class BackgroundColor
        {
            public enum ColorType
            {
                Default = 1,
                Correct,
                Wrong,
                NewWord
            }

            public enum ElementType
            {
                Background = 1,
                Line
            }

            private readonly struct TypePair
            {
                public static TypePair Create(ColorType cType, ElementType eType)
                {
                    return new TypePair(cType, eType);
                }

                private TypePair(ColorType cType, ElementType eType)
                {
                    ColorType = cType;
                    ElementType = eType;
                }

                public ColorType ColorType { get; }

                public ElementType ElementType { get; }

                public static bool operator ==(TypePair left, TypePair right)
                {
                    return left.ColorType == right.ColorType && left.ElementType == right.ElementType;
                }

                public static bool operator !=(TypePair left, TypePair right)
                {
                    return !(left == right);
                }

                public override bool Equals(object? obj)
                {
                    if (obj == null || obj is not TypePair)
                    {
                        return false;
                    }

                    var pair = (TypePair)obj;

                    return this == pair;
                }

                public override int GetHashCode()
                {
                    return ColorType.GetHashCode() ^ ElementType.GetHashCode();
                }
            }

            private static readonly IReadOnlyDictionary<TypePair, string> styles = new Dictionary<TypePair, string>
            {
                {TypePair.Create(ColorType.Default, ElementType.Background), "DefaultBackground25"},
                {TypePair.Create(ColorType.Default, ElementType.Line), "DefaultLineBackground"},
                {TypePair.Create(ColorType.Correct, ElementType.Background), "CorrectBackground25"},
                {TypePair.Create(ColorType.Correct, ElementType.Line), "CorrectLineBackground"},
                {TypePair.Create(ColorType.Wrong, ElementType.Background), "WrongBackground25"},
                {TypePair.Create(ColorType.Wrong, ElementType.Line), "WrongLineBackground"},
                {TypePair.Create(ColorType.NewWord, ElementType.Background), "NewWordBackground25"},
                {TypePair.Create(ColorType.NewWord, ElementType.Line), "NewWordLineBackground"},
            };

            public static void SetStyle(Shape shape, ResourceDictionary resources, ColorType colorType, ElementType elementType)
            {
                var pair = TypePair.Create(colorType, elementType);

                var style = resources[styles[pair]] as Style;

                shape.Style = style;
            }
        }
    }
}
