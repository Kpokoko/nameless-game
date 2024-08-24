using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class Attacher : SlimBlock
{
    public Attacher(int x, int y, Vector2 dir) : base(x, y, dir)
    {
        Colliders.RemoveAll();

        Colliders.Add(new Collisions.Collider(this, 20, 20));
        Colliders[0].SetOffset(Direction * 32);
        Colliders[0].Color = Color.SteelBlue;
        Globals.CollisionManager.CollisionComponent.Remove(Colliders[0]);
    }

    public Attacher(Vector2 block1, Vector2 block2) : this((int)block1.X, (int)block1.Y, block2 - block1) { }

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
            AttachBlock(blocks[0]);
            AttachBlock(blocks[1]);
        }
    }
    public void Detach()
    {
        var storage = Globals.SceneManager.GetStorage();
        var blocks = new[] { (Block)storage[TilePosition], (Block)storage[TilePosition + Direction] };
        if (blocks[0] != null && blocks[1] != null)
        {
            blocks[0].DetachBlock(blocks[1]);
            blocks[0].DetachBlock(this);
            blocks[1].DetachBlock(blocks[0]);
            blocks[1].DetachBlock(this);
            DetachBlock(blocks[0]);
            DetachBlock(blocks[1]);
        }
    }


    public override void Remove()
    {
        base.Remove();
        Detach();
    }
}
