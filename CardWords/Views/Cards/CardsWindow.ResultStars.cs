using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CardWords.Views.Cards
{
    public partial class CardsWindow
    {
        private sealed class ResultStars
        {
            private sealed class AreaManager
            {
                public sealed class Area
                {
                    public Area(Point Id, Point startArea, Point endArea)
                    {
                        this.Id = Id;

                        SetPoints(startArea, endArea);
                    }

                    private void SetPoints(Point startArea, Point endArea)
                    {
                        P_1 = startArea;
                        P_2 = new Point(endArea.X, startArea.Y);
                        P_3 = endArea;
                        P_4 = new Point(startArea.X, endArea.Y);
                    }



                    public Point Id { get; }

                    public Point P_1 { get; private set; }

                    public Point P_2 { get; private set; }

                    public Point P_3 { get; private set; }

                    public Point P_4 { get; private set; }

                    public void ReCalculateArea(Point startArea, Point endArea)
                    {
                        SetPoints(startArea, endArea);
                    }

                    public bool IsIntersect(Area other)
                    {
                        if (other == null)
                        {
                            return false;
                        }

                        if (PointInArea(this, other.P_1)
                            || PointInArea(this, other.P_2)
                            || PointInArea(this, other.P_3)
                            || PointInArea(this, other.P_4)
                            || PointInArea(other, P_1)
                            || PointInArea(other, P_2)
                            || PointInArea(other, P_3)
                            || PointInArea(other, P_4))
                        {
                            return true;
                        }

                        return false;
                    }

                    private static bool PointInArea(Area area, Point point)
                    {
                        return point.X >= area.P_1.X
                            && point.Y >= area.P_1.Y
                            && point.X <= area.P_3.X
                            && point.Y <= area.P_3.Y;
                    }

                    public Point GetCenter()
                    {
                        var x = Math.Round((P_3.X + P_1.X) / 2, 1);

                        var y = Math.Round((P_3.Y + P_1.Y) / 2, 1);

                        return new Point(x, y);
                    }
                }

                public readonly Dictionary<string, Area> prohibitedAreas = new();
                public Dictionary<Point, Area> availableAreas = new();
                private List<Point> reservedAreaIds = new();
                private Point[] reservedArray;
                private Grid drawGrid;
                private Random random;

                public AreaManager(Grid drawGrid, Random random)
                {
                    this.drawGrid = drawGrid;
                    this.random = random;
                    reservedArray = new Point[9];
                }

                public Area this[Point id]
                {
                    get => availableAreas[id];
                    set => availableAreas[id] = value;
                }

                public void SetProhibitedArea(UIElement element)
                {
                    var name = (string)element.GetValue(NameProperty);

                    var startPoint = element.TranslatePoint(new Point(0, 0), drawGrid);

                    var startArea = new Point(Math.Round(startPoint.X, 1), Math.Round(startPoint.Y, 1));

                    var width = Math.Round((double)element.GetValue(ActualWidthProperty), 1);
                    var height = Math.Round((double)element.GetValue(ActualHeightProperty), 1);

                    var endArea = new Point(startArea.X + width, startArea.Y + height);

                    SetProhibitedArea(name, startPoint, endArea);
                }

                private void SetProhibitedArea(string name, Point startArea, Point endArea)
                {
                    if (prohibitedAreas.ContainsKey(name))
                    {
                        prohibitedAreas[name].ReCalculateArea(startArea, endArea);
                    }
                    else
                    {
                        prohibitedAreas.Add(name, new Area(startArea, startArea, endArea));
                    }
                }

                public void CalculateAvailableAreas()
                {
                    var width = drawGrid.ActualWidth;
                    var height = drawGrid.ActualHeight;

                    var rowCount = Math.Round(height / defaultSize, 0, MidpointRounding.ToNegativeInfinity);
                    var columnCount = Math.Round(width / defaultSize, 0, MidpointRounding.ToNegativeInfinity);

                    for (int i = 0; i < rowCount; i++)
                    {
                        var yOffset = i * defaultSize;

                        for (int k = 0; k < columnCount; k++)
                        {
                            var xOffset = k * defaultSize;

                            var startArea = new Point(xOffset, yOffset);
                            var endArea = new Point(xOffset + defaultSize, yOffset + defaultSize);
                            var id = new Point(i, k);
                            var newArea = new Area(id, startArea, endArea);

                            if (IsAvailableArea(newArea))
                            {
                                availableAreas.Add(id, newArea);
                            }
                        }
                    }
                }

                public bool IsAvailableArea(Area area)
                {
                    return prohibitedAreas.All(x => !x.Value.IsIntersect(area));
                }

                public Point GetAvailableAreaId()
                {
                    var areas = availableAreas.Keys.Where(CanReservedArea);

                    var count = areas.Count();

                    var skip = random.Next(count);

                    return areas.Skip(skip).FirstOrDefault();
                }

                public int AvailableCount() => availableAreas.Count() / 9;

                private bool CanReservedArea(Point areaId)
                {
                    FillReservedArray(areaId);

                    return reservedArray.Intersect(reservedAreaIds).Count() == 0;
                }

                private void FillReservedArray(Point areaId)
                {
                    reservedArray[0] = new Point(areaId.X - 1, areaId.Y - 1);
                    reservedArray[1] = new Point(areaId.X - 1, areaId.Y);
                    reservedArray[2] = new Point(areaId.X - 1, areaId.Y + 1);
                    reservedArray[3] = new Point(areaId.X, areaId.Y - 1);
                    reservedArray[4] = areaId;
                    reservedArray[5] = new Point(areaId.X, areaId.Y + 1);
                    reservedArray[6] = new Point(areaId.X + 1, areaId.Y - 1);
                    reservedArray[7] = new Point(areaId.X + 1, areaId.Y);
                    reservedArray[8] = new Point(areaId.X + 1, areaId.Y + 1);
                }

                public void ReserveArea(Point areaId)
                {
                    FillReservedArray(areaId);

                    reservedAreaIds.AddRange(reservedArray);
                }

                public void FreeArea(Point areaId)
                {
                    FillReservedArray(areaId);

                    for (int i = 0; i < reservedArray.Length; i++)
                    {
                        reservedAreaIds.Remove(reservedArray[i]);
                    }
                }
            }

            private sealed class StarManager
            {
                public sealed class Star
                {
                    public Star(AreaManager.Area area, Polygon polygon, int timeDelayInMilliseconds, int timeDurationInMilliseconds, StarManagerEventHandler changeAreaEvent)
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

                    public AreaManager.Area Area { get; set; }

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

                private void ChangeStarArea(Star star, AreaManager.Area newArea)
                {
                    star.Area = newArea;

                    DrawPolygon(star.Polygon, newArea);

                    var center = newArea.GetCenter();

                    var scaleTransform = star.Polygon.RenderTransform as ScaleTransform;

                    scaleTransform.CenterX = center.X;
                    scaleTransform.CenterY = center.Y;
                }

                private Polygon GetPolygon(AreaManager.Area area)
                {
                    var polygon = new Polygon();

                    DrawPolygon(polygon, area);

                    var center = area.GetCenter();

                    polygon.RenderTransform = new ScaleTransform(0, 0, center.X, center.Y);

                    return polygon;
                }

                private void DrawPolygon(Polygon polygon, AreaManager.Area area)
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
