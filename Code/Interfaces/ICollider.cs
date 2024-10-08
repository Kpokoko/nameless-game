﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Collisions;
using MonoGame.Extended.Collisions;

namespace nameless.Interfaces;

public interface ICollider : IEntity
{
    Colliders Colliders { get; set; }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo);
}
