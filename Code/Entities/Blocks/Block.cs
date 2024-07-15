﻿using System;
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

public partial class Block : TileGridEntity, IEntity, ICollider
{
    public Block(int x, int y)
    {
        //Position = new Tile(x, y).Position;
        TilePosition = new Vector2(x, y);
        colliders.Add(new Collider(this, 64, 64));
    }

    public Block() { }
    [XmlIgnore]

    int IGameObject.DrawOrder => 1;
    [XmlIgnore]
    public Colliders colliders { get; set; } = new();

    public override void OnPositionChange(Vector2 position)
    {
        if (colliders != null)
            colliders.Position = position;
    }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    { }

    public virtual void Update(GameTime gameTime)
    { }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo)
    {
    }
}
