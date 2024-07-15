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
using nameless.Entity;
using nameless.Interfaces;
using nameless.Tiles;

namespace nameless.Entities.Blocks;

/// <summary>
/// Invisible entity.
/// </summary>
internal class Pivot :  TileGridEntity, IEntity, ICollider
{
    int IGameObject.DrawOrder => 1;
    [XmlIgnore]
    public Colliders colliders { get; set; }

    public Pivot(int x, int y)
    {
        TilePosition = new Vector2(x, y);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    { }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo)
    {
        throw new InvalidOperationException();
    }

    public void Update(GameTime gameTime)
    { }

    public override void OnPositionChange(Vector2 position)
    { }
}
