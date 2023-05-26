using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CardWords.Views.Cards
{
    public partial class CardsWindow
    {
        private sealed partial class ResultStars
        {

            private const double defaultSize = 25d; // размер звезды в пикселях
            private const int defaultDurationInMillisecons = 2000;
            private const int defaultDelayInMillisecons = 2000;

            private static readonly double halfDefaultSize = defaultSize / 2;            

            private readonly AreaManager areaManager;
            private readonly StarManager starManager;

            public ResultStars(Dispatcher dispatcher, Grid drawGrid, Random random, double intensity)
            {
                areaManager = new AreaManager(drawGrid, random);
                starManager = new StarManager(dispatcher, drawGrid, areaManager, random, intensity);
            }

            public void SetProhibitedArea(UIElement element)
            {
                areaManager.SetProhibitedArea(element);
            }

            public void StartDraw()
            {
                starManager.Start();
            }

            public void Stop()
            {
                starManager.Stop();
            }
        }
    }
}
