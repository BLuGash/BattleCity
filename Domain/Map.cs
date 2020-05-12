using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BattleCity.Domain
{
    public class Map
    {
        public static Map Default => new Map(5, 5, new Point(2, 2), Direction.Up, 1, 1);
        public readonly HashSet<Point> WallCells;
        public readonly HashSet<Point> EmptyCells;
        public readonly MapEntities Entities;
        public readonly Tank Player;
        public int EnemiesCount { get; private set; }
        public readonly int Width;
        public readonly int Height;
        public Map(int width, int height, Point playerPosition, Direction playerDirection, int playerHP, int enemeiesCount, 
            HashSet<Point> emptyCells, HashSet<Point> wallCells)
        {
            Width = width;
            Height = height;
            Entities = new MapEntities();
            Player = CreateTank(playerPosition, playerDirection, playerHP);
            EnemiesCount = enemeiesCount;
            WallCells = wallCells;
            EmptyCells = emptyCells;
        }

        public Map(int width, int height, Point playerPosition, Direction playerDirection, int playerHP, int enemeiesCount)
            : this(width, height, playerPosition, playerDirection, playerHP, enemeiesCount,
                  Enumerable.Range(0, width * height).Select(x => new Point(x / width, x % width)).ToHashSet(), new HashSet<Point>())
        { }

        public void CreateEnemy(Point position, Direction direction, int visionDistance, int HP)
        {
            if (EnemiesCount <= 0)
                return;
            if (!EmptyCells.Contains(position) || Player.Position == position)
                throw new ArgumentException("Position is not empty");
            EnemiesCount--;
            var enemyTank = CreateTank(position, direction, HP);
            Entities.ActiveEnemies.Add(new Enemy(enemyTank, visionDistance, this));
        }

        public Point GetRandomEmptyPoint(Random rand)
        {
            return EmptyCells
                .Where(point => !Entities.Tanks.Any(tank => tank.Position == point))
                .Skip(rand.Next(EmptyCells.Count))
                .FirstOrDefault();
        }

        public void SpawnPanin(Point position)
        {
            Entities.Panin = new Panin(position, this);
        }

        public void CreateDog(Point position)
        {
            Entities.Dogs.Add(new Dog(position, this));
        }

        public void KillDog(Dog dog)
        {
            if (Entities.Dogs.Contains(dog))
                Entities.Dogs.Remove(dog);
        }

        public void DestroyTank(ITank tank)
        {
            if (Entities.Tanks.Contains(tank))
            {
                Entities.Tanks.Remove(tank);
                if (tank != Player)
                {
                    Entities.ActiveEnemies
                        .Remove(Entities.ActiveEnemies.Where(enemy => enemy.Tank == tank).FirstOrDefault());
                    if (EnemiesCount == 0)
                    {
                        Game.PassLevel();
                    }
                }
                
                else if (tank == Player)
                    Game.ChangeState(GameStage.Lost);
            }
        }

        public void DestroyBullet(IBullet bullet)
        {
            if (Entities.BulletsFlying.Contains(bullet))
                Entities.BulletsFlying.Remove(bullet);
        }

        private Tank CreateTank(Point position, Direction direction, int HP)
        {
            var tank = new Tank(position, direction, HP, this);
            Entities.Tanks.Add(tank);
            return tank;
        }
    }
}
