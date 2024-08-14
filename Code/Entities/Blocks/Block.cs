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

public partial class Block : TileGridEntity, IEntity, ICollider
{
    public Block(int x, int y)
    {
        //Position = new Tile(x, y).Position;
        TilePosition = new Vector2(x, y);
        Colliders.Add( new Collider(this, 64, 64));
        Colliders[0].Color = Color.Brown;
    }

    public Block(Vector2 tilePosition) : this((int)tilePosition.X, (int)tilePosition.Y) { }

    public Block() { }

    int IGameObject.DrawOrder => 1;
    public Colliders Colliders { get; set; } = new();

    public override void OnPositionChange(Vector2 position)
    {
        if (Colliders != null)
            Colliders.Position = position;
        PrepareSerializationInfo();
    }

    private bool _isSelected;
    public override bool IsSelected
    {
        get { return _isSelected; }
        set 
        {
            _isSelected = value;
            if (_isSelected)
            {
                if (!Globals.UIManager.Selected.Contains(this))
                    Globals.UIManager.Selected.Add(this);
            }
            else
                Globals.UIManager.Selected.Remove(this);
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    { }

    public virtual void Update(GameTime gameTime)
    { }

    public virtual void OnCollision(params CollisionEventArgs[] collisionsInfo)
    { }

    public SceneChangerDirection GetBlockDirection(List<IEntity> sceneContent)
    {
        var entities = sceneContent.Select(e=>e.TilePosition);
        var leftBorder = entities.Select(v=>v.X).Min();
        var rightBorder = entities.Select(v => v.X).Max();
        var topBorder = entities.Select(v => v.Y).Min();
        var bottomBorder = entities.Select(v => v.Y).Max();
        if (TilePosition.X == leftBorder) return SceneChangerDirection.left;
        if (TilePosition.Y == topBorder) return SceneChangerDirection.top;
        if (TilePosition.Y == bottomBorder) return SceneChangerDirection.bottom;
        return SceneChangerDirection.right;
    }


    public virtual void Remove()
    {
        Colliders.RemoveAll();
    }
}
