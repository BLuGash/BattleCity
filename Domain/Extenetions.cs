﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleCity.Domain
{
    public static class Extenetions
    {
        public static Size ConvertToVector(this Direction direction)
        {
            return new Size(direction == Direction.Left ? -1 : direction == Direction.Right ? 1 : 0,
                direction == Direction.Up ? -1 : direction == Direction.Down ? 1 : 0);
        }

        public static bool IntersectedWith(this Rectangle r1, Rectangle r2)
        {
            return (r1.Left <= r2.Right == r2.Left <= r1.Right) && (r1.Top <= r2.Bottom == r2.Top <= r1.Bottom);
        }

        public static bool CanMoveTo(this Point point, Map map)
        {
            return map.EmptyCells.Contains(point)
                && map.Entities.Tanks.All(tank => tank.Position != (point))
                && point.InsideMap(map);
        }

        public static bool InsideMap(this Point point, Map map)
        {
            return point.X >= 0
                && point.Y >= 0
                && point.X < map.Width
                && point.Y < map.Height;
        }

        public static IEnumerable<Point> GetNeighbours(this Point point)
        {
            yield return point + new Size(0, 1);
            yield return point + new Size(-1, 0);
            yield return point + new Size(1, 0);
            yield return point + new Size(0, -1);
        }

        public static List<Direction> ConvertWayToDirections(this IEnumerable<Point> way)
        {
            return way
                .Zip(way.Skip(1), (begin, end) => Game.offsetToDirection[new Size(end.X - begin.X, end.Y - begin.Y)])
                .ToList();
        }
    }
}
