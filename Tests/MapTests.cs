using BattleCity.Domain;
using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Tests
{
    class MapTests
    {
        [Test]
        public void MapShould()
        {
            var map = new Map(3, 5, new Point(2, 2), Direction.Up, 4);
            CheckForBasicConfiguration(map, 3, 5, new Point(2, 2), 4, 3 * 5);
        }

        [Test]
        public void MapFromText()
        {
            var map = Map.CreateMapFromText(BattleCity.Properties.Resources.testmap);
            CheckForBasicConfiguration(map, 16, 8, new Point(1, 1), 3, 14 * 6);
            var map2 = Map.CreateMapFromText(BattleCity.Properties.Resources.testmap2);
            CheckForBasicConfiguration(map2, 16, 13, new Point(14, 11), 17, 14 * 11 - 21);
        }

        private void CheckForBasicConfiguration(
            Map map, int width, int height, Point playerPos, int enemiesCount, int emptyCellsCount)
        {
            Assert.AreEqual(width, map.Width, "Incorrect Width Of map");
            Assert.AreEqual(height, map.Height, "Incorrect Height Of map");
            Assert.AreEqual(playerPos, map.Player.Position, "Incorrect start position of player");
            Assert.AreEqual(enemiesCount, map.EnemiesCount, "Incorrect count of enemies");
            Assert.AreEqual(emptyCellsCount, map.EmptyCells.Count, "There is some problems empty cells");
            Assert.AreEqual(width * height - emptyCellsCount, map.WallCells.Count, "There is some problems with walls");
        }
    }
}
