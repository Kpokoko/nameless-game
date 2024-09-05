using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
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
    public Vector2 Direction;
    public Collider SpaceChecker;
    public List<float> Distances;
    public RayCaster(int x, int y, Vector2 dir) : base(x, y)
    {
        var width = 23 * 64 * Math.Abs((int)dir.X) + 1;
        var height = 13 * 64 * Math.Abs((int)dir.Y) + 1;
        SpaceChecker = new Collider(this, width, height);
        //if (dir.X >= 0 && dir.Y >= 0)
            SpaceChecker.Position = new Vector2(this.Position.X + 23 * 32 * (int)dir.X, this.Position.Y + 13 * 32 * (int)dir.Y);
        if (dir.X < 0 || dir.Y < 0)
        {
            
        }
        //else
        //{
        //    if (dir.Y < 0)
        //    {
        //        SpaceChecker.Position = new Vector2(SpaceChecker.Position.X, 0);
        //        SpaceChecker.Bounds.Position = new Vector2(SpaceChecker.Position.X, 0);
        //    }
        //    else
        //    {
        //        SpaceChecker.Position = new Vector2(0, SpaceChecker.Position.Y);
        //        SpaceChecker.Bounds.Position = new Vector2(0, SpaceChecker.Position.Y);
        //    }
        //}
        //if (dir.Y < 0)
        //    SpaceChecker.Position
        //        = new Vector2(this.Position.X + 23 * 32 * Math.Abs((int)dir.X), this.Position.Y + 13 * 32 * Math.Abs((int)dir.Y) - SpaceChecker.Position.Y);
        //if (dir.X < 0)
        //    SpaceChecker.Position
        //        = new Vector2(this.Position.X + 23 * 32 * Math.Abs((int)dir.X) - SpaceChecker.Position.X, this.Position.Y + 13 * 32 * Math.Abs((int)dir.Y));
        SpaceChecker.SetId("LaserChecker");
        SpaceChecker.IsNeedToDraw = false;
        CastedRay = new Ray(x, y, 10, 10, this, dir);
        Colliders[0].Color = Color.Pink;
        Direction = dir;
        PrepareSerializationInfo();
        Distances = new List<float>();
    }

    public override void Update(GameTime gameTime)
    {
        CastedRay.Update(gameTime);
        Distances.Clear();
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

    public override void OnCollision(params CollisionEventArgs[] collisionsInfo)
    {
        var ev = collisionsInfo[0];
        if ((ev.Other as Collider).Entity is RayCaster || (ev.Other as Collider).Entity is Ray)
            return;
        if (this.Colliders.colliders.Count == 0) return;
        if (ev.Other.Bounds.Intersects((this.Colliders.colliders[0]).Bounds) && !((ev.Other as Collider).Entity is MovingBlock))
            return;
        var distance = ev.Other.Bounds.Position - this.Position;
        var length = Direction.X != 0 ? distance.X : distance.Y;
        Distances.Add(length);
        base.OnCollision(collisionsInfo);
    }
}