using System;

namespace CardWords.Views.Cards
{
    public partial class CardsWindow
    {
        private sealed partial class ResultStars
        {
            private sealed partial class StarManager
            {
                private sealed class StarEventInfo
                {
                    public delegate void StarInfoEventHandler(Star star);
                    private event StarInfoEventHandler Event;

                    public StarEventInfo(Star star, TimeSpan startTime, StarInfoEventHandler method)
                    {
                        Star = star;
                        StartTime = startTime;
                        Event += method;
                    }

                    public Star Star { get; }

                    public TimeSpan StartTime { get; }

                    public void Action()
                    {
                        Event.Invoke(Star);
                    }
                }
            }
        }
    }
}
