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
using nameless.Engine;
using nameless.Interfaces;

namespace nameless.Collisions;

public partial class Collider : ICollisionActor
{
    [XmlIgnore]
    public IShapeF Bounds { get; protected set; }
    public ICollider Entity { get; protected set; }
    public Color Color { get; set; } = Color.Red;
    public string Id { get; protected set; }
    protected Vector2 offset = Vector2.Zero;

    public Collider() { }

    public Collider(ICollider entity, int width, int height)
    {
        entity.collider = this;
        entity.collider.SetCollision(entity,width, height);
    }

    protected virtual void SetCollision(IEntity entity, int width, int height)
    {
        Bounds = new RectangleF(entity.Position + Globals.Offset(width, height), new Size2(width, height));
        this.Entity = (ICollider)entity;
        Globals.CollisionComponent.Insert(this);
        Globals.Colliders.Add(this);
    }

    public void RemoveCollision()
    {
        Globals.CollisionComponent.Remove(this);
        Globals.Colliders.Remove(this);
    }

    public void SetOffset(Vector2 offset)
    {
        this.offset = offset;
        Bounds.Position += offset;
    }

    public void SetId(string id)
    {
        Id = id;
    }

    public virtual void OnCollision(CollisionEventArgs collisionInfo)
    {
        //if (CollisionManager.OnCollisionDisabled) return;
    }

    public void DrawCollision(SpriteBatch spriteBatch)
    {
        var rectBounds = (RectangleF)Bounds;
        //spriteBatch.DrawRectangle(new RectangleF(new Point2(rectBounds.X - rectBounds.Width / 2, rectBounds.Y - rectBounds.Height / 2), rectBounds.Size), Color.Red, 5);
        spriteBatch.DrawRectangle((RectangleF)Bounds, Color,3);
    }
}
