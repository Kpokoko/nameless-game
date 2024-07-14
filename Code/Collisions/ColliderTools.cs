using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
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

    public static Vector2 SideToPenetrationVector(Side side)
    {
        switch (side)
        {
            case Side.Left:
                return new Vector2(-1, 0);
            case Side.Right:
                return new Vector2(1, 0);
            case Side.Top:
                return new Vector2(0,-1);
            case Side.Bottom:
                return new Vector2(0, 1);
            default: throw new ArgumentException();
        }
    }

    public static bool IsOppositeSides(Side side1, Side side2) => (int)side1 + (int)side2 == 3;

    public static Side GetOppositeSide(Side side) => (Side)(3 - (int)side);

    public static List<Side> VelocityToPossibleCollisionSides(Vector2 velocity)
    {
        var sides = new List<Side>();
        if (velocity.X != 0)
            sides.Add(PenetrationVectorToSide(new Vector2(velocity.X, 0)));
        if (velocity.Y != 0)
            sides.Add(PenetrationVectorToSide(new Vector2(0, velocity.Y)));
        return sides;
    }

    public static float GetBoundsBorderPosition(RectangleF rect, Side borderSide)
    {
        var type = rect.GetType();
        return (float)type.GetProperty(borderSide.ToString()).GetValue(rect, null);
    }

    public static Vector2 GetDistanceBetweenBorders(float borderPos,float otherBorderPos,Side intersectionSide)
    {
        switch (intersectionSide)
        {
            case Side.Left:
                return new Vector2(otherBorderPos - borderPos, 0);
            case Side.Right:
                return new Vector2(otherBorderPos - borderPos,0);
            case Side.Top:
                return new Vector2(0,otherBorderPos - borderPos);
            case Side.Bottom:
                return new Vector2(0,otherBorderPos - borderPos);
            default: throw new ArgumentException();
        }
    }

    public static Vector2 TransformVectorToMatchAxisLength(Vector2 vector, Vector2 axisVector)
    {
        if (axisVector.Y == 0)
        {
            return (vector / vector.X) * axisVector.X;
        }
        if (axisVector.X == 0)
        {
            return (vector / vector.Y) * axisVector.Y;
        }
        throw new ArgumentException("{0} Is not axis alligned vector",axisVector.ToString());
    }
}
