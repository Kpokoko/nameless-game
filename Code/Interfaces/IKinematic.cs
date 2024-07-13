using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Interfaces;

public interface IKinematic : ICollider
{
    public Vector2 Velocity { get; }
}
