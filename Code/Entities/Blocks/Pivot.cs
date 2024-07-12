using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using nameless.Collisions;
using nameless.Interfaces;
using nameless.Tiles;

namespace nameless.Entities.Blocks;

/// <summary>
/// Invisible entity.
/// </summary>
internal class Pivot : IEntity, ICollider
{
    [XmlIgnore]
    public Vector2 Position { get; set; }
    public Vector2 TilePosition { get; set; }
    int IGameObject.DrawOrder => 1;
    [XmlIgnore]
    public Collider collider { get; set; }

    public Pivot(int x, int y)
    {
        Position = new Tile(x, y).Position;
        TilePosition = new Vector2(x, y);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
    }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo)
    {
        throw new InvalidOperationException();
    }

    public void Update(GameTime gameTime)
    {
    }
}
