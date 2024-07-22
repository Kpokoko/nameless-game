﻿using System;
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

public partial class Block : TileGridEntity, IEntity, ICollider
{
    public Block(int x, int y)
    {
        //Position = new Tile(x, y).Position;
        TilePosition = new Vector2(x, y);
        Colliders.Add( new Collider(this, 64, 64));
        Colliders[0].Color = Color.Brown;
    }

    public Block() { }
    [XmlIgnore]

    int IGameObject.DrawOrder => 1;
    [XmlIgnore]
    public Colliders Colliders { get; set; } = new();

    public override void OnPositionChange(Vector2 position)
    {
        if (Colliders != null)
            Colliders.Position = position;
        PrepareSerializationInfo();
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    { }

    public virtual void Update(GameTime gameTime)
    { }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo)
    { }

    public SceneChangerLocation GetBlockPlace()
    {
        //var width = Globals.Engine.Window.ClientBounds.Width;
        //var height = Globals.Engine.Window.ClientBounds.Height;
        var a = this.TilePosition.X;
        var b = this.TilePosition.Y;
        if (a == 0) return SceneChangerLocation.left;
        if (b == 0) return SceneChangerLocation.top;
        if (a > b) return SceneChangerLocation.right;
        return SceneChangerLocation.bottom;
    }
}
