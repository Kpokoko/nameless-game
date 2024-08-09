using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;

namespace nameless.Collisions;

public class MyCollisionEventArgs
{
    public ICollisionActor Other { get; internal set; }

    public Vector2 PenetrationVector { get; internal set; }

    public Side CollisionSide { get; internal set; }

    public bool OtherIsDynamic { get; internal set; } = false;
    public MyCollisionEventArgs(ICollisionActor other, Vector2 penetrationVector, Side side, bool otherIsDynamic = false)
    {
        PenetrationVector = penetrationVector;
        Other = other;
        CollisionSide = side;
        OtherIsDynamic = otherIsDynamic;
     }

    public MyCollisionEventArgs(CollisionEventArgs collisionInfo)
    {
        PenetrationVector = collisionInfo.PenetrationVector;
        Other = collisionInfo.Other;
        CollisionSide = Collider.CollisionToSide(collisionInfo);
    }
}
