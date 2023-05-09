using CardWords.Business.WordAction;
using CardWords.Business.WordActivities;
using CardWords.Core.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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

        private static class ResultStars
        {            
            private static readonly Point defaultPolygonCenterPoint = new(50, 50);
            private static readonly byte defaultColor = 204; //#CCC

            public static Polygon GetPolygon(Polygon pointPolygon, Random random)
            {
                var centerPoint = GetCenterPointPolygon(pointPolygon);

                var scale = random.Next(10, 60) / 100d;
                var color = (byte) random.Next(100, 240);

                var polygon = new Polygon();

                DrawByCenter(polygon, centerPoint, scale);

                polygon.Fill = GetBrush(color);

                return polygon;
            }            

            private static void DrawByCenter(Polygon polygon, Point center, double scale)
            {
                var p_1 = new Point(center.X, center.Y - 50 * scale);
                var p_2 = new Point(center.X - 10 * scale, center.Y - 10 * scale);
                var p_3 = new Point(center.X - 50 * scale, center.Y);
                var p_4 = new Point(center.X - 10 * scale, center.Y + 10 * scale);
                var p_5 = new Point(center.X, center.Y + 50 * scale);
                var p_6 = new Point(center.X + 10 * scale, center.Y + 10 * scale);
                var p_7 = new Point(center.X + 50 * scale, center.Y);
                var p_8 = new Point(center.X + 10 * scale, center.Y - 10 * scale);

                polygon.Points.Add(p_1);
                polygon.Points.Add(p_2);
                polygon.Points.Add(p_3);
                polygon.Points.Add(p_4);
                polygon.Points.Add(p_5);
                polygon.Points.Add(p_6);
                polygon.Points.Add(p_7);
                polygon.Points.Add(p_8);
            }

            private static Brush GetBrush(byte value)
            {
                var color = Color.FromRgb(value, value, value);

                return new SolidColorBrush(color);
            }

            private static Point GetCenterPointPolygon(Polygon polygon)
            {
                var point = polygon.Points.First();

                return new Point(point.X + 1, point.Y + 1);
            }            
        }

        private WordActionData[] data;

        private bool wordIsShowed;

        private bool isResult;

        private WordActionInfo info;

        public CardsWindow(int wordCount)
        {
            InitializeComponent();

            data = CommandHelper.GetCommand<IGetWordActionDataCommand>().Execute(wordCount);

            var word = data[0];

            SetProgress(data.Length);

            SetDataItem(word);

            ShowWord(word.IsNewWord);

            wordIsShowed = true;

            maxCorrectAnswerSequence = 0;
            CorrectAnswerSequence = 0;
            isResult = false;

            info = new WordActionInfo
            {
                StartDate = DateTime.Now,
                WordsCount = data.Length,
                SelectedCardWordsCount = wordCount
            };

            G_Result.Visibility = Visibility.Collapsed;
            G_WordCard.Visibility = Visibility.Visible;
        }

        private int maxCorrectAnswerSequence;

        private int _correctAnswerSequence;
        private int CorrectAnswerSequence
        { 
            get
            {
                return _correctAnswerSequence;
            }
            set 
            {
                _correctAnswerSequence = value;

                if(value > maxCorrectAnswerSequence)
                {
                    maxCorrectAnswerSequence = value;
                }
            } 
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

        private void SetCorrectAnswerSequence(WordActivityType type)
        {
            if(type == WordActivityType.CorrectAnswer)
            {
                CorrectAnswerSequence++;
            }
            else
            {
                CorrectAnswerSequence = 0;
            }
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
            if(isResult)
            {
                ResultFromKeyDown(e.Key);
            }
            else
            {
                ActionFromKeyDown(e.Key);
            }            
        }

        private void ResultFromKeyDown(Key key)
        {
            if(key != Key.Escape && key != Key.Enter && key != Key.Space)
            {
                return;
            }

            Close();
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

            if (result == WordActivityType.CorrectAnswer)
            {
                SetCorrectColor(side);
            }
            else
            {
                SetWrongColor(side, currentWord.CorrectSide);
            }

            SetCorrectAnswerSequence(result);

            Next(true);
        }

        private async void Next(bool wait = false)
        {
            wordIsShowed = false;

            PB_progress.Value++;

            var nextWord = GetNextWord();

            if(nextWord == null)
            {
                info.EndDate = DateTime.Now;
            }

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
                SaveResult();

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
            isResult = true;

            TB_ResultWordsCount.Text = info.WordsCount.ToString();
            TB_ResultSequenceCount.Text = info.MaxSequence.ToString();
            TB_ResultNewWordsCount.Text = info.NewWordsCount.ToString();
            TB_ResultCorrectWordsCount.Text = info.CorrectAnswersCount.ToString();
            TB_ResultWrongWordsCount.Text = info.WrongAnswersCount.ToString();
            TB_ResultTime.Text = TimeHelper.GetTime(info.Duration);

            // draw stars

            var starsPointPolygons = G_Stars.Children.Cast<Polygon>().ToList();

            var random = new Random((int)info.Duration.TotalSeconds);            

            foreach (var item in starsPointPolygons)
            {
                var starPolygon = ResultStars.GetPolygon(item, random);

                G_Result.Children.Add(starPolygon);
            }

            G_Result.Visibility = Visibility.Visible;
            G_WordCard.Visibility = Visibility.Collapsed;
        }

        private void SaveResult()
        {
            info.MaxSequence = maxCorrectAnswerSequence;            

            foreach (var item in data)
            {
                if (item.IsNewWord)
                {
                    info.NewWordsCount++;
                    continue;
                }

                if (item.Result == WordActivityType.CorrectAnswer)
                {
                    info.CorrectAnswersCount++;
                    continue;
                }

                info.WrongAnswersCount++;
            }

            CommandHelper.GetCommand<ISaveWordActionDataCommand>().Execute(data, info);
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
