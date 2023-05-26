using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CardWords.Views.Cards
{
    public partial class CardsWindow
    {
        private sealed partial class ResultStars
        {
            private sealed partial class StarManager
            {
                public delegate void StarManagerEventHandler(Star star);
                public event StarManagerEventHandler EndAnimationEvent;

                private Grid drawGrid;
                private List<Star> stars = new();
                private AreaManager areaManager;
                private Random random;
                private Timer timer;
                private TimeSpan timePassed;
                private Dispatcher mainDispatcher;
                private double intensity;

                private List<StarEventInfo> timeline = new List<StarEventInfo>();

                public StarManager(Dispatcher mainDispatcher, Grid drawGrid, AreaManager areaManager, Random random, double intensity)
                {
                    this.mainDispatcher = mainDispatcher;
                    this.drawGrid = drawGrid;
                    this.random = random;
                    this.areaManager = areaManager;
                    this.intensity = intensity;
                    timer = CreateTimer();
                    timePassed = new TimeSpan();
                    EndAnimationEvent += EndAnimation;
                }

                private Timer CreateTimer()
                {
                    var timer = new Timer();

                    timer.Enabled = true;
                    timer.Interval = 20;
                    timer.Elapsed += TimerAction;
                    timer.AutoReset = true;

                    return timer;
                }

                public void Start()
                {
                    areaManager.CalculateAvailableAreas();
                    CreateStars();
                    LoadTimeLine();

                    timer.Start();
                }

                private void TimerAction(object sender, ElapsedEventArgs e)
                {
                    timePassed = timePassed.Add(TimeSpan.FromMilliseconds(timer.Interval));

                    var count = timeline.Count(x => x.StartTime <= timePassed);

                    if (count > 0)
                    {
                        while (true)
                        {
                            var info = timeline.Where(x => x.StartTime <= timePassed).FirstOrDefault();

                            if (info != null)
                            {
                                timeline.Remove(info);

                                mainDispatcher.BeginInvoke(delegate (StarEventInfo x) { x.Action(); }, info);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                public void Stop()
                {
                    timer.Stop();
                    timer.Dispose();
                }

                private void CreateStars()
                {
                    var starCount = Convert.ToInt32(Math.Round(areaManager.AvailableCount() * intensity));

                    for (int i = 0; i < starCount; i++)
                    {
                        var star = CreateStar();

                        if (star != null)
                        {
                            stars.Add(star);
                            drawGrid.Children.Add(star.Polygon);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                private void LoadTimeLine()
                {
                    var sortedStars = stars.OrderBy(x => x.TimeDelayInMilliseconds).ToList();

                    foreach (var star in sortedStars)
                    {
                        var time = TimeSpan.FromMilliseconds(star.TimeDelayInMilliseconds);

                        var info = new StarEventInfo(star, time, BeginAnimation);

                        timeline.Add(info);
                    }
                }

                private void BeginAnimation(Star star)
                {
                    star.BeginAnimation();
                }

                private void EndAnimation(Star star)
                {
                    areaManager.FreeArea(star.Area.Id);

                    var availableAreaId = areaManager.GetAvailableAreaId();

                    if (availableAreaId == default)
                    {
                        return;
                    }

                    var area = areaManager[availableAreaId];

                    ChangeStarArea(star, area);

                    var time = timePassed.Add(TimeSpan.FromMilliseconds(star.TimeDelayInMilliseconds));

                    var info = new StarEventInfo(star, time, BeginAnimation);

                    timeline.Add(info);
                }

                private Star? CreateStar()
                {
                    var availableAreaId = areaManager.GetAvailableAreaId();

                    if (availableAreaId == default)
                    {
                        return null;
                    }

                    var area = areaManager[availableAreaId];
                    var polygon = GetPolygon(area);

                    var delay = Convert.ToInt32(Math.Round(defaultDelayInMillisecons * (random.Next(50, 150) / 100d), 0));
                    var duration = Convert.ToInt32(Math.Round(defaultDurationInMillisecons * (random.Next(20, 100) / 100d), 0));

                    areaManager.ReserveArea(availableAreaId);

                    return new Star(area, polygon, delay, duration, EndAnimationEvent);
                }

                private void ChangeStarArea(Star star, Area newArea)
                {
                    star.Area = newArea;

                    DrawPolygon(star.Polygon, newArea);

                    var center = newArea.GetCenter();

                    var scaleTransform = star.Polygon.RenderTransform as ScaleTransform;

                    scaleTransform.CenterX = center.X;
                    scaleTransform.CenterY = center.Y;
                }

                private Polygon GetPolygon(Area area)
                {
                    var polygon = new Polygon();

                    DrawPolygon(polygon, area);

                    var center = area.GetCenter();

                    polygon.RenderTransform = new ScaleTransform(0, 0, center.X, center.Y);

                    return polygon;
                }

                private void DrawPolygon(Polygon polygon, Area area)
                {
                    var centerPoint = area.GetCenter();

                    polygon.Points.Clear();

                    var scale = random.Next(20, 100) / 100d;                    

                    DrawByCenter(polygon, centerPoint, scale);

                    polygon.Fill = GetBrush(255);

                    polygon.Opacity = 0;
                }

                private static void DrawByCenter(Polygon polygon, Point center, double scale)
                {
                    var centerOffset = defaultSize / 10;

                    var p_1 = new Point(center.X, center.Y - halfDefaultSize * scale);
                    var p_2 = new Point(center.X - centerOffset * scale, center.Y - centerOffset * scale);
                    var p_3 = new Point(center.X - halfDefaultSize * scale, center.Y);
                    var p_4 = new Point(center.X - centerOffset * scale, center.Y + centerOffset * scale);
                    var p_5 = new Point(center.X, center.Y + halfDefaultSize * scale);
                    var p_6 = new Point(center.X + centerOffset * scale, center.Y + centerOffset * scale);
                    var p_7 = new Point(center.X + halfDefaultSize * scale, center.Y);
                    var p_8 = new Point(center.X + centerOffset * scale, center.Y - centerOffset * scale);

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
            }
        }
    }
}
