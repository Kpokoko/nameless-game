using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Interfaces;
using nameless.Entity;

namespace nameless.Collisions;

class SideCollider : Collider
{
    private MainSideCollider parent;
    public Side side { get; }
    public Vector2 offset { get; private set; }
    static private int outerSize = 1;

    public SideCollider(MainSideCollider parent, Side side) : base()
    {
        this.parent = parent;
        this.side = side;
    }

    public override void SetCollision(IEntity gameObject, int width, int height)
    {
        base.SetCollision(gameObject, width, height);

        var newBounds = (RectangleF)Bounds;
        switch (side)
        {
            case Side.Left:
                {
                    offset = new Vector2(-outerSize, 0);
                    newBounds.Width = outerSize;
                    break;
                }
            case Side.Right:
                {
                    offset = new Vector2(newBounds.Width, 0);
                    newBounds.Width = outerSize;
                    break;
                }
            case Side.Top:
                {
                    offset = new Vector2(0, -outerSize);
                    newBounds.Height = outerSize;
                    break;
                }
            case Side.Bottom:
                {
                    offset = new Vector2(0, newBounds.Height);
                    newBounds.Height = outerSize;
                    break;
                }
        }
        newBounds.Position += offset;
        Bounds = newBounds;
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (collisionInfo.Other is IEntity)
            parent.AddToBuffer(side);
    }
}
