using System.Collections.Generic;
using System.Windows.Media;

namespace CardWords.Views.Common
{
    public static class ThemeColor
    {
        public enum ColorType
        {
            Default = 1,
            Active,
            Hover,
            HoverActive,
            Correct,
            Wrong,
            NewWord,
            WordResult,
            ValidationErrorField,
            ValidationError
        }

        public enum ElementType
        {
            Default = 1,
            Line,
            Text,
            Background
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

        private static Color defaultBacgroundColor = Color.FromRgb(233, 233, 233); // #e9e9e9

        private static Color defaultColor = Color.FromRgb(0, 121, 169); // #0079a9
        private static Color defaultLineColor = Color.FromRgb(0, 151, 211); // #0097d3
        private static Color defaultTextColor = Color.FromRgb(238, 238, 238); // #DDD

        private static Color defaultActiveColor = Color.FromRgb(78, 49, 130); //#4e3182
        private static Color defaultActiveLineColor = Color.FromRgb(106, 68, 174); //#6a44ae
        private static Color defaultActiveTextColor = Color.FromRgb(204, 204, 204); //#CCC

        private static Color hoverColor = Color.FromRgb(0, 151, 211); // #0097d3
        private static Color hoverLineColor = Color.FromRgb(41, 186, 244); // #29baf4
        private static Color hoverTextColor = Color.FromRgb(255, 255, 255); // #FFF

        private static Color hoverActiveColor = Color.FromRgb(106, 68, 174); //#6a44ae
        private static Color hoverActiveLineColor = Color.FromRgb(132, 83, 220); //#8453dc
        private static Color hoverActiveTextColor = Color.FromRgb(221, 221, 221); //#DDD

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


        private static IReadOnlyDictionary<TypePair, Color> colors = new Dictionary<TypePair, Color>
        {
            {TypePair.Create(ColorType.Default, ElementType.Background), defaultBacgroundColor},

            {TypePair.Create(ColorType.Default, ElementType.Default), defaultColor},
            {TypePair.Create(ColorType.Default, ElementType.Line), defaultLineColor},
            {TypePair.Create(ColorType.Default, ElementType.Text), defaultTextColor},

            {TypePair.Create(ColorType.Active, ElementType.Default), defaultActiveColor},
            {TypePair.Create(ColorType.Active, ElementType.Line), defaultActiveLineColor},
            {TypePair.Create(ColorType.Active, ElementType.Text), defaultActiveTextColor},

            {TypePair.Create(ColorType.Hover, ElementType.Default), hoverColor},
            {TypePair.Create(ColorType.Hover, ElementType.Line), hoverLineColor},
            {TypePair.Create(ColorType.Hover, ElementType.Text), hoverTextColor},

            {TypePair.Create(ColorType.HoverActive, ElementType.Default), hoverActiveColor},
            {TypePair.Create(ColorType.HoverActive, ElementType.Line), hoverActiveLineColor},
            {TypePair.Create(ColorType.HoverActive, ElementType.Text), hoverActiveTextColor},

            {TypePair.Create(ColorType.Correct, ElementType.Default), correctColor},
            {TypePair.Create(ColorType.Correct, ElementType.Line), correctLineColor},
            {TypePair.Create(ColorType.Correct, ElementType.Text), correctTextColor},

            {TypePair.Create(ColorType.Wrong, ElementType.Default), wrongtColor},
            {TypePair.Create(ColorType.Wrong, ElementType.Line), wrongLineColor},
            {TypePair.Create(ColorType.Wrong, ElementType.Text), wrongTextColor},

            {TypePair.Create(ColorType.NewWord, ElementType.Default), newWordColor},
            {TypePair.Create(ColorType.NewWord, ElementType.Line), newWordLineColor},
            {TypePair.Create(ColorType.NewWord, ElementType.Text), newWordTextColor},

            {TypePair.Create(ColorType.WordResult, ElementType.Default), wordResultColor},
            {TypePair.Create(ColorType.WordResult, ElementType.Background), wordResultBacgroundColor},
            {TypePair.Create(ColorType.WordResult, ElementType.Text), wordResultTextColor},

            {TypePair.Create(ColorType.ValidationErrorField, ElementType.Default), validationErrorFieldColor},
            {TypePair.Create(ColorType.ValidationErrorField, ElementType.Text), validationErrorFieldTextColor},

            {TypePair.Create(ColorType.ValidationError, ElementType.Text), validationErrorTextColor},
        };

        public static Color GetColor(ColorType colorType, ElementType elementType)
        {
            var pair = TypePair.Create(colorType, elementType);

            return colors[pair];
        }
    }
}
