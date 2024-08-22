using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class Attacher : Block
{
    Vector2 Direction;
    public Vector2[] BetweenTiles;
    public Attacher(int x, int y, Vector2 dir) : base(x, y)
    {
        Colliders.RemoveAll();
        Direction = dir;

        if ((TilePosition + dir).Length() < TilePosition.Length())
            BetweenTiles = new[] {TilePosition + dir, TilePosition};
        else
            BetweenTiles = new[] {TilePosition, TilePosition + dir };

        Colliders.Add(new Collisions.Collider(this, 20, 20));
        Colliders[0].SetOffset(Direction * 32);
        Colliders[0].Color = Color.SteelBlue;
        Globals.CollisionManager.CollisionComponent.Remove(Colliders[0]);
        PrepareSerializationInfo();
    }

    public void Attach()
    {
        var storage = Globals.SceneManager.GetStorage();
        var blocks = new[] { (Block)storage[TilePosition], (Block)storage[TilePosition + Direction] };
        if (blocks[0] != null && blocks[1] != null)
        {
            blocks[0].AttachBlock(blocks[1]);
            blocks[0].AttachBlock(this);
            blocks[1].AttachBlock(blocks[0]);
            blocks[1].AttachBlock(this);
        }
    }

    public override void PrepareSerializationInfo()
    {
        base.PrepareSerializationInfo();
        Info.Direction = Direction;
    }


}
