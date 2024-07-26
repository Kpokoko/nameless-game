using MonoGame.Extended.Collisions;
using nameless.Collisions;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.Collisions
{
    public class MovingPlatformCollider : Collider
    {
        public MovingPlatformCollider(ICollider entity, int width, int height) : base(entity, width, height)
        {
        }

        public void Update()
        {
            Position = Entity.Position;
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is HitboxTrigger || collisionInfo.Other is KinematicAccurateCollider)
                return;
            ((MovingPlatform)Entity).TurnAround();
            base.OnCollision(collisionInfo);
        }
    }
}
