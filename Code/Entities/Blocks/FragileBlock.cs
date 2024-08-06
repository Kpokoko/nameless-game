using nameless.Collisions;
using nameless.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity
{
    public class FragileBlock : Block
    {
        public FragileBlock(int x, int y) : base(x, y)
        {
            var trigger = new HitboxTrigger(this, 64, 64, ReactOnProperty.ReactOnEntityType, SignalProperty.Continuous);
            trigger.OnCollisionExitEvent += () =>
            { Break(); };
            Colliders.Add(trigger);
            Colliders[0].Color = Color.Purple;
            IsEnableToPlayer = true;
        }

        public void Break()
        {
            IsEnableToPlayer = false;
            Colliders.RemoveAll();
        }
    }
}
