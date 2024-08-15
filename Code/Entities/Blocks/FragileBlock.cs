﻿using nameless.Collisions;
using nameless.Entity;
using nameless.GameObjects;
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
            var trigger = new HitboxTrigger(this, 65, 65, ReactOnProperty.ReactOnEntityType, Collisions.SignalProperty.OnceOnEveryContact);
            trigger.SetTriggerEntityTypes(typeof(PlayerModel), typeof(MovingPlatform));
            trigger.OnCollisionExitEvent += () =>
            { TimerTrigger.DelayEvent(300, () => { if (!trigger.isActivated) Break();});
            };
            Colliders.Add(trigger);
            Colliders[0].Color = Color.Purple;
            IsEnableToPlayer = true;
        }
        public FragileBlock(Vector2 tilePosition) : this((int)tilePosition.X, (int)tilePosition.Y) { }


        public void Break()
        {
            IsEnableToPlayer = false;
            Colliders.RemoveAll();
        }
    }
}
