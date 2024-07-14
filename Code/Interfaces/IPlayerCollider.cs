using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Collisions;
using MonoGame.Extended.Collisions;

namespace nameless.Interfaces;

public interface IPlayerCollider : IEntity
{
    Collider collider { get; set; }

    public void OnCollision(params MyCollisionEventArgs[] collisionsInfo);
}
