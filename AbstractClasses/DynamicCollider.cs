using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using nameless.Interfaces;

namespace nameless.AbstractClasses;

public class DynamicCollider : Collider
{
    public override void SetCollision(IEntity gameObject, int width, int height)
    {
        Globals.DynamicColliders.Add(this);
        base.SetCollision(gameObject, width, height);
    }

    public void Update()
    {
        var rectBounds = (RectangleF)Bounds;
        Bounds.Position = gameObject.Position + Globals.Offset((int)rectBounds.Width, (int)rectBounds.Height);
    }
}
