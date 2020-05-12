using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BattleCity.Domain
{
    public class Panin
    {
        public Point Position { get; private set; }
        private readonly Map map;
        private List<Direction> currentWay;
        private int currentIndex;
        private Point currentDogPos = Game.DefaultPoint;

        public Panin(Point position, Map map)
        {
            Position = position;
            this.map = map;
        }

        public void Act()
        {
            if (map.Entities.Dogs.Any(dog => dog.Position == Position))
                map.KillDog(map.Entities.Dogs.Where(dog => dog.Position == Position).First());
            if (currentWay == null || currentIndex >= currentWay.Count || map.Entities.Dogs.All(dog => dog.Position == currentDogPos))
                FindSomeDog();
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

        private void FindSomeDog()
        {
            var queue = new Queue<Point>();
            var track = new Dictionary<Point, Point>();
            queue.Enqueue(Position);
            track.Add(Position, Game.DefaultPoint);
            while (queue.Count > 0)
            {
                var currentPos = queue.Dequeue();
                foreach (var nextPoint in currentPos.GetNeighbours()
                    .Where(nextPoint => !track.ContainsKey(nextPoint)))
                {

                    if (nextPoint.CanMoveTo(map))
                    {
                        track.Add(nextPoint, currentPos);
                        queue.Enqueue(nextPoint);
                    }
                    if (map.Entities.Dogs.Any(dog => dog.Position == nextPoint))
                    {
                        currentDogPos = nextPoint;
                        currentWay = ConvertTrackInfoToWay(track, nextPoint).ConvertWayToDirections();
                        return;
                    }
                }
            }
        }

        private List<Point> ConvertTrackInfoToWay(Dictionary<Point, Point> track, Point end)
        {
            var resultList = new List<Point>();
            var currentPoint = end;
            while (currentPoint != Game.DefaultPoint)
            {
                resultList.Add(currentPoint);
                currentPoint = track[currentPoint];
            }
            resultList.Reverse();
            return resultList;
        }
    }
}
