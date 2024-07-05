using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Interfaces;

namespace nameless.Collisions;

public class Collider : ICollisionActor
{
    [XmlIgnore]
    public IShapeF Bounds { get; protected set; }
    protected IEntity entity;

    public virtual void SetCollision(IEntity entity, int width, int height)
    {
        Bounds = new RectangleF(entity.Position + Globals.Offset(width, height), new Size2(width, height));
        this.entity = entity;
        Globals.CollisionComponent.Insert(this);
        Globals.Colliders.Add(this);
    }

    public virtual void OnCollision(CollisionEventArgs collisionInfo)
    {
        //Console.WriteLine(collisionInfo.Other.ToString());
    }

    public void DrawCollision(SpriteBatch spriteBatch)
    {
        var rectBounds = (RectangleF)Bounds;
        //spriteBatch.DrawRectangle(new RectangleF(new Point2(rectBounds.X - rectBounds.Width / 2, rectBounds.Y - rectBounds.Height / 2), rectBounds.Size), Color.Red, 5);
        spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red,5);
    }


    //Static functions


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
