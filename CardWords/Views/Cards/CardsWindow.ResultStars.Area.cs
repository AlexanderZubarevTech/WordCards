using System;
using System.Windows;

namespace WordCards.Views.Cards
{
    public partial class CardsWindow
    {
        private sealed partial class ResultStars
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

        }
    }
}
