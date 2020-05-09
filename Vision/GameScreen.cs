using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleCity.Domain;
using System.Drawing.Drawing2D;
using System.Resources;

namespace BattleCity.Vision
{
    public class GameScreen : UserControl
    {
        private int CellSize;
        private static Brush backgroundBrush = new SolidBrush(Color.FromArgb(20, 20, 20));
        private readonly Timer timer;
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private Point GraphicsZeroTransform => new Point((Width - Game.CurrentMap.Width * CellSize) / 2, CellSize);
        private int time = 0;
        private int mapHeight => Game.CurrentMap.Height + 1;
        private readonly Dictionary<Direction, float> directionToDegrees = new Dictionary<Direction, float>
        {
            { Direction.Up, 0},
            { Direction.Down, 180},
            { Direction.Left, 270},
            { Direction.Right, 90},
        };

        public GameScreen()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = Color.DimGray;
            SetCellsize(mapHeight);
            timer = new Timer();
            timer.Interval = 20;
            SizeChanged += (s, e) => 
            {
                SetCellsize(mapHeight);
                Invalidate();
            };
            timer.Tick += OnTick;
            Game.StageChanged += () =>
            {
                if (Game.Stage == GameStage.Playing)
                    timer.Start();
                if (Game.Stage == GameStage.Lost || Game.Stage == GameStage.Won)
                    timer.Stop();
                time = 0;
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Game.keyPressed = e.KeyCode;
            pressedKeys.Add(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (pressedKeys.Contains(e.KeyCode))
                pressedKeys.Remove(e.KeyCode);
            Game.keyPressed = pressedKeys.Any() ? pressedKeys.First() : Keys.None;
        }

        private void SetCellsize(int mapHeight)
        {
            CellSize = Height / mapHeight;
        }
        private void OnTick(object sender, EventArgs args)
        {
            var rand = new Random();
            if (time % 2 == 0)
            {
                var bullets = Game.CurrentMap.Entities.BulletsFlying.Select(x => x).ToHashSet();
                foreach (var bullet in bullets)
                {
                    bullet.CheckForCollisions();
                    bullet.Move();
                }
            }
            if (time % 4 == 0)
            {
                foreach (var enemy in Game.CurrentMap.Entities.ActiveEnemies)
                {
                    enemy.Act();
                    enemy.Tank.WaitForShootPenalty();
                }
                var dogs = Game.CurrentMap.Entities.Dogs.Select(x => x).ToHashSet();
                foreach (var dog in dogs)
                    dog.Act();
                HandleButton(Game.keyPressed);
                Game.CurrentMap.Player.WaitForShootPenalty();
            }
            if (time % 200 == 0)
                Game.CurrentMap.CreateEnemy(Game.CurrentMap.GetRandomEmptyPoint(rand), Direction.Up, 32, 1);
            if (time == 400)
            {
                Game.CurrentMap.CreateDog(Game.CurrentMap.GetRandomEmptyPoint(rand));
                time = 0;
            }
            time++;
            Invalidate();
        }

        private void HandleButton(Keys key)
        {
            switch (key)
            {
                case Keys.A:
                    Game.CurrentMap.Player.MoveTo(Direction.Left);
                    break;
                case Keys.W:
                    Game.CurrentMap.Player.MoveTo(Direction.Up);
                    break;
                case Keys.S:
                    Game.CurrentMap.Player.MoveTo(Direction.Down);
                    break;
                case Keys.D:
                    Game.CurrentMap.Player.MoveTo(Direction.Right);
                    break;
                case Keys.Space:
                    Game.CurrentMap.Player.Shoot();
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(GraphicsZeroTransform.X, GraphicsZeroTransform.Y);
            DrawUI(e.Graphics, Game.CurrentMap);
            FillBackground(e.Graphics, Game.CurrentMap);
            DrawWalls(e.Graphics, Game.CurrentMap);
            DrawDogs(e.Graphics, Game.CurrentMap);
            DrawTanks(e.Graphics, Game.CurrentMap);
            DrawBullets(e.Graphics, Game.CurrentMap);
        }

        private void DrawUI(Graphics graphics, Map map)
        {
            graphics.DrawImage(Resource.Enemy, CellSize * 2, -CellSize, CellSize, CellSize);
            graphics.DrawString(map.EnemiesCount.ToString(), new Font("Arial", 30), Brushes.Black, CellSize * 3.2f, -CellSize * 0.9f);
            graphics.DrawImage(Resource.Heart, CellSize * 6, -CellSize, CellSize, CellSize);
            graphics.DrawString(map.Player.HealthPoints.ToString(), new Font("Arial", 30), Brushes.Black, CellSize * 7.2f, -CellSize * 0.9f);
        }

        private void DrawWalls(Graphics graphics, Map map)
        {
            foreach (var wallPos in map.WallCells)
                graphics.DrawImage(Resource.BigWall, new Rectangle(wallPos.X * CellSize, wallPos.Y * CellSize, CellSize, CellSize));
        }

        private void DrawTanks(Graphics graphics, Map map)
        {
            DrawTank(graphics, map, map.Player, Resource.Tank);
            foreach (var enemy in map.Entities.ActiveEnemies)
                DrawTank(graphics, map, enemy.Tank, Resource.Enemy);
            graphics.ResetTransform();
            graphics.TranslateTransform(GraphicsZeroTransform.X, GraphicsZeroTransform.Y);
        }

        private void DrawDogs(Graphics graphics, Map map)
        {
            foreach (var dog in map.Entities.Dogs)
            {
                graphics.DrawImage(Resource.Dog, new Rectangle(dog.Position.X * CellSize, dog.Position.Y * CellSize, CellSize, CellSize));
            }
        }

        private void DrawTank(Graphics graphics, Map map, ITank tank, Image tankImage)
        {
            var tankScale = 0.8f;
            graphics.TranslateTransform((tank.Position.X + 0.5f) * CellSize, (tank.Position.Y + 0.5f) * CellSize);
            graphics.RotateTransform(directionToDegrees[tank.Direction]);
            graphics.DrawImage(tankImage, - CellSize * tankScale / 2, - CellSize * tankScale / 2, CellSize * tankScale, CellSize * tankScale);
            graphics.ResetTransform();
            graphics.TranslateTransform(GraphicsZeroTransform.X, GraphicsZeroTransform.Y);
        }

        private void DrawBullets(Graphics graphics, Map map)
        {
            foreach (var bullet in map.Entities.BulletsFlying)
                graphics.DrawImage(Resource.Bullet, (bullet.Position.X + 0.5f) * CellSize, (bullet.Position.Y + 0.5f) * CellSize, CellSize / 6f, CellSize / 6f);
        }

        private void FillBackground(Graphics graphics, Map map)
        {
            graphics.FillRectangle(backgroundBrush, new Rectangle(0, CellSize, map.Width * CellSize, map.Height * CellSize));
        }
    }
}
