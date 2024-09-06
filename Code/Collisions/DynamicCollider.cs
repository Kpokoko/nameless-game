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

    public override void ActivateCollider()
    {
        Globals.CollisionManager.DynamicColliders.Add(this);
        base.ActivateCollider();
    }

    public override void RemoveCollider()
    {
        base.RemoveCollider();
        Globals.CollisionManager.DynamicColliders.Remove(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        base.OnCollision(collisionInfo);
        //Entity.OnCollision(collisionInfo);
    }

    virtual public void Update()
    {
        Position = Entity.Position;
    }
}
