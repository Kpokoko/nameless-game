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
using MonoGame.Extended;

namespace nameless.Entity;

public class Block : TileGridEntity, IEntity, ICollider, ISerializable, IKinematic
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
    public virtual Vector2 Velocity {  get; set; }

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
    {
        if (AttachedBlocks == null || !Globals.SceneManager.GetStorage().AttachedMovers.ContainsKey(this))
            return;

        var movingBlock = Globals.SceneManager.GetStorage().AttachedMovers[this];
        if (movingBlock.Collided)
            return;

        var collisionInfo = collisionsInfo[0];


        if (collisionInfo.Other is HitboxTrigger || collisionInfo.Other is KinematicAccurateCollider)
            return;
        //if (collisionInfo.PenetrationVector.NormalizedCopy() != movingBlock.Direction * movingBlock.Speed / Math.Abs(movingBlock.Speed) || collisionInfo.PenetrationVector.Length() < 1e-03)
        //    return;
        if (Collider.PenetrationVectorToSide(collisionInfo.PenetrationVector) != Collider.PenetrationVectorToSide(movingBlock.Direction * movingBlock.Speed) || collisionInfo.PenetrationVector.Length() < 1e-03)
            return;

        movingBlock.Collided = true;
        movingBlock.CollisionShift -= collisionInfo.PenetrationVector * 2;
    }

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
    public HashSet<Block> AttachedBlocks { get; set; }

    public void AttachBlock(Block block)
    {
        if (AttachedBlocks == null)
            AttachedBlocks = new HashSet<Block>();
        if (AttachedBlocks.Contains(block)) return;
        AttachedBlocks.Add(block);
    }

    public void DetachBlock(Block block)
    {
        if (AttachedBlocks == null)
            return;
        if (AttachedBlocks.Contains(block))
            AttachedBlocks.Remove(block);
    }

    public SerializationInfo Info { get; set; } = new();


    public virtual void PrepareSerializationInfo()
    {
        Info.TilePos = TilePosition;
        Info.TypeOfElement = this.GetType().Name;
    }
}
