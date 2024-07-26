using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using nameless.Code.Collisions;
using nameless.Collisions;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.Entities
{
    public class Platform : Block, ICollider
    {
        public Platform(int x, int y)
        {
            TilePosition = new Vector2(x, y);
            Colliders.Add(new PlatformCollider(this, 64, 10));
            Colliders[0].Color = Color.Green;
        }

        public override Vector2 TilePosition { get => base.TilePosition;
            set { base.TilePosition = value;  Position = new Vector2(Position.X, Position.Y - 27); }  }
    }
}
