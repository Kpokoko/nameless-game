using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Code.SceneManager;
using nameless.Collisions;
using nameless.GameObjects;
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
        private Vector2 _previousPosition;

        //private Vector2 _collisionShift;

        private bool Collided = false;
        public bool Blocked = false;
        public bool Static = false;
        private List<MovingPlatform> AttachedMovingPlatforms;
        private Storage _storage;
        public MovingPlatform(int x, int y, Vector2 dir, float speed) : base(x, y)
        {
            //Position = new Vector2(Position.X, Position.Y - 27);
            Direction = dir;
            Speed = speed;
            Colliders.Remove(this.Colliders[0]);
            Colliders.Add(new DynamicCollider(this, 64, 64));
            Colliders[0].Color = Color.Goldenrod;
            PrepareSerializationInfo();

            _previousPosition = Position;
        }
        public MovingPlatform(Vector2 tilePosition, Vector2 dir, float speed) : this((int)tilePosition.X, (int)tilePosition.Y, dir, speed) { }


        public void SetMovement(Vector2 dir, float speed)
        {
            Direction = dir.NormalizedCopy();
            Speed = speed;
            PrepareSerializationInfo();
        }

        public override void OnPositionChange(Vector2 position)
        {
            //base.OnPositionChange(position);
        }


        private void MoveAttachedBlocks(HashSet<Block> blocks, Vector2 vec)
        {
            if (blocks == null || vec == Vector2.Zero)
                return;
            foreach (var block in blocks)
            {
                if (block.Velocity == Vector2.Zero)
                {
                    //block.Position -= block.Velocity;

                    block.Velocity = vec;
                    block.Position += vec;
                    MoveAttachedBlocks(block.AttachedBlocks, vec);
                }
            }
        }
        private void FreezeAttachedBlocks(HashSet<Block> blocks)
        {
            if (blocks == null)
                return;
            foreach (var block in blocks)
            {
                if (block.Velocity != Vector2.Zero)
                {
                    block.Velocity = Vector2.Zero;
                    FreezeAttachedBlocks(block.AttachedBlocks);
                }
            }
        }

        public override void UpdateConstructor()
        {
            base.UpdateConstructor();
            if (Colliders != null)
                Colliders.Position = Position;
            PrepareSerializationInfo();
        }

        public override void Update(GameTime gameTime)
        {
            if (Static)
                return;
            //Position += _collisionShift;
            //_collisionShift = Vector2.Zero;

            Velocity = Vector2.Zero;
            Blocked = false;
            if (_storage == null)
                _storage = Globals.SceneManager.GetStorage();
            var movingTo = (Direction * Speed).NormalizedCopy();
            movingTo = new Vector2((float)Math.Round(movingTo.X),(float)Math.Round(movingTo.Y));
            if (!_storage.IsTileFree(movingTo + TilePosition) && !_storage.IsTileFree(TilePosition - movingTo))
                Blocked = true;
            if (Blocked)
                return;
            
            FreezeAttachedBlocks(AttachedBlocks);

            var vel = Direction * Speed * 60f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += vel;
            //if (!_attachedBlocks.Any() && Globals.SceneManager.CurrentLocation == new Vector2(3,-1) && TilePosition == new Vector2(12,4))
            //{
            //    AttachBlock((Block)(Globals.SceneManager.GetStorage()[11, 4, 0]));
            //    AttachBlock((Block)(Globals.SceneManager.GetStorage()[13, 4, 0]));
            //}//only on test scene
            Velocity +=  Position - _previousPosition;
            MoveAttachedBlocks(AttachedBlocks, Position - _previousPosition);
            Collided = false;

            var rnd = new Random();
            var rndVec = () => new Vector2((float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1);
            for (int i = 0; i < 1; i++)
            {
                var p = new BlendingParticle(Position, Vector2.Zero, 700, Color.DarkGoldenrod * 0.08f);//, rndVec() * 0.01f);
                p.SetSecondColor(Color.Transparent);
            }

            _previousPosition = Position;
        }

        public override void OnCollision(params CollisionEventArgs[] collisionsInfo)
        {
            if (Collided)
                return;
            base.OnCollision(collisionsInfo);
            if (collisionsInfo.Select(i => i.Other).Any(o => o is HitboxTrigger || o is KinematicAccurateCollider))
                return;
            TurnAround();
            //_collisionShift -= collisionsInfo.MaxBy(i => i.PenetrationVector.Length()).PenetrationVector * 2;
            if (!Static)
                Position -= collisionsInfo.MaxBy(i => i.PenetrationVector.Length()).PenetrationVector * 2;

            Collided = true;
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
            Globals.AudioManager.PlaySound(Sound.SoundType.Click, 0.0f);

            var rnd = new Random();
            var rndSpeed = rnd.NextDouble() * 2 + 0.5;
            var dirVec = new Vector2(1, 0);
            //Globals.AnimationManager.PlayAnimation(Globals.AnimationManager.Animations.Keys.ToArray()[rnd.NextInt64(Globals.AnimationManager.Animations.Keys.Count-1)], Position, (float)rndSpeed);

            var rndVec = () => new Vector2((float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1);
            for (int i = 0; i < 50; i++)
            {
                var p = new BlendingParticle(Position, dirVec * 2, 500, Color.Goldenrod * 0.07f);
                p.SetSecondColor(Color.Transparent);
                dirVec = dirVec.Rotate((float)Math.PI / 25);
            }
        }
    }
}
