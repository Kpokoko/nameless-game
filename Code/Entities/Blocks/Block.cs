using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Code.Entities;
using nameless.Collisions;
using nameless.Interfaces;
using nameless.Tiles;

namespace nameless.Entity;

public class Block : EntityCollider
{
    public Collider Collider { get; set; }
    public Block(int x, int y)
    {
        Position = new Tile(x, y).Position;
        TilePosition = new Vector2(x, y);
        Collider = new Collider();
        Collider.SetCollision(this, 64, 64);
    }

    public Block() { }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    { }

    public void Update(GameTime gameTime)
    { }
}
