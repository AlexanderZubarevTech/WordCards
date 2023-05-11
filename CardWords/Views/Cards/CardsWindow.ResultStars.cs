using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CardWords.Views.Cards
{
    public partial class CardsWindow
    {
        private static class ResultStars
        {            
            private static readonly Point defaultPolygonCenterPoint = new(50, 50);
            private static readonly byte defaultColor = 204; //#CCC

            public static Polygon GetPolygon(Polygon pointPolygon, Random random)
            {
                var centerPoint = GetCenterPointPolygon(pointPolygon);

                var scale = random.Next(10, 30) / 100d;
                var color = (byte) random.Next(200, 250);

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
    }
}
