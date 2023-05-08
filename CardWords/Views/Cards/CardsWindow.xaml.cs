using CardWords.Business.WordAction;
using CardWords.Core.Helpers;
using CardWords.Core.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CardWords.Views.Cards
{
    /// <summary>
    /// Логика взаимодействия для CardsWindow.xaml
    /// </summary>
    public partial class CardsWindow : Window
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

            private static Color defaultColor = Color.FromRgb(93, 167, 168);
            private static Color defaultLineColor = Color.FromRgb(108, 195, 196);
            private static Color correctColor = Color.FromRgb(3, 132, 36);
            private static Color correctLineColor = Color.FromRgb(53, 162, 81);
            private static Color wrongColor = Color.FromRgb(108, 36, 33);
            private static Color wrongLineColor = Color.FromRgb(134, 58, 55);
            private static Color newWordColor = Color.FromRgb(77, 39, 139);
            private static Color newWordLineColor = Color.FromRgb(98, 44, 185);
            
            //private const string defaultColor_c = "#5da7a8";
            //private const string defaultLineColor_c = "#6cc3c4";
            //private const string correctColor_c = "#038424";
            //private const string correctLineColor_c = "#35a251";
            //private const string wrongColor_c = "#6c2421";
            //private const string wrongLineColor_c = "#863a37";



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

        private WordActionData[] data;

        private bool wordIsShowed;

        public CardsWindow(int wordCount)
        {
            InitializeComponent();

            data = CommandHelper.GetCommand<IGetWordActionDataCommand>().Execute(wordCount);

            var word = data[0];

            SetProgress(data.Length);

            SetDataItem(word);

            ShowWord(word.IsNewWord);

            wordIsShowed = true;
        }

        private void SetDataItem(WordActionData item)
        {
            var index = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if(item.Id == data[i].Id) 
                {
                    index = i;

                    break;
                }
            }

            WordIndex.Text = index.ToString();
            TB_WordName.Text = item.WordName;
            TB_Transcription.Text = item.Transcription;

            TB_TranslationNewWord.Text = item.IsNewWord 
                ? item.CorrectTranslation 
                : string.Empty;

            TB_LeftTranslation.Text = item.GetTranslationBySide(WordActionData.Side.Left);
            TB_RightTranslation.Text = item.GetTranslationBySide(WordActionData.Side.Right);                     
        }

        private void SetProgress(int count)
        {
            PB_progress.Minimum = 0;
            PB_progress.Maximum = count;
            PB_progress.Value = 0;
        }

        private void SetDelaultColor()
        {
            SetWordBackgroundColor(BackgroundColor.ColorType.Default);
            SetLeftTranslationBackgroundColor(BackgroundColor.ColorType.Default);
            SetRightTranslationBackgroundColor(BackgroundColor.ColorType.Default);
        }

        private void SetCorrectColor(WordActionData.Side side)
        {
            SetWordBackgroundColor(BackgroundColor.ColorType.Correct);
            SetTranslationBackgroundColor(BackgroundColor.ColorType.Correct, side);
        }

        private void SetWrongColor(WordActionData.Side side, WordActionData.Side correctSide)
        {
            SetWordBackgroundColor(BackgroundColor.ColorType.Wrong);
            SetTranslationBackgroundColor(BackgroundColor.ColorType.Wrong, side);
            SetTranslationBackgroundColor(BackgroundColor.ColorType.Correct, correctSide);
        }

        private void SetWordBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetColor(R_WordBackground, type);
            BackgroundColor.SetLineColor(P_WordBackground_Line_1, type);
            BackgroundColor.SetLineColor(P_WordBackground_Line_2, type);
        }

        private void SetTranslationBackgroundColor(BackgroundColor.ColorType type, WordActionData.Side side)
        {
            if(side == WordActionData.Side.Left)
            {
                SetLeftTranslationBackgroundColor(type);
            }

            if(side == WordActionData.Side.Right)
            {
                SetRightTranslationBackgroundColor(type);
            }
        }

        private void SetLeftTranslationBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetColor(R_LeftTranslationBackbround, type);
            BackgroundColor.SetLineColor(P_LeftTranslationBackbround_Line, type);            
        }

        private void SetRightTranslationBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetColor(R_RightTranslationBackbround, type);
            BackgroundColor.SetLineColor(P_RightTranslationBackbround_Line, type);
        }

        private void SetUnderstoodBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetColor(R_UnderstoodBackbround, type);
            BackgroundColor.SetLineColor(P_UnderstoodBackbround_Line_1, type);
            BackgroundColor.SetLineColor(P_UnderstoodBackbround_Line_2, type);
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            ActionFromKeyDown(e.Key);
        }

        private void ActionFromKeyDown(Key key)
        {
            if (!wordIsShowed)
            {
                return;
            }

            if (key != Key.Enter
                && key != Key.Space
                && key != Key.Right
                && key != Key.Left)
            {
                return;
            }           

            var currentWord = GetCurrentWord();

            if (currentWord.IsNewWord &&
                (key == Key.Enter ||
                key == Key.Space ||
                key == Key.Right))
            {
                currentWord.Check();

                Next();

                return;
            }

            if (key != Key.Right && key != Key.Left)
            {
                return;
            }

            var side = key == Key.Left ? WordActionData.Side.Left : WordActionData.Side.Right;

            var result = currentWord.Check(side);

            if (result == Business.WordActivities.WordActivityType.TrueAnswer)
            {
                SetCorrectColor(side);
            }
            else
            {
                SetWrongColor(side, currentWord.CorrectSide);
            }

            Next(true);
        }

        private async void Next(bool wait = false)
        {
            wordIsShowed = false;

            PB_progress.Value++;

            if (wait)
            {
                await AsyncNext();
            } 
            else
            {
                NextInternal();
            }
        }

        private async Task AsyncNext()
        {
            await Task.Delay(1000);

            NextInternal();
        }        

        private WordActionData GetCurrentWord()
        {
            return data[Convert.ToInt32(WordIndex.Text)];
        }

        private WordActionData? GetNextWord()
        {
            var index = Convert.ToInt32(WordIndex.Text);

            if(index == data.Length - 1)
            {
                return null;
            }

            return data[index + 1];
        }

        private void NextInternal()
        {
            var nextWord = GetNextWord();

            if(nextWord == null)
            {
                PB_progress.Value++;

                ShowResult();

                return;
            }

            SetDataItem(nextWord);

            ShowWord(nextWord.IsNewWord);

            wordIsShowed = true;
        }

        private void ShowWord(bool isNewWord)
        {
            if(isNewWord)
            {
                TB_TranslationNewWord.Visibility = Visibility.Visible;
                TB_NewWord.Visibility = Visibility.Visible;
                G_Understood.Visibility = Visibility.Visible;

                G_LeftTranslation.Visibility = Visibility.Collapsed;
                G_RightTranslation.Visibility = Visibility.Collapsed;

                SetWordBackgroundColor(BackgroundColor.ColorType.NewWord);
                SetUnderstoodBackgroundColor(BackgroundColor.ColorType.NewWord);

                return;
            }

            G_LeftTranslation.Visibility = Visibility.Visible;
            G_RightTranslation.Visibility = Visibility.Visible;

            TB_TranslationNewWord.Visibility = Visibility.Collapsed;
            TB_NewWord.Visibility = Visibility.Collapsed;
            G_Understood.Visibility = Visibility.Collapsed;

            SetDelaultColor();
        }

        private void ShowResult()
        {

        }

        private void Grid_LeftTranslation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ActionFromKeyDown(Key.Left);
        }

        private void Grid_RightTranslation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ActionFromKeyDown(Key.Right);
        }

        private void Grid_Understood_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ActionFromKeyDown(Key.Enter);
        }

        protected override void OnClosed(EventArgs e)
        {
            Owner.Show();

            base.OnClosed(e);
        }
    }
}
