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

namespace nameless.Entity;

public class Block : IEntity, ICollider
{
    public Block(int x, int y)
    {
        Position = new Tile(x, y).Position;
        TilePosition = new Vector2(x, y);
        Collider.SetCollider(this, 64, 64);
    }

    public Block() { }
    [XmlIgnore]
    public Vector2 Position { get; set; }
    public Vector2 TilePosition { get; set; }
    int IGameObject.DrawOrder => 1;

    public Collider collider { get; set; }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    { }

    public void Update(GameTime gameTime)
    { }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo)
    {
    }
}
