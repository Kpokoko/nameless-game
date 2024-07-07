using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Collisions;
using MonoGame.Extended.Collisions;

namespace nameless.Interfaces;

public interface ICollider : IEntity
{
    Collider collider { get; set; }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo);
}
