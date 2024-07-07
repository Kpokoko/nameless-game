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

public partial class Collider : ICollisionActor
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
        spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red,3);
    }
}
