using WordCards.Business.WordAction;
using WordCards.Business.WordActivities;
using WordCards.Configurations;
using WordCards.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WordCards.Views.Cards
{
    /// <summary>
    /// Логика взаимодействия для CardsWindow.xaml
    /// </summary>
    public partial class CardsWindow : Window
    {
        private readonly WordActionData[] data;

        private readonly WordActionInfo info;

        private readonly Dispatcher mainDispatcher;

        private readonly List<ResultStars> resultStars = new(2);

        private readonly CustomTimer timer;

        private bool wordIsShowed;

        private bool isResult;

        private int maxCorrectAnswerSequence;

        public CardsWindow(int wordCount)
        {
            InitializeComponent();

            wordIsShowed = true;
            maxCorrectAnswerSequence = 0;
            CorrectAnswerSequence = 0;
            isResult = false;            

            mainDispatcher = Dispatcher.CurrentDispatcher;

            info = new WordActionInfo
            {
                StartDate = TimeHelper.GetCurrentDate(),                
                SelectedCardWordsCount = wordCount,
                LanguageId = AppConfiguration.Instance.CurrentLanguage,
                TranslationLanguageId = AppConfiguration.Instance.CurrentTranslationLanguage,
            };

            data = CommandHelper.GetCommand<IGetWordActionDataCommand>().Execute(wordCount);

            if(data.Length == 0)
            {
                timer = new CustomTimer(PB_timer);

                info.WordsCount = 0;
                info.EndDate = info.StartDate;

                ShowResult();
            }
            else
            {
                var word = data[0];

                timer = new CustomTimer(word, PB_timer, LeftTime);

                Initialize(word);
            }
        }

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

        private void Initialize(WordActionData word)
        {
            info.WordsCount = data.Length;

            SetProgress(data.Length);

            SetDataItem(word);

            ShowWord(word.IsNewWord);

            G_Result.Visibility = Visibility.Hidden;
            G_WordCard.Visibility = Visibility.Visible;

            if (!word.IsNewWord)
            {
                timer.Start();
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

        private void ShowWord(bool isNewWord)
        {
            if (isNewWord)
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

        private void LeftTime(object? sender, ElapsedEventArgs e)
        {
            mainDispatcher.BeginInvoke(() => 
            {
                if (timer.IsTimeLeft)
                {
                    TimeLeft();
                } 
                else
                {
                    timer.Progress();
                }
            });
        }

        #region Color

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

        private void SetWrongColor(WordActionData.Side correctSide, WordActionData.Side? side = null)
        {
            SetWordBackgroundColor(BackgroundColor.ColorType.Wrong);

            if(side != null)
            {
                SetTranslationBackgroundColor(BackgroundColor.ColorType.Wrong, side.Value);
            }
            
            SetTranslationBackgroundColor(BackgroundColor.ColorType.Correct, correctSide);
        }

        private void SetWordBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetStyle(R_WordBackground, Resources, type, BackgroundColor.ElementType.Background);
            BackgroundColor.SetStyle(P_WordBackground_Line_1, Resources, type, BackgroundColor.ElementType.Line);
            BackgroundColor.SetStyle(P_WordBackground_Line_2, Resources, type, BackgroundColor.ElementType.Line);
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
            BackgroundColor.SetStyle(R_LeftTranslationBackbround, Resources, type, BackgroundColor.ElementType.Background);
        }

        private void SetRightTranslationBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetStyle(R_RightTranslationBackbround, Resources, type, BackgroundColor.ElementType.Background);
        }

        private void SetUnderstoodBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetStyle(R_UnderstoodBackbround, Resources, type, BackgroundColor.ElementType.Background);
            BackgroundColor.SetStyle(P_UnderstoodBackbround_Line_1, Resources, type, BackgroundColor.ElementType.Line);
            BackgroundColor.SetStyle(P_UnderstoodBackbround_Line_2, Resources, type, BackgroundColor.ElementType.Line);
        }

        #endregion

        #region Events

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.IsRepeat)
            {
                return;
            }

            if(isResult)
            {
                ResultFromKeyDown(e.Key);
            }
            else
            {
                ActionFromKeyDown(e.Key);
            }
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

        private void GridResultClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResultFromKeyDown(Key.Enter);
        }        

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            ResultFromKeyDown(Key.Enter);
        }

        private void Heap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        #endregion

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
                SetWrongColor(currentWord.CorrectSide, side);
            }

            SetCorrectAnswerSequence(result);

            Next(true);
        }

        private void TimeLeft()
        {
            var currentWord = GetCurrentWord();

            var result = currentWord.Check();

            SetWrongColor(currentWord.CorrectSide);

            SetCorrectAnswerSequence(result);

            Next(true);
        }

        private async void Next(bool wait = false)
        {
            timer.Stop();

            wordIsShowed = false;

            PB_progress.Value++;            

            var nextWord = GetNextWord();

            if(nextWord == null)
            {
                info.EndDate = TimeHelper.GetCurrentDate();
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

            timer.Restart(nextWord);
        }        

        private void ShowResult()
        {
            isResult = true;
            timer.Stop();

            TB_ResultWordsCount.Text = info.WordsCount.ToString();
            TB_ResultSequenceCount.Text = info.MaxSequence.ToString();
            TB_ResultNewWordsCount.Text = info.NewWordsCount.ToString();
            TB_ResultCorrectWordsCount.Text = info.CorrectAnswersCount.ToString();
            TB_ResultWrongWordsCount.Text = info.WrongAnswersCount.ToString();
            TB_ResultTime.Text = TimeHelper.GetTime(info.Duration);

            G_WordCard.Visibility = Visibility.Collapsed;
            G_Result.Visibility = Visibility.Visible;

            DrawStars();
        }

        private async void DrawStars()
        {
            await AsyncDrawStars();
        }

        private async Task AsyncDrawStars()
        {
            await Task.Delay(200);

            DrawStarsInternal();
        }

        private void DrawStarsInternal()
        {
            var random = new Random((int)info.Duration.TotalSeconds);

            var stars = new ResultStars(mainDispatcher, G_Stars, random, 0.5);

            stars.SetProhibitedArea(TB_ResultWordsCountTitle);
            stars.SetProhibitedArea(TB_ResultWordsCount);

            stars.StartDraw();

            resultStars.Add(stars);

            var stars2 = new ResultStars(mainDispatcher, G_Stars2, random, 0.5);

            stars2.SetProhibitedArea(TB_ResultCorrectWordsCountTitle);
            stars2.SetProhibitedArea(TB_ResultCorrectWordsCount);

            stars2.StartDraw();

            resultStars.Add(stars2);

            var stars3 = new ResultStars(mainDispatcher, G_Stars3, random, 0.5);

            stars3.SetProhibitedArea(TB_ResultNewWordsCountTitle);
            stars3.SetProhibitedArea(TB_ResultNewWordsCount);

            stars3.SetProhibitedArea(TB_ResultSequenceCountTitle);
            stars3.SetProhibitedArea(TB_ResultSequenceCount);

            stars3.StartDraw();

            resultStars.Add(stars3);

            var stars4 = new ResultStars(mainDispatcher, G_Stars4, random, 0.5);

            stars4.SetProhibitedArea(TB_ResultWrongWordsCountTitle);
            stars4.SetProhibitedArea(TB_ResultWrongWordsCount);

            stars4.SetProhibitedArea(TB_ResultTimeTitle);
            stars4.SetProhibitedArea(TB_ResultTime);

            stars4.StartDraw();

            resultStars.Add(stars4);

            var closeStars = new ResultStars(mainDispatcher, G_CloseStars, random, 0.6);

            closeStars.SetProhibitedArea(TB_Result_Close);

            closeStars.StartDraw();

            resultStars.Add(closeStars);
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

        #region Override

        protected override void OnClosed(EventArgs e)
        {
            foreach (var item in resultStars)
            {
                item.Stop();
            }

            (Owner as MainWindow).IsRunCards = false;

            Owner.Show();

            timer.Dispose();

            base.OnClosed(e);
        }

        #endregion        
    }
}
