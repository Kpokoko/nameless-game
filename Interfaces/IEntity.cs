using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace nameless.Interfaces;

public interface IEntity : IGameObject
{
    public Vector2 Position { get; } 
    public Vector2 TilePosition { get; }
}
