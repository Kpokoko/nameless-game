using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Collisions;

namespace nameless.Collisions;

public partial class Collider
{
    public static Side CollisionToSide(CollisionEventArgs collisionInfo)
    {
        if (collisionInfo.PenetrationVector.Y == 0)
        {
            if (collisionInfo.PenetrationVector.X < 0)
                return Side.Left;
            else
                return Side.Right;
        }
        else
        {
            if (collisionInfo.PenetrationVector.Y > 0)
                return Side.Bottom;
            else
                return Side.Top;
        }
    }

    public static bool IsPairSide(Side side1, Side side2) => (int)side1 + (int)side2 == 3; //Pair is: Top-Bottom, Left-Right
}
