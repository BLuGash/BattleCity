using BattleCity.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleCity.Vision
{
    public static class GameScreenPainter
    {
        private static Brush backgroundBrush = new SolidBrush(Color.FromArgb(20, 20, 20));
        private static int _cellSize;
        private static readonly Dictionary<Direction, float> directionToDegrees = new Dictionary<Direction, float>
        {
            { Direction.Up, 0},
            { Direction.Down, 180},
            { Direction.Left, 270},
            { Direction.Right, 90},
        };

        public static void Paint(this PaintEventArgs e, int cellSize, Point zeroPos)
        {
            var graphics = e.Graphics;
            _cellSize = cellSize;
            graphics.TranslateTransform(zeroPos.X, zeroPos.Y);
            DrawUI(graphics, Game.CurrentMap);
            FillBackground(graphics, Game.CurrentMap);
            DrawWalls(graphics, Game.CurrentMap);
            DrawDogs(graphics, Game.CurrentMap);
            DrawPanin(graphics, Game.CurrentMap);
            DrawTanks(graphics, Game.CurrentMap, zeroPos);
            DrawBullets(graphics, Game.CurrentMap);
        }

        private static void DrawUI(Graphics graphics, Map map)
        {
            graphics.DrawImage(Resource.Enemy, _cellSize * 2, -_cellSize, _cellSize, _cellSize);
            graphics.DrawString(map.EnemiesCount.ToString(), new Font("Arial", 30), Brushes.Black, _cellSize * 3.2f, -_cellSize * 0.9f);
            graphics.DrawImage(Resource.Heart, _cellSize * 6, -_cellSize, _cellSize, _cellSize);
            graphics.DrawString(map.Player.HealthPoints.ToString(), new Font("Arial", 30), Brushes.Black, _cellSize * 7.2f, -_cellSize * 0.9f);
        }

        private static void DrawWalls(Graphics graphics, Map map)
        {
            foreach (var wallPos in map.WallCells)
                graphics.DrawImage(Resource.BigWall, new Rectangle(wallPos.X * _cellSize, wallPos.Y * _cellSize, _cellSize, _cellSize));
        }

        private static void DrawTanks(Graphics graphics, Map map, Point zeroPos)
        {
            DrawTank(graphics, map, map.Player, Resource.Tank, zeroPos);
            foreach (var enemy in map.Entities.ActiveEnemies)
                DrawTank(graphics, map, enemy.Tank, Resource.Enemy, zeroPos);
            graphics.ResetTransform();
            graphics.TranslateTransform(zeroPos.X, zeroPos.Y);
        }

        private static void DrawPanin(Graphics graphics, Map map)
        {
            if (map.Entities.Panin != null)
                graphics.DrawImage(Resource.Panin,
                    new Rectangle(map.Entities.Panin.Position.X * _cellSize, map.Entities.Panin.Position.Y * _cellSize, _cellSize, _cellSize));
        }

        private static void DrawDogs(Graphics graphics, Map map)
        {
            foreach (var dog in map.Entities.Dogs)
            {
                graphics.DrawImage(Resource.Dog, new Rectangle(dog.Position.X * _cellSize, dog.Position.Y * _cellSize, _cellSize, _cellSize));
            }
        }

        private static void DrawTank(Graphics graphics, Map map, ITank tank, Image tankImage, Point zeroPos)
        {
            var tankScale = 0.8f;
            graphics.TranslateTransform((tank.Position.X + 0.5f) * _cellSize, (tank.Position.Y + 0.5f) * _cellSize);
            graphics.RotateTransform(directionToDegrees[tank.Direction]);
            graphics.DrawImage(tankImage, -_cellSize * tankScale / 2, -_cellSize * tankScale / 2, _cellSize * tankScale, _cellSize * tankScale);
            graphics.ResetTransform();
            graphics.TranslateTransform(zeroPos.X, zeroPos.Y);
        }

        private static void DrawBullets(Graphics graphics, Map map)
        {
            foreach (var bullet in map.Entities.BulletsFlying)
                graphics.DrawImage(Resource.Bullet, (bullet.Position.X + 0.5f) * _cellSize, (bullet.Position.Y + 0.5f) * _cellSize, _cellSize / 6f, _cellSize / 6f);
        }

        private static void FillBackground(Graphics graphics, Map map)
        {
            graphics.FillRectangle(backgroundBrush, new Rectangle(0, _cellSize, map.Width * _cellSize, map.Height * _cellSize));
        }
    }
}
