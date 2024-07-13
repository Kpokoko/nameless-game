﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;

namespace nameless.Collisions;

partial class MyCollisionEventArgs
{
    public ICollisionActor Other { get; internal set; }

    public Vector2 PenetrationVector { get; internal set; }
    
    public MyCollisionEventArgs(ICollisionActor other, Vector2 penetrationVector)
    {
        PenetrationVector = penetrationVector;
        Other = other;
    }
}
