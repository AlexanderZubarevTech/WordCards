using CardWords.Business.WordAction;
using CardWords.Business.WordActivities;
using CardWords.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CardWords.Views.Cards
{
    /// <summary>
    /// Логика взаимодействия для CardsWindow.xaml
    /// </summary>
    public partial class CardsWindow : Window
    {
        private WordActionData[] data;

        private bool wordIsShowed;

        private bool pressedKey;

        private bool isResult;

        private WordActionInfo info;

        private Timer timer;

        private Dispatcher mainDispatcher;

        private List<ResultStars> resultStars = new(2);        

        public CardsWindow(int wordCount)
        {
            InitializeComponent();

            mainDispatcher = Dispatcher.CurrentDispatcher;

            data = CommandHelper.GetCommand<IGetWordActionDataCommand>().Execute(wordCount);

            var word = data[0];

            SetProgress(data.Length);

            SetDataItem(word);

            ShowWord(word.IsNewWord);

            wordIsShowed = true;

            maxCorrectAnswerSequence = 0;
            CorrectAnswerSequence = 0;
            isResult = false;
            pressedKey = false;

            info = new WordActionInfo
            {
                StartDate = TimeHelper.GetCurrentDate(),
                WordsCount = data.Length,
                SelectedCardWordsCount = wordCount,
                LanguageId = MainWindow.AppConfiguration.CurrentLanguage,
                TranslationLanguageId = MainWindow.AppConfiguration.CurrentTranslationLanguage,
            };

            G_Result.Visibility = Visibility.Hidden;
            G_WordCard.Visibility = Visibility.Visible;

            PB_timer.Value = word.IsNewWord ? PB_timer.Maximum : 0;

            timer = CreateTimer();

            if(!word.IsNewWord)
            {
                timer.Start();
            }
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

        private Timer CreateTimer()
        {
            var timer = new Timer();
            
            timer.Enabled = false;
            timer.Interval = 10;
            timer.Elapsed += LeftTime;
            timer.AutoReset = true;

            return timer;
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

        private void LeftTime(object sender, ElapsedEventArgs e)
        {
            mainDispatcher.BeginInvoke(() => 
            {
                if (PB_timer.Value < PB_timer.Maximum)
                {
                    PB_timer.Value += 1;
                } 
                else
                {
                    TimeLeft();
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
        }

        private void SetRightTranslationBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetColor(R_RightTranslationBackbround, type);            
        }

        private void SetUnderstoodBackgroundColor(BackgroundColor.ColorType type)
        {
            BackgroundColor.SetColor(R_UnderstoodBackbround, type);
            BackgroundColor.SetLineColor(P_UnderstoodBackbround_Line_1, type);
            BackgroundColor.SetLineColor(P_UnderstoodBackbround_Line_2, type);
        }

        #endregion

        #region Events

        private void Window_KeyDown(object sender, KeyEventArgs e)
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

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            pressedKey = false;
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

            if(pressedKey)
            {
                return;
            }

            pressedKey = true;

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

            if(nextWord.IsNewWord)
            {
                timer.Stop();
                PB_timer.Value = PB_timer.Maximum;
            } 
            else
            {
                PB_timer.Value = 0;
                timer.Start();
            }            
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

        private void DrawStars()
        {
            var random = new Random((int)info.Duration.TotalSeconds);

            var stars = new ResultStars(mainDispatcher, G_Stars, random, 0.3);

            stars.SetProhibitedArea(TB_ResultWordsCountTitle);
            stars.SetProhibitedArea(TB_ResultWordsCount);

            stars.SetProhibitedArea(TB_ResultSequenceCountTitle);
            stars.SetProhibitedArea(TB_ResultSequenceCount);

            stars.SetProhibitedArea(TB_ResultNewWordsCountTitle);
            stars.SetProhibitedArea(TB_ResultNewWordsCount);

            stars.SetProhibitedArea(TB_ResultCorrectWordsCountTitle);
            stars.SetProhibitedArea(TB_ResultCorrectWordsCount);

            stars.SetProhibitedArea(TB_ResultWrongWordsCountTitle);
            stars.SetProhibitedArea(TB_ResultWrongWordsCount);

            stars.SetProhibitedArea(TB_ResultTimeTitle);
            stars.SetProhibitedArea(TB_ResultTime);

            stars.StartDraw();

            resultStars.Add(stars);

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
