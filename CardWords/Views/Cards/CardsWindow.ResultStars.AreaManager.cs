using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WordCards.Views.Cards
{
    public partial class CardsWindow
    {
        private sealed partial class ResultStars
        {
            private sealed class AreaManager
            {
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
        }
    }
}
