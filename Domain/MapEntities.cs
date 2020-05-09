using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleCity.Domain
{
    public class MapEntities
    {
        public readonly HashSet<ITank> Tanks;
        public readonly HashSet<Enemy> ActiveEnemies;
        public readonly HashSet<IBullet> BulletsFlying;
        public readonly HashSet<Dog> Dogs;

        public MapEntities()
        {
            Tanks = new HashSet<ITank>();
            ActiveEnemies = new HashSet<Enemy>();
            BulletsFlying = new HashSet<IBullet>();
            Dogs = new HashSet<Dog>();
        }
    }
}
