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
            map.Entities.ActiveEnemies.First().Act();
            Assert.AreEqual(1, map.Entities.BulletsFlying.Count);
        }

        [Test]
        public void BlindEnemy()
        {
            var map = Map.Default;
            map.CreateEnemy(new Point(2, 0), Direction.Down, 0, 1);
            map.Entities.ActiveEnemies.First().Act();
            Assert.AreEqual(0, map.Entities.BulletsFlying.Count);
        }

        [Test]
        public void DoesntSeePlayerThroughWall()
        {
            var map = MapCreator.CreateMapFromText(BattleCity.Properties.Resources.testmap3);
            map.CreateEnemy(new Point(7, 1), Direction.Down, 5, 1);
            var enemy = map.Entities.ActiveEnemies.First();
            enemy.Act();
            Assert.AreEqual(0, map.Entities.BulletsFlying.Count);
            enemy.Act();
            enemy.Act();
            enemy.Act();
            Assert.AreEqual(0, map.Entities.BulletsFlying.Count);
        }
    }
}
