using BattleCity.Domain;
using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Tests
{
    class EnemiesTests
    {
        [Test]
        public void ShootsIfSeesPlayer()
        {
            var map = Map.Default;
            map.CreateEnemy(new Point(2, 0), Direction.Down, 5, 1);
            map.ActiveEnemies.First().Act();
            Assert.AreEqual(1, map.BulletsFlying.Count);
        }

        [Test]
        public void BlindEnemy()
        {
            var map = Map.Default;
            map.CreateEnemy(new Point(2, 0), Direction.Down, 0, 1);
            map.ActiveEnemies.First().Act();
            Assert.AreEqual(0, map.BulletsFlying.Count);
        }

        [Test]
        public void DoesntSeePlayerThroughWall()
        {
            var map = Map.CreateMapFromText(BattleCity.Properties.Resources.testmap3);
            map.CreateEnemy(new Point(1, 7), Direction.Down, 5, 1);
            var enemy = map.ActiveEnemies.First();
            enemy.Act();
            Assert.AreEqual(0, map.BulletsFlying.Count);
            enemy.Act();
            enemy.Act();
            enemy.Act();
            Assert.IsTrue(0 == map.BulletsFlying.Count);
        }
    }
}
