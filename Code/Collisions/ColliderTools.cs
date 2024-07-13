using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;

namespace nameless.Collisions;

public partial class Collider
{
    public static Side CollisionToSide(CollisionEventArgs collisionInfo)
    {
        return PenetrationVectorToSide(collisionInfo.PenetrationVector);
    }

    public static Side PenetrationVectorToSide(Vector2 penetrationVector)
    {
        if (penetrationVector.Y == 0)
        {
            if (penetrationVector.X < 0)
                return Side.Left;
            else
                return Side.Right;
        }
        else
        {
            if (penetrationVector.Y > 0)
                return Side.Bottom;
            else
                return Side.Top;
        }
    }

    public static bool IsOppositeSides(Side side1, Side side2) => (int)side1 + (int)side2 == 3; //Opposite is: Top-Bottom, Left-Right

    public static List<Side> VelocityToPossibleCollisionSides(Vector2 velocity)
    {
        var sides = new List<Side>();
        if (velocity.X != 0)
            sides.Add(PenetrationVectorToSide(new Vector2(velocity.X, 0)));
        if (velocity.Y != 0)
            sides.Add(PenetrationVectorToSide(new Vector2(0, velocity.Y)));
        return sides;
    }
}
