using BattleCity.Domain;
using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Tests
{
    public class TankTests
    {

        [Test]
        public void TankShouldMoveProperly()
        {
            var tank = new Tank(new Point(2, 2), Direction.Down, 1, Map.Default);
            Assert.AreEqual(Direction.Down, tank.Direction);
            tank.Rotate(Direction.Up);
            Assert.AreEqual(Direction.Up, tank.Direction);
            tank.MoveTo(Direction.Left);
            Assert.AreEqual(Direction.Left, tank.Direction);
            Assert.AreEqual(new Point(1, 2), tank.Position);
        }

        [Test]
        public void CircleMove()
        {
            var map = Map.Default;
            var playerPos = map.Player.Position;
            map.Player.MoveTo(Direction.Left);
            map.Player.MoveTo(Direction.Down);
            map.Player.MoveTo(Direction.Right);
            map.Player.MoveTo(Direction.Up);
            Assert.AreEqual(Direction.Up, map.Player.Direction);
            Assert.AreEqual(playerPos, map.Player.Position);
        }

        [Test]
        public void TankDoesntMoveThroughWallsAndOutOfMap()
        {
            var map = Map.CreateMapFromText(BattleCity.Properties.Resources.testmap);
            map.Player.MoveTo(Direction.Up);
            Assert.AreEqual(new Point(1, 1), map.Player.Position);
            var map2 = Map.Default;
            map2.Player.MoveTo(Direction.Up);
            map2.Player.MoveTo(Direction.Up);
            map2.Player.MoveTo(Direction.Up);
            map2.Player.MoveTo(Direction.Up);
            map2.Player.MoveTo(Direction.Up);
            map2.Player.MoveTo(Direction.Up);
            Assert.AreEqual(new Point(2, 0), map2.Player.Position);
        }

        [Test]
        public void TankCanShoot()
        {
            var map = Map.CreateMapFromText(BattleCity.Properties.Resources.testmap);
            map.Player.Shoot();
            Assert.AreEqual(1, map.BulletsFlying.Count);
            map.Player.Shoot();
            Assert.AreEqual(2, map.BulletsFlying.Count);
            Assert.AreEqual(map.BulletsFlying.FirstOrDefault().Direction, map.Player.Direction);
        }

        [Test]
        public void TanksShouldCollide()
        {
            var map = Map.Default;
            var tank1 = new Tank(new Point(0, 0), Direction.Down, 1, map);
            var tank2 = new Tank(new Point(0, 1), Direction.Down, 1, map);
            tank2.MoveTo(Direction.Left);
            Assert.AreEqual(new Point(0, 1), tank2.Position);
            Assert.AreEqual(Direction.Left, tank2.Direction);
        }
    }
}