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
    public class MovingBlock : Block
    {
        public Vector2 Direction;
        public float Speed;
        private Vector2 _previousPosition;

        public Vector2 CollisionShift;

        public bool Collided = false;
        public bool Blocked = false;
        public bool Static = false;
        private List<MovingBlock> AttachedMovingPlatforms;
        private Storage _storage;
        public MovingBlock(int x, int y, Vector2 dir, float speed) : base(x, y)
        {
            //Position = new Vector2(Position.X, Position.Y - 27);
            Direction = dir;
            Speed = speed;
            Colliders.Remove(this.Colliders[0]);
            Colliders.Add(new DynamicCollider(this, 64, 64));
            Colliders[0].Color = Color.Goldenrod;
            PrepareSerializationInfo();
            IsDeleted = true;
            _previousPosition = Position;
        }
        public MovingBlock(Vector2 tilePosition, Vector2 dir, float speed) : this((int)tilePosition.X, (int)tilePosition.Y, dir, speed) { }


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
            if (IsOutOfBounds())
            {
                Globals.SceneManager.GetScene().ToRemove.Add(this);
                //Remove();
                return;
            }
            UpdateVisuals();

            if (Collided)
                TurnAround();
            Collided = false;
            if (Static)
                return;
            Position += CollisionShift;
            CollisionShift = Vector2.Zero;

            UpdatePhysics(gameTime);
        }

        private bool IsOutOfBounds()
        {
            return (Position.X < 64 * -2 || Position.X > Storage.StorageWidth * 64 + 2 * 64 ||
                Position.Y < 64 * -2 || Position.Y > Storage.StorageHeight * 64 + 2 * 64);
        }

        private void UpdatePhysics(GameTime gameTime)
        {
            Velocity = Vector2.Zero;

            if (Blocked)
                return;
                       
            FreezeAttachedBlocks(AttachedBlocks);

            var vel = Direction * Speed * 60f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += vel;
            Velocity +=  Position - _previousPosition;
            MoveAttachedBlocks(AttachedBlocks, Position - _previousPosition);

            _previousPosition = Position;
        }

        public void UpdateVisuals()
        {
            var rnd = new Random();
            var rndVec = () => new Vector2((float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1);
            for (int i = 0; i < 1; i++)
            {
                var p = new BlendingParticle(Position, Vector2.Zero, 700, Color.DarkGoldenrod * 0.08f);//, rndVec() * 0.01f);
                p.SetSecondColor(Color.Transparent);
            }
        }

        public void IsBlocked()
        {
            Blocked = false;
            if (_storage == null)
                _storage = Globals.SceneManager.GetStorage();
            var movingTo = (Direction * Speed).NormalizedCopy();
            movingTo = new Vector2((float)Math.Round(movingTo.X),(float)Math.Round(movingTo.Y));
            if (!_storage.IsTileFree(movingTo + TilePosition) && !_storage.IsTileFree(TilePosition - movingTo))
                Blocked = true;
        }

        public override void OnCollision(params CollisionEventArgs[] collisionsInfo)
        {
            base.OnCollision(collisionsInfo);
            if (Static)
                return;
            if (Collided)
                return;

            var collisionInfo = collisionsInfo[0];

            if ((collisionInfo.Other as Collider).Id == "LaserChecker") return;

            if (collisionInfo.Other is HitboxTrigger || collisionInfo.Other is KinematicAccurateCollider)
                return;
            //if (collisionInfo.PenetrationVector.NormalizedCopy() != Direction * Speed / Math.Abs(Speed) || collisionInfo.PenetrationVector.Length() < 1e-03)
            //    return;
            if (Collider.PenetrationVectorToSide(collisionInfo.PenetrationVector) != Collider.PenetrationVectorToSide(Direction * Speed) || collisionInfo.PenetrationVector.Length() < 1e-03)
                return;


            //_collisionShift -= collisionsInfo.MaxBy(i => i.PenetrationVector.Length()).PenetrationVector * 2;
            CollisionShift -= collisionInfo.PenetrationVector * 2;
            //if (!Static)
            //    Position -= collisionInfo.PenetrationVector * 2;
                //Position -= collisionsInfo.MaxBy(i => i.PenetrationVector.Length()).PenetrationVector * 2;

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

            //var rndVec = () => new Vector2((float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1);
            for (int i = 0; i < 50; i++)
            {
                var p = new BlendingParticle(Position, dirVec * 2, 500, Color.Goldenrod * 0.07f);
                p.SetSecondColor(Color.Transparent);
                dirVec = dirVec.Rotate((float)Math.PI / 25);
            }
        }
    }
}
