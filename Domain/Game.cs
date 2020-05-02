using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace BattleCity.Domain
{
    public static class Game
    {
        public static readonly Point DefaultPoint = new Point(-1, -1);
        public static Map Map { get; private set; }

        public static void ChangeMap(Map map)
        {
            Map = map;
        }
    }
}
