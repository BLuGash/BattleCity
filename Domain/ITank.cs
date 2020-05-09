using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleCity.Domain
{
    public interface ITank
    {
        Point Position { get; }
        int HealthPoints { get; }
        Direction Direction { get; }
        void MoveTo(Direction direction);
        void Shoot();
        void Rotate(Direction direction);
        void Damage(int HP);
        void WaitForShootPenalty();
    }
}
