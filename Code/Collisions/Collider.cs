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
using nameless.Entity;
using nameless.Interfaces;

namespace nameless.Collisions;

public partial class Collider : ICollisionActor
{
    [XmlIgnore]
    public IShapeF Bounds { get; protected set; }
    public bool IsNeedToDraw = true;
    public Vector2 Position {
        get { return _position;}
        set { 
            _position = value;
            var rect = (RectangleF)Bounds;
            Bounds.Position = _position + Globals.Offset((int)rect.Width,(int)rect.Height) + offset;
        } }
    private Vector2 _position;
    public ICollider Entity { get; protected set; }
    public Color Color { get; set; } = Color.Red;
    public string Id { get; protected set; }
    protected Vector2 offset = Vector2.Zero;

    public Collider() { }

    public Collider(ICollider entity, int width, int height)
    {
        //entity.collider = this;
        SetCollider(entity,width, height);
    }

    protected virtual void SetCollider(IEntity entity, int width, int height)
    {
        Bounds = new RectangleF(Vector2.Zero, new Size2(width, height));
        Position = entity.Position;
        this.Entity = (ICollider)entity;
        ActivateCollider();
    }

    public virtual void ActivateCollider()
    {
        Globals.CollisionManager.CollisionComponent.Insert(this);
        Globals.CollisionManager.Colliders.Add(this);
    }

    public virtual void RemoveCollider()
    {
        Globals.CollisionManager.CollisionComponent.Remove(this);
        Globals.CollisionManager.Colliders.Remove(this);
    }

    public void SetOffset(Vector2 offset)
    {
        this.offset = offset;
        Position = Position;
    }

    public void SetId(string id)
    {
        Id = id;
    }

    public virtual void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (CollisionManager.OnCollisionDisabled) return;
            Entity.OnCollision(collisionInfo);
    }

    public void DrawCollision(SpriteBatch spriteBatch)
    {
        if (!IsNeedToDraw) return;
        var rectBounds = (RectangleF)Bounds;
        //spriteBatch.DrawRectangle(new RectangleF(new Point2(rectBounds.X - rectBounds.Width / 2, rectBounds.Y - rectBounds.Height / 2), rectBounds.Size), Color.Red, 5);
        spriteBatch.DrawRectangle((RectangleF)Bounds, Color,4);
        spriteBatch.FillRectangle((RectangleF)Bounds, Color * 0.3f);
    }
}
