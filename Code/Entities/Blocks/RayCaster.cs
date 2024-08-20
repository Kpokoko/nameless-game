using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Code.SceneManager;
using nameless.Collisions;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class RayCaster : Block
{
    public Ray CastedRay;
    public Storage Storage;
    private bool _nextTileIsEmpty;
    private Vector2 Direction;
    public RayCaster(int x, int y, Vector2 dir) : base(x, y)
    {
        Colliders[0].Color = Color.Pink;
        Direction = dir;
        PrepareSerializationInfo();
    }

    public override void Update(GameTime gameTime)
    {
        if (Storage is null)
            Storage = Globals.SceneManager.GetStorage();
        if (CastedRay == null || CastedRay.IsNeedToBeDeleted)
        {
            if (Direction.Y != 0)
                CastedRay = new Ray((int)(TilePosition.X + Direction.X), (int)(TilePosition.Y + Direction.Y), 10, 64, null, this, Direction);
            else
                CastedRay = new Ray((int)(TilePosition.X + Direction.X), (int)(TilePosition.Y + Direction.Y), 64, 10, null, this, Direction);
        }
        CastedRay.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (CastedRay != null)
            CastedRay.Draw(spriteBatch);
    }

    public void SetMovement(Vector2 dir)
    {
        Direction = dir.NormalizedCopy();
        PrepareSerializationInfo();
    }

    public override void PrepareSerializationInfo()
    {
        base.PrepareSerializationInfo();
        Info.Direction = this.Direction;
    }
}