using BattleCity.Domain;
using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Tests
{
    class ShootingTests
    {
        [Test]
        public void BulletDamages()
        {
            var map = Map.Default;
            var tank = new Tank(new Point(2, 0), Direction.Down, 1, map);
            map.Player.Shoot();
            var bullet = (Bullet)map.BulletsFlying.First();
            bullet.CheckForCollisions();
            Assert.AreEqual(2, map.Tanks.Count);
            bullet.Move();
            bullet.CheckForCollisions();
            Assert.AreEqual(2, map.Tanks.Count);
            bullet.Move();
            bullet.CheckForCollisions();
            Assert.AreEqual(1, map.Tanks.Count);
        }

        [Test]
        public void BulletDestroysNearTheEndOfMap()
        {
            var map = Map.Default;
            map.Player.MoveTo(Direction.Right);
            map.Player.MoveTo(Direction.Right);
            map.Player.Shoot();
            ((Bullet)map.BulletsFlying.First()).CheckForCollisions();
            Assert.AreEqual(0, map.BulletsFlying.Count);
        }

        [Test]
        public void BulletDestroysNearTheWall()
        {
            var map = Map.CreateMapFromText(BattleCity.Properties.Resources.testmap);
            map.Player.Shoot();
            ((Bullet)map.BulletsFlying.First()).CheckForCollisions();
            Assert.AreEqual(0, map.BulletsFlying.Count);
        }
    }
}