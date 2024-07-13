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
    public DynamicCollider(ICollider entity, int width, int height) : base(entity, width, height)
    {
    }

    protected override void SetCollider(IEntity gameObject, int width, int height)
    {
        Globals.CollisionManager.DynamicColliders.Add(this);
        base.SetCollider(gameObject, width, height);
    }

    public override void RemoveCollider()
    {
        base.RemoveCollider();
        Globals.CollisionManager.DynamicColliders.Remove(this);
    }

    virtual public void Update()
    {
        Position = Entity.Position;
    }
}
