using System;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CardWords.Views.Cards
{
    public partial class CardsWindow
    {
        private sealed partial class ResultStars
        {
            private sealed partial class StarManager
            {
                public sealed class Star
                {
                    public Star(Area area, Polygon polygon, int timeDelayInMilliseconds, int timeDurationInMilliseconds, StarManagerEventHandler changeAreaEvent)
                    {
                        Area = area;
                        Polygon = polygon;
                        TimeDelayInMilliseconds = timeDelayInMilliseconds;
                        TimeDurationInMilliseconds = timeDurationInMilliseconds;

                        this.changeAreaEvent = changeAreaEvent;

                        animation = new DoubleAnimation
                        {
                            From = 0,
                            To = 1,
                            Duration = TimeSpan.FromMilliseconds(TimeDurationInMilliseconds),
                            AutoReverse = true
                        };

                        animation.Completed += Animation_Completed;
                    }

                    private DoubleAnimation animation;

                    private StarManagerEventHandler changeAreaEvent;

                    public Area Area { get; set; }

                    public Polygon Polygon { get; }

                    public int TimeDelayInMilliseconds { get; }

                    public int TimeDurationInMilliseconds { get; }

                    public void BeginAnimation()
                    {
                        Polygon.BeginAnimation(OpacityProperty, animation);
                        Polygon.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                        Polygon.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
                    }

                    private void Animation_Completed(object sender, EventArgs e)
                    {
                        changeAreaEvent(this);
                    }
                }
            }
        }
    }
}
