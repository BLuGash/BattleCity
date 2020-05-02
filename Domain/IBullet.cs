using System;
using System.Collections.Generic;
using System.Drawing;

namespace BattleCity.Domain
{
    public interface IBullet
    {
        Point Position { get; }
        Direction Direction { get; }
        int Damage { get; }
        bool WallAhead();
        void Move();
        ITank GetEnemyNearby();
        bool ThereIsTankToDamage();
    }
}
