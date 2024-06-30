using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Interfaces;

namespace nameless.AbstractClasses;

public class Collider : ICollisionActor
{
    public IShapeF Bounds { get; private set; }
    protected IEntity gameObject;

    public virtual void SetCollision(IEntity gameObject, int width, int height)
    {
        Bounds = new RectangleF(gameObject.Position, new Size2(width, height));
        this.gameObject = gameObject;
        Globals.CollisionComponent.Insert(this);
        Globals.Colliders.Add(this);
    }

    public virtual void OnCollision(CollisionEventArgs collisionInfo)
    {
        Console.WriteLine(collisionInfo.Other.ToString());
    }

    public void DrawCollision(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red,5);
    }
}
