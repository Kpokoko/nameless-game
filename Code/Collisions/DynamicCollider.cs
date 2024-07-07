using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Interfaces;

namespace nameless.Collisions;

public class DynamicCollider : Collider
{
    public override void SetCollision(IEntity gameObject, int width, int height)
    {
        Globals.DynamicColliders.Add(this);
        base.SetCollision(gameObject, width, height);
    }

    virtual public void Update()
    {
        var rectBounds = (RectangleF)Bounds;
        Bounds.Position = entity.Position + Globals.Offset((int)rectBounds.Width, (int)rectBounds.Height);
    }
}
