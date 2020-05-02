using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BattleCity.Domain
{
    public class Map
    {
        public static Map Default => new Map(5, 5, new Point(2, 2), Direction.Up, 1);
        public readonly HashSet<Point> WallCells;
        public readonly HashSet<Point> EmptyCells;
        public readonly ITank Player;
        public int EnemiesCount { get; private set; }
        public readonly HashSet<ITank> Tanks;
        public readonly HashSet<Enemy> ActiveEnemies = new HashSet<Enemy>();
        public readonly HashSet<IBullet> BulletsFlying = new HashSet<IBullet>();
        public readonly int Width;
        public readonly int Height;
        public GameStage stage { get; private set; } = GameStage.Playing;
        public Map(int width, int height, Point playerPosition, Direction playerDirection, int enemeiesCount, 
            HashSet<Point> emptyCells, HashSet<Point> wallCells)
        {
            Width = width;
            Height = height;
            Tanks = new HashSet<ITank>();
            Player = new Tank(playerPosition, playerDirection, 1, this);
            EnemiesCount = enemeiesCount;
            WallCells = wallCells;
            EmptyCells = emptyCells;
        }

        public Map(int width, int height, Point playerPosition, Direction playerDirection, int enemeiesCount)
            : this(width, height, playerPosition, playerDirection, enemeiesCount,
                  Enumerable.Range(0, width * height).Select(x => new Point(x / width, x % width)).ToHashSet(), new HashSet<Point>())
        { }

        public static Map CreateMapFromText(string text)
        {
            return CreateMapFromLines(text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static Map CreateMapFromLines(string[] lines)
        {
            var height = lines.Length - 1;
            var width = lines[1].Length;
            var playerPos = Game.DefaultPoint;
            var emptyCells = new HashSet<Point>();
            var wallCells = new HashSet<Point>();
            for (var i = 1; i <= height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (lines[i][j] == '#')
                        wallCells.Add(new Point(j, i - 1));
                    else if (lines[i][j] == 'P')
                    {
                        playerPos = new Point(j, i - 1);
                        emptyCells.Add(new Point(j, i - 1));
                    }
                    else if (lines[i][j] == ' ')
                        emptyCells.Add(new Point(i - 1, j));
                    else throw new ArgumentException($"Undifined symbol '{lines[i][j]}'");
                }
            }
            if (playerPos == Game.DefaultPoint)
                throw new InvalidOperationException("There is no player");
            return new Map(width, height, playerPos, Direction.Up,
                int.Parse(lines[0]), emptyCells, wallCells);
        }

        public void CreateEnemy(Point position, Direction direction, int visionDistance, int HP)
        {
            if (EnemiesCount <= 0)
                throw new InvalidOperationException("there is no free enemies");
            if (!EmptyCells.Contains(position))
                throw new ArgumentException("Position is not at empty position");
            EnemiesCount--;
            var enemyTank = new Tank(position, direction, HP, this);
            ActiveEnemies.Add(new Enemy(enemyTank, visionDistance, this));
        }

        public Point GetRandomEmptyPoint()
        {
            return EmptyCells.Skip(new Random().Next(EmptyCells.Count)).FirstOrDefault();
        }

        public void DestroyTank(ITank tank)
        {
            if (Tanks.Contains(tank))
            {
                Tanks.Remove(tank);
                if (EnemiesCount == 0)
                    stage = GameStage.Won;
                else if (tank == Player)
                    stage = GameStage.Lost;
            }
        }

        public void DestroyBullet(IBullet bullet)
        {
            if (BulletsFlying.Contains(bullet))
                BulletsFlying.Remove(bullet);
        }
    }
}
