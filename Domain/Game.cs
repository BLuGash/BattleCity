using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BattleCity.Domain
{
    public static class Game
    {
        public static readonly Point DefaultPoint = new Point(-1, -1);
        public static Map[] Maps = LoadLevels().ToArray();
        private static int CurrentMapIndex = 0;
        public static Map CurrentMap => Maps[CurrentMapIndex];
        public static GameStage Stage { get; private set; }
        public static event Action StageChanged;
        public static Keys keyPressed;
        public  static readonly Dictionary<Size, Direction> offsetToDirection = new Dictionary<Size, Direction>
        {
            {new Size(0, -1), Direction.Up},
            {new Size(0, 1), Direction.Down},
            {new Size(-1, 0), Direction.Left},
            {new Size(1, 0), Direction.Right}
        };

        public  static readonly Dictionary<Direction, Size> directionToOffset = new Dictionary<Direction, Size>
        {
            {Direction.Up, new Size(0, -1)},
            {Direction.Down, new Size(0, 1)},
            {Direction.Left, new Size(-1, 0)},
            {Direction.Right, new Size(1, 0)}
        };
        public static void PassLevel()
        {
            if (CurrentMapIndex == Maps.Length - 1)
                ChangeState(GameStage.Won);
            else
                CurrentMapIndex++;
        }

        public static void ChangeState(GameStage stage)
        {
            Stage = stage;
            StageChanged?.Invoke();
        }

        private static IEnumerable<Map> LoadLevels()
        {
            yield return MapCreator.CreateMapFromText(Resource.Level1);
            /*yield return MapCreator.CreateMapFromText(Resource.Level2);
            yield return MapCreator.CreateMapFromText(Resource.Level3);
            yield return MapCreator.CreateMapFromText(Resource.Level4);*/
        }
    }
}
