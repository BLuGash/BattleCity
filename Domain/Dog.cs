using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleCity.Domain
{
    public class Dog
    {
        public Point Position { get; private set; }
        private readonly Map map;
        List<Direction> currentWay;
        private Point currentTankPos;
        private int currentIndex = 0 ;

        public Dog(Point position, Map map)
        {
            Position = position;
            this.map = map;
        }

        public void Act()
        {
            if (map.Entities.ActiveEnemies.Any(enemy => enemy.Tank.Position == Position))
                map.DestroyTank(map.Entities.ActiveEnemies.Where(enemy => enemy.Tank.Position == Position).FirstOrDefault().Tank);
            if (currentWay == null || currentIndex >= currentWay.Count || map.Entities.Tanks.All(tank => tank.Position == currentTankPos))
                FindTank();
            if (currentWay != null && currentIndex < currentWay.Count)
            {
                MoveTo(currentWay[currentIndex++]);
                if (currentIndex >= currentWay.Count)
                {
                    currentIndex = 0;
                    currentWay = null;
                }
            }
        }

        public void MoveTo(Direction direction)
        {
            var nextPos = Position + direction.ConvertToVector();
            if (nextPos.CanMoveTo(map))
                Position = nextPos;
        }

        public void FindTank()
        {
            var queue = new Queue<Point>();
            var track = new Dictionary<Point, Point>();
            queue.Enqueue(Position);
            while (queue.Count > 0)
            {
                var currentPos = queue.Dequeue();
                foreach (var nextPoint in GetNeighbours(currentPos)
                    .Where(nextPoint => !track.ContainsKey(nextPoint)))
                {
                        track.Add(nextPoint, currentPos);
                    if (nextPoint.CanMoveTo(map))
                    {
                        queue.Enqueue(nextPoint);
                    }
                    if (map.Entities.ActiveEnemies.Any(enemy => enemy.Tank.Position == nextPoint))
                    {
                        currentTankPos = nextPoint;
                        currentWay = ConvertWayToDirections(ConvertTrackInfoToWay(track, nextPoint));
                        return;
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

        private IEnumerable<Point> GetNeighbours(Point point)
        {
            yield return point + new Size(-1, 0);
            yield return point + new Size(1, 0);
            yield return point + new Size(0, -1);
            yield return point + new Size(0, 1);
        }
    }
}
