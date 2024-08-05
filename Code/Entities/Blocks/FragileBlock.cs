using nameless.Collisions;
using nameless.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.Entities.Blocks
{
    public class FragileBlock : Block
    {
        public FragileBlock(int x, int y) : base(x, y)
        {
            var trigger = new HitboxTrigger(this, 70, 70, ReactOnProperty.ReactOnEntityType, SignalProperty.Continuous);
            trigger.OnCollisionExitEvent += () =>
            { Globals.Constructor.DeleteBlock(this); };
            Colliders.Add(trigger);
            Colliders[0].Color = Color.Purple;
        }
    }
}
