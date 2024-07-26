using Microsoft.Xna.Framework;
using nameless.Code.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity
{
    public class MovingPlatform : Block
    {
        public Vector2 Direction;
        public float Speed;
        public MovingPlatform(int x, int y, Vector2 dir, float speed) : base(x, y)
        {
            Position = new Vector2(Position.X, Position.Y - 27);
            Direction = dir;
            Speed = speed;
            Colliders.Remove(this.Colliders[0]);
            Colliders.Add(new MovingPlatformCollider(this, 64, 10));
            Colliders[0].Color = Color.Yellow;
            PrepareSerializationInfo();
        }

        public override void Update(GameTime gameTime)
        {
            Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public override void PrepareSerializationInfo()
        {
            base.PrepareSerializationInfo();
            Info.Direction = Direction;
            Info.Speed = Speed;
        }

        public void TurnAround()
        {
            Speed = Speed * (-1);
        }
    }
}
