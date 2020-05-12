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
        private readonly Timer timer;
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private Point GraphicsZeroTransform => new Point((Width - Game.CurrentMap.Width * CellSize) / 2, CellSize);
        private int time = 0;
        private int mapHeight => Game.CurrentMap.Height + 1;

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
            var map = Game.CurrentMap;
            var entities = map.Entities;
            if (time % 2 == 0)
            {
                var bullets = entities.BulletsFlying.Select(x => x).ToHashSet();
                foreach (var bullet in bullets)
                {
                    bullet.CheckForCollisions();
                    bullet.Move();
                }
            }
            if (time % 4 == 0)
            {
                ActAllEntities(map, entities);
                HandleButton(Game.keyPressed);
                map.Player.WaitForShootPenalty();
            }
            if (time % 200 == 0)
                map.CreateEnemy(map.GetRandomEmptyPoint(rand), Direction.Up, 32, 1);
            if (time % 400 == 0)
                map.CreateDog(map.GetRandomEmptyPoint(rand));
            if (time == 800)
            {
                map.SpawnPanin(map.GetRandomEmptyPoint(rand));
                time = 0;
            }
            time++;
            Invalidate();
        }

        private void ActAllEntities(Map map, MapEntities entities)
        {
            foreach (var enemy in entities.ActiveEnemies)
            {
                enemy.Act();
                enemy.Tank.WaitForShootPenalty();
            }
            var dogs = entities.Dogs.Select(x => x).ToHashSet();
            foreach (var dog in dogs)
                dog.Act();
            var panin = entities.Panin;
            if (panin != null)
                panin.Act();
            
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
            e.Paint(CellSize, GraphicsZeroTransform);
        }
    }
}
