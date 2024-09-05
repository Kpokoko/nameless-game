using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Engine;
using nameless.Entity;
using nameless.Interfaces;

namespace nameless.Collisions;
public class KinematicAccurateCollider : DynamicCollider
{
    private List<MyCollisionEventArgs> collisionInfoBuffer = new();

    public KinematicAccurateCollider(IKinematic entity, int width, int height) : base(entity, width, height)
    {
    }

    protected override void SetCollider(IEntity gameObject, int width, int height)
    {
        Globals.CollisionManager.KinematicAccurateColliders.Add(this);
        base.SetCollider(gameObject, width, height);
    }

    public override void RemoveCollider()
    {
        base.RemoveCollider();
        Globals.CollisionManager.KinematicAccurateColliders.Remove(this);
    }

    public void UpdateCollision()
    {
        if (collisionInfoBuffer.Count > 0)
        {
            var info = AnalyseCollisions();
            OnCollision(info);
            ClearBuffer();
        }
    }

    private void AddToBuffer(MyCollisionEventArgs collisionInfo)
    {
       collisionInfoBuffer.Add(collisionInfo);
    }

    private void ClearBuffer() 
    {
        collisionInfoBuffer.Clear();
    }

    private MyCollisionEventArgs[] AnalyseCollisions()
    {
        var collisionsInfo = collisionInfoBuffer.ToArray();

        var m = collisionsInfo.MaxBy(c=>c.PenetrationVector.X);

        if (collisionsInfo.Length == 1)
            return collisionsInfo;

        //if (collisionsInfo.Length > 2)
        collisionsInfo = collisionsInfo
            .OrderByDescending(info => info.PenetrationVector.Length())
            .ThenBy(info => Math.Abs(Position.X - ((RectangleF)info.Other.Bounds).Center.X))
            .DistinctBy(info => info.CollisionSide)
            .ToArray();

        if (collisionsInfo.Length == 2)
        {
            if (IsOppositeSides(collisionInfoBuffer[0].CollisionSide, collisionInfoBuffer[1].CollisionSide))
                return collisionsInfo;
            return GetRealCollision(collisionsInfo);
        }
        if (collisionsInfo.Where(i => !i.OtherIsDynamic).Count() == 2)
        {
            return GetRealDynamicCollision(collisionsInfo);
        }
        return collisionsInfo;
    }

    private MyCollisionEventArgs[] GetRealCollision(MyCollisionEventArgs[] collisionsInfo)
    {
        var actualPosition = Bounds.Position;
        for (var i = 0; i < collisionsInfo.Length; i++)
        {
            ClearBuffer();
            var collisionInfo = collisionsInfo[i];
            Bounds.Position = actualPosition - collisionInfo.PenetrationVector;
            Globals.CollisionManager.CollisionComponent.Update(Globals.GameTime);

            if (collisionInfoBuffer.Count == 0)
            {
                Bounds.Position = actualPosition;
                return new[] { collisionsInfo[i] };
            }
        }
        Bounds.Position = actualPosition;
        return collisionsInfo;
    }

    private MyCollisionEventArgs[] GetRealDynamicCollision(MyCollisionEventArgs[] collisionsInfo)
    {
        var actualPosition = Bounds.Position;
        var dynamicCollisionInfo = collisionsInfo.Where(i => i.OtherIsDynamic).ToArray();
        var staticCollisionsInfo = collisionsInfo.Where(i => !i.OtherIsDynamic).ToArray();
        for (var i = 0; i < staticCollisionsInfo.Length; i++)
        {
            ClearBuffer();
            var collisionInfo = staticCollisionsInfo[i];
            Bounds.Position = actualPosition - collisionInfo.PenetrationVector;
            Globals.CollisionManager.CollisionComponent.Update(Globals.GameTime);

            if (collisionInfoBuffer.Where(i => !i.OtherIsDynamic).Count() == 0)
            {
                Bounds.Position = actualPosition;
                return (new[] { staticCollisionsInfo[i] }.Concat(dynamicCollisionInfo)).ToArray();
            }
        }
        Bounds.Position = actualPosition;
        return collisionsInfo;
    }


    public virtual void OnCollision(MyCollisionEventArgs[] collisionsInfo) 
    {
        if (collisionsInfo.Any(i => i.OtherIsDynamic)) 
        {
            collisionsInfo = collisionsInfo
                .Where(i => !(i.OtherIsDynamic && i.CollisionSide == Side.Top && Globals.SceneManager.GetStorage().AttachedMovers.ContainsKey((i.Other as Collider).Entity as Block) && Globals.SceneManager.GetStorage().AttachedMovers[(i.Other as Collider).Entity as Block].Collided))
                .ToArray();
            if (collisionsInfo.Length == 0)
                return;
        }
        var penetrationVector = collisionsInfo.Select(collisionInfo => collisionInfo.PenetrationVector).Aggregate((a, b) => a + b);
        var Position = Entity.GetType().GetProperty("Position");
        Position.SetValue(Entity, (Vector2)Position.GetValue(Entity) - penetrationVector);

        Update();
        ((PlayerModel)Entity).OnCollision(collisionsInfo);
    }

    public sealed override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (CollisionManager.OnCollisionDisabled && !this.Equals(CollisionManager.Processing)) return;

        if (collisionInfo.Other is HitboxTrigger) return;
        
        if ((collisionInfo.Other as Collider).Id == "LaserChecker") return;

        var kinematicCollisionInfo = TraceCollisionBasedOnKinematicVelocity(collisionInfo);

        AddToBuffer(kinematicCollisionInfo);
    }

    private MyCollisionEventArgs TraceCollisionBasedOnKinematicVelocity(CollisionEventArgs collisionInfo)
    {
        bool isDynamic = false;
        var vel = ((IKinematic)Entity).Velocity;

        IKinematic other = null;
        if (((Collider)collisionInfo.Other).Entity is IKinematic)
            other = (IKinematic)((Collider)collisionInfo.Other).Entity;
        if (other != null && other.Velocity != Vector2.Zero)
        {
            isDynamic = true;
            var player = (PlayerModel)Entity;


            if (player.PullingForce != other.Velocity)
            {
                //    vel -= ((MovingPlatform)(((Collider)collisionInfo.Other).Entity)).Velocity;

                if (player.PullingForce != Vector2.Zero)
                    vel += player.PullingForce;
                //else
                vel -= other.Velocity;
            }
        }
        else
        {
            vel += ((PlayerModel)Entity).OuterForce;
        }
        if (vel == Vector2.Zero)
        {
            return new MyCollisionEventArgs(collisionInfo);
            //vel = new Vector2(0, 1);
        }
        var possibleSides = VelocityToPossibleCollisionSides(vel);
        var collidingBorderPositions = possibleSides.Select(s => GetBoundsBorderPosition((RectangleF)Bounds, s)).ToArray();
        var otherCollidingBorderPositions = possibleSides.Select(s => GetBoundsBorderPosition((RectangleF)collisionInfo.Other.Bounds, GetOppositeSide(s))).ToArray();
        var axisVectorShiftForProperIntersection = possibleSides.Select((s, i) =>
            GetDistanceBetweenBorders(collidingBorderPositions[i], otherCollidingBorderPositions[i], s));
        var shiftVectorForProperIntersection = axisVectorShiftForProperIntersection.Select((a) =>
            TransformVectorToMatchAxisLength(vel, a));
        var possibleCollisionInfos = shiftVectorForProperIntersection.Select((v, i) => new MyCollisionEventArgs(collisionInfo.Other, -v, possibleSides[i]));
        var tracedCollisionInfo = possibleCollisionInfos.MinBy(i => i.PenetrationVector.Length());

        if (tracedCollisionInfo.CollisionSide is Side.Top || tracedCollisionInfo.CollisionSide is Side.Bottom)
        {
            tracedCollisionInfo = new MyCollisionEventArgs(tracedCollisionInfo.Other, tracedCollisionInfo.PenetrationVector.SetX(0),tracedCollisionInfo.CollisionSide);
        }
        else
        {
            tracedCollisionInfo = new MyCollisionEventArgs(tracedCollisionInfo.Other, tracedCollisionInfo.PenetrationVector.SetY(0), tracedCollisionInfo.CollisionSide);
        }
        tracedCollisionInfo.OtherIsDynamic = isDynamic;
        return tracedCollisionInfo;
    }
}
