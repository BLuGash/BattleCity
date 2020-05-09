using System;
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
        private int timePerPenalty = 4;


        public Tank(Point position, Direction direction, int HP, Map map)
        {
            Position = position;
            Direction = direction;
            this.map = map;
            HealthPoints = HP;
        }

        public void Rotate(Direction direction)
        {
            Direction = direction;
        }

        public void MoveTo(Direction direction)
        {
            Rotate(direction);
            var nextPos = Position + direction.ConvertToVector();
            if (nextPos.CanMoveTo(map))
                Position = nextPos;
        }

        public void Shoot()
        {
            if (timePerPenalty == 4)
            {
                map.Entities.BulletsFlying.Add(new Bullet(2, Position, Direction, this, map));
                timePerPenalty = 0;
            }
        }

        public void Damage(int HP)
        {
            HealthPoints -= HP;
            if (HealthPoints <= 0)
                map.DestroyTank(this);
        }

        public void WaitForShootPenalty()
        {
            timePerPenalty = timePerPenalty == 4 ? timePerPenalty : timePerPenalty + 1;
        }
    }
}
