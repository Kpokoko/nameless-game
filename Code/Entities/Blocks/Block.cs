using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using nameless.Serialize;
using nameless.Collisions;
using nameless.Interfaces;
using nameless.Tiles;
using System.Runtime.CompilerServices;

namespace nameless.Entity;

public class Block : TileGridEntity, IEntity, ICollider, ISerialization
{
    public Block(int x, int y)
    {
        //Position = new Tile(x, y).Position;
        Info = new SerializationInfo();
        TilePosition = new Vector2(x, y);
        Info.TilePos = TilePosition;
        Info.TypeOfElement = this.GetType().Name;
        colliders.Add( new Collider(this, 64, 64));
    }

    public Block() { }
    [XmlIgnore]

    int IGameObject.DrawOrder => 1;
    [XmlIgnore]
    public Colliders colliders { get; set; } = new();
    public SerializationInfo Info { get; set; } = new();

    public override void OnPositionChange(Vector2 position)
    {
        if (colliders != null)
            colliders.Position = position;
        Info.TilePos = TilePosition;
    }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    { }

    public virtual void Update(GameTime gameTime)
    { }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo)
    { }
}
