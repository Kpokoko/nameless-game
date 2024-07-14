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

        if (collisionsInfo.Length == 1)
            return collisionsInfo;

        //if (collisionsInfo.Length > 2)
        collisionsInfo = collisionsInfo
            .OrderByDescending(info => info.PenetrationVector.Length())
            .DistinctBy(info => info.CollisionSide)
            .ToArray();

        if (collisionsInfo.Length == 2)
        {
            if (IsOppositeSides(collisionInfoBuffer[0].CollisionSide, collisionInfoBuffer[1].CollisionSide))
                return collisionsInfo;
            return GetRealCollision(collisionsInfo);
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

    public virtual void OnCollision(MyCollisionEventArgs[] collisionsInfo) 
    {
        var penetrationVector = collisionsInfo.Select(collisionInfo => collisionInfo.PenetrationVector).Aggregate((a, b) => a + b);
        var Position = Entity.GetType().GetProperty("Position");
        Position.SetValue(Entity, (Vector2)Position.GetValue(Entity) - penetrationVector);

        ((PlayerModel)Entity).OnCollision(collisionsInfo);
    }

    public sealed override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (CollisionManager.OnCollisionDisabled && !this.Equals(CollisionManager.Processing)) return;

        if (collisionInfo.Other is HitboxTrigger) return;

        var kinematicCollisionInfo = TraceCollisionBasedOnKinematicVelocity(collisionInfo);

        AddToBuffer(kinematicCollisionInfo);
    }

    private MyCollisionEventArgs TraceCollisionBasedOnKinematicVelocity(CollisionEventArgs collisionInfo)
    {
        var vel = ((IKinematic)Entity).Velocity;
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
        return tracedCollisionInfo;
    }
}
