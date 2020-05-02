using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BattleCity.Domain
{
    public class Tank : ITank
    {
        private readonly Map map;
        public int HealthPoints { get; private set; }
        public Direction Direction { get; private set; }
        public Point Position { get; private set; }

        public Tank(Point position, Direction direction, int HP, Map map)
        {
            Position = position;
            Direction = direction;
            this.map = map;
            map.Tanks.Add(this);
            HealthPoints = HP;
        }

        public void Rotate(Direction direction)
        {
            Direction = direction;
        }

        public void MoveTo(Direction direction)
        {
            Rotate(direction);
            if ((Position + direction.ConvertToVector()).CanMoveTo(map))
                Position += direction.ConvertToVector();
        }

        public void Shoot()
        {
            map.BulletsFlying.Add(new Bullet(2, Position, Direction, this, map));
        }

        public void Damage(int HP)
        {
            HealthPoints -= HP;
            if (HealthPoints <= 0)
                map.DestroyTank(this);
        }
    }
}
