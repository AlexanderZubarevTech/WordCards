using CardWords.Business.WordAction;
using CardWords.Configurations;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CardWords.Views.Cards
{
    public partial class CardsWindow
    {
        private sealed class CustomTimer
        {
            private const int interval = 10; // ms

            private static readonly int progressMaximumValue = 1000 / interval * AppConfiguration.Instance.WordCardTimerDutationInSeconds;

            public CustomTimer(ProgressBar progressBar)
            {
                this.progressBar = progressBar;
            }

            public CustomTimer(WordActionData firstWord, ProgressBar progressBar, ElapsedEventHandler elapsed)
            {
                this.progressBar = progressBar;

                if(AppConfiguration.Instance.WordCardHasTimer)
                {
                    timer = CreateTimer(elapsed);
                    progressBar.Visibility = Visibility.Visible;
                    progressBar.Maximum = progressMaximumValue;
                    progressBar.Value = firstWord.IsNewWord ? progressMaximumValue : 0;
                }
                else
                {
                    progressBar.Visibility = Visibility.Collapsed;
                }
            }

            private readonly Timer? timer;
            private readonly ProgressBar progressBar;

            public bool IsTimeLeft => progressBar.Value == progressMaximumValue;

            private static Timer CreateTimer(ElapsedEventHandler elapsed)
            {
                var timer = new Timer();

                timer.Elapsed += elapsed;
                timer.Enabled = false;
                timer.Interval = interval;
                timer.AutoReset = true;

                return timer;
            }

            public void Start()
            {
                if(timer != null)
                {
                    timer.Start();
                }
            }

            public void Stop()
            {
                if (timer != null)
                {
                    timer.Stop();
                }
            }

            public void Dispose()
            {
                if (timer != null)
                {
                    timer.Dispose();
                }
            }

            public void Restart(WordActionData nextWord)
            {
                if(timer == null)
                {
                    return;
                }

                if (nextWord.IsNewWord)
                {
                    timer.Stop();
                    progressBar.Value = progressMaximumValue;
                }
                else
                {
                    progressBar.Value = 0;
                    timer.Start();
                }
            }

            public void Progress()
            {
                progressBar.Value++;
            }

        }           
    }
}
