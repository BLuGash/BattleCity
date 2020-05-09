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
        private Point currentPlayerPos = Game.DefaultPoint;
        private int currentIndex = 0;
        public Enemy(ITank tank, int visionDistance, Map map)
        {
            Tank = tank;
            if (visionDistance > map.Width + map.Height)
                throw new ArgumentException("Enemy sees too good");
            VisionDistance = visionDistance;
            this.map = map;
        }



        public void Act()
        {
            if (GetAllPointsAhead().Any(point => map.Player.Position == point))
                Tank.Shoot();
            else if (!PlayerIsNearby())
                MoveRandomly();
            else
            {
                if (currentWay == null || currentIndex >= currentWay.Count || map.Player.Position != currentPlayerPos)
                    FindPlayer();
                if (currentWay == null || currentIndex >= currentWay.Count)
                { 
                    MoveRandomly();
                    currentIndex = 0;
                    return;
                }
                Tank.MoveTo(currentWay[currentIndex++]);
                if (currentIndex >= currentWay.Count)
                {
                    currentIndex = 0;
                    currentWay = null;
                }
            }
        }

        private IEnumerable<Point> GetAllPointsAhead()
        {
            var currentPoint = Tank.Position;
            for (var i = 0; i < VisionDistance; i++)
            {
                currentPoint += Game.directionToOffset[Tank.Direction];
                if (map.WallCells.Contains(currentPoint))
                    break;
                yield return currentPoint;
            }
        }

        private bool PlayerIsNearby()
        {
            return Math.Abs(map.Player.Position.X - Tank.Position.X)
                + Math.Abs(map.Player.Position.Y - Tank.Position.Y) <= VisionDistance;
        }

        public void FindPlayer()
        {
            if (!PlayerIsNearby())
                return;
            var queue = new Queue<(int, Point)>();
            var track = new Dictionary<Point, Point>();
            queue.Enqueue((0, Tank.Position));
            while (queue.Count > 0)
            {
                var currentPos = queue.Dequeue();
                foreach (var nextPoint in GetNeighbours(currentPos.Item2).Where(point => !track.ContainsKey(point)))
                {
                    if (currentPos.Item1 < VisionDistance)
                    { 
                        if (nextPoint.CanMoveTo(map))
                        {
                            queue.Enqueue((currentPos.Item1 + 1, nextPoint));
                            track.Add(nextPoint, currentPos.Item2);
                        }
                        if (nextPoint == map.Player.Position)
                        {
                            track.Add(nextPoint, currentPos.Item2);
                            currentPlayerPos = nextPoint;
                            currentWay = ConvertWayToDirections(ConvertTrackInfoToWay(track, nextPoint));
                            return;
                        }
                    }
                }
            }
        }

        private static List<Direction> ConvertWayToDirections(IEnumerable<Point> way)
        {
            return way
                .Zip(way.Skip(1), (begin, end) => Game.offsetToDirection[new Size(end.X - begin.X, end.Y - begin.Y)])
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
