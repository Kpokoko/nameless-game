using nameless.Collisions;
using nameless.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity
{
    public class TemporaryBlock : Block
    {
        public TemporaryBlock(int x, int y) : base(x, y)
        {
            var trigger = new TimerTriggerAutoStart(3000, GameObjects.SignalProperty.Once);
            trigger.OnTimeoutEvent += () => this.Break();
            trigger.Start();
            Colliders[0].Color = Color.DarkSeaGreen;
        }
        public TemporaryBlock(Vector2 tilePosition) : this((int)tilePosition.X, (int)tilePosition.Y) { }


        public void Break()
        {
            IsEnableToPlayer = false;
            Colliders.RemoveAll();
        }
    }
}
