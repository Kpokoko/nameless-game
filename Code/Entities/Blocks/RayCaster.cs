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
    public RayCaster(int x, int y, int width, int height, float speed) : base(x, y)
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
            CastedRay = new Ray((int)TilePosition.X + 1, (int)TilePosition.Y, 64, 10, null, this);
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
        Info.Direction = Direction;
    }
}
