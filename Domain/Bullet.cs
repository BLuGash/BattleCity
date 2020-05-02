using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BattleCity.Domain
{
    public class Bullet : IBullet
    {
        private readonly Map map;
        public Direction Direction { get; }
        public int Damage { get; }

        public Point Position { get; private set; }

        public ITank Sender { get; }

        public Bullet(int damage, Point position, Direction direction, ITank sender, Map map)
        {
            Damage = damage;
            Position = position;
            Direction = direction;
            Sender = sender;
            this.map = map;
        }

        public void Move()
        {
            Position += Direction.ConvertToVector();
        }

        public void CheckForCollisions()
        {
            if (ThereIsTankToDamage())
            {
                GetEnemyNearby().Damage(Damage);
                map.DestroyBullet(this);
            }
            else if (WallAhead() || !(Position + Direction.ConvertToVector()).InsideMap(map))
                map.DestroyBullet(this);
        }

        public ITank GetEnemyNearby()
        {
            var enemy = map.Tanks
                .Where(tank => tank != Sender && tank.Position == Position)
                .FirstOrDefault();
            if (enemy == default(ITank))
                throw new ArgumentException("There is no damaged enemies");
            return enemy;
        }

        public bool WallAhead()
        {
            return map.WallCells.Contains(Position + Direction.ConvertToVector());
        }

        public bool ThereIsTankToDamage()
        {
            return map.Tanks
                .Where(enemy => enemy != Sender)
                .Any(enemy => enemy.Position == Position);
        }
    }
}
