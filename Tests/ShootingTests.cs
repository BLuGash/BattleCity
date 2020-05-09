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
            map.CreateEnemy(new Point(2, 0), Direction.Down, 0, 1);
            map.Player.Shoot();
            var bullet = (Bullet)map.Entities.BulletsFlying.First();
            bullet.CheckForCollisions();
            Assert.AreEqual(2, map.Entities.Tanks.Count);
            bullet.Move();
            bullet.CheckForCollisions();
            Assert.AreEqual(2, map.Entities.Tanks.Count);
            bullet.Move();
            bullet.CheckForCollisions();
            Assert.AreEqual(1, map.Entities.Tanks.Count);
        }

        [Test]
        public void BulletDestroysNearTheEndOfMap()
        {
            var map = Map.Default;
            map.Player.MoveTo(Direction.Right);
            map.Player.MoveTo(Direction.Right);
            map.Player.Shoot();
            ((Bullet)map.Entities.BulletsFlying.First()).CheckForCollisions();
            Assert.AreEqual(0, map.Entities.BulletsFlying.Count);
        }

        [Test]
        public void BulletDestroysNearTheWall()
        {
            var map = MapCreator.CreateMapFromText(BattleCity.Properties.Resources.testmap);
            map.Player.Shoot();
            ((Bullet)map.Entities.BulletsFlying.First()).CheckForCollisions();
            Assert.AreEqual(0, map.Entities.BulletsFlying.Count);
        }
    }
}