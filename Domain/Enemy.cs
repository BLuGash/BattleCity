using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BattleCity.Domain
{
    public class Enemy
    {
        public readonly ITank Tank;
        private readonly Map map;
        public int VisionDistance { get; }
        private List<Direction> currentWay;
        private int currentIndex = 0;
        public Enemy(ITank tank, int visionDistance, Map map)
        {
            Tank = tank;
            if (visionDistance > Math.Max(map.Width, map.Height))
                throw new ArgumentException("Enemy sees too good");
            VisionDistance = visionDistance;
            this.map = map;
        }

        private static readonly Dictionary<Size, Direction> offsetToDirection = new Dictionary<Size, Direction>
        {
            {new Size(0, -1), Direction.Up},
            {new Size(0, 1), Direction.Down},
            {new Size(-1, 0), Direction.Left},
            {new Size(1, 0), Direction.Right}
        };

        private static readonly Dictionary<Direction, Size> directionToOffset = new Dictionary<Direction, Size>
        {
            {Direction.Up, new Size(0, -1)},
            {Direction.Down, new Size(0, 1)},
            {Direction.Left, new Size(-1, 0)},
            {Direction.Right, new Size(1, 0)}
        };

        public void Act()
        {
            if (!PlayerIsNearby())
                MoveRandomly();
            else
            {
                var playerPos = FindPlayer();
                if (playerPos == Game.DefaultPoint && (currentWay == null || currentIndex >= currentWay.Count))
                {
                    MoveRandomly();
                    currentIndex = 0;
                }
                else
                {
                    Tank.MoveTo(currentWay[currentIndex++]);
                    if (GetAllPointsAhead().Any(point => playerPos == point))
                        Tank.Shoot();
                }
            }
        }

        private IEnumerable<Point> GetAllPointsAhead()
        {
            var currentPoint = Tank.Position;
            for (var i = 0; i < VisionDistance; i++)
            {
                currentPoint += directionToOffset[Tank.Direction];
                yield return currentPoint;
            }
        }

        private bool PlayerIsNearby()
        {
            return map.Player.Position.X - Tank.Position.X
                + map.Player.Position.Y - Tank.Position.Y <= VisionDistance;
        }

        public Point FindPlayer()
        {
            if (!PlayerIsNearby())
                return Game.DefaultPoint;
            var queue = new Queue<(int, Point)>();
            var track = new Dictionary<Point, Point>();
            queue.Enqueue((0, Tank.Position));
            while (queue.Count > 0)
            {
                var currentPos = queue.Dequeue();
                foreach (var nextPoint in GetNeighbours(currentPos.Item2).Where(point => !track.ContainsKey(point)))
                {
                    if (nextPoint == map.Player.Position)
                    {
                        track.Add(nextPoint, currentPos.Item2);
                        currentWay = ConvertWayToDirections(ConvertTrackInfoToWay(track, nextPoint));
                        return nextPoint;
                    }
                    if (nextPoint.CanMoveTo(map))
                    {
                    track.Add(nextPoint, currentPos.Item2);
                    queue.Enqueue((currentPos.Item1 + 1, nextPoint));
                    }
                }
            }
            return Game.DefaultPoint;
        }

        private static List<Direction> ConvertWayToDirections(IEnumerable<Point> way)
        {
            return way
                .Zip(way.Skip(1), (begin, end) => offsetToDirection[new Size(end.X - begin.X, end.Y - begin.Y)])
                .ToList();
        }

        private List<Point> ConvertTrackInfoToWay(Dictionary<Point, Point> track, Point end)
        {
            var resultList = new List<Point>();
            var currentPoint = end;
            while (track.ContainsKey(currentPoint))
            {
                resultList.Add(track[currentPoint]);
                currentPoint = track[currentPoint];
            }
            resultList.Reverse();
            return resultList;
        }

        private void MoveRandomly()
        {
            switch(new Random().Next(8))
            {
                case 0:
                    Tank.MoveTo(Direction.Down);
                    break;
                case 1:
                    Tank.MoveTo(Direction.Left);
                    break;
                case 2:
                    Tank.MoveTo(Direction.Right);
                    break;
                case 3:
                    Tank.MoveTo(Direction.Up);
                    break;
            }
        }

        private IEnumerable<Point> GetNeighbours(Point point)
        {
            yield return point + new Size(-1, 0);
            yield return point + new Size(1, 0);
            yield return point + new Size(0, -1);
            yield return point + new Size(0, 1);
        }
    }
}
