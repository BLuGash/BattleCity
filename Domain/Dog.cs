﻿using System;
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
            {
                map.DestroyTank(map.Entities.ActiveEnemies.Where(enemy => enemy.Tank.Position == Position).FirstOrDefault().Tank);
                map.KillDog(this);
            }
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
                    if (map.Entities.ActiveEnemies.Any(enemy => enemy.Tank.Position == nextPoint))
                    {
                        track.Add(nextPoint, currentPos);
                        currentTankPos = nextPoint;
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
