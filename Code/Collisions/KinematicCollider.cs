using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Engine;
using nameless.Interfaces;

namespace nameless.Collisions;
public class KinematicCollider : DynamicCollider
{
    private List<CollisionEventArgs> collisionInfoBuffer = new();

    public KinematicCollider(IKinematic entity, int width, int height) : base(entity, width, height)
    {
    }

    public override void ActivateCollider()
    {
        base.ActivateCollider();
        Globals.CollisionManager.KinematicColliders.Add(this);
    }

    public override void RemoveCollider()
    {
        base.RemoveCollider();
        Globals.CollisionManager.KinematicColliders.Remove(this);
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

    private void AddToBuffer(CollisionEventArgs collisionInfo)
    {
       collisionInfoBuffer.Add(collisionInfo);
        var a = collisionInfo.PenetrationVector.Length;
    }

    private void ClearBuffer() 
    {
        collisionInfoBuffer.Clear();
    }

    private CollisionEventArgs[] AnalyseCollisions()
    {
        var collisionsInfo = collisionInfoBuffer.ToArray();

        if (collisionsInfo.Length == 1)
            return collisionsInfo;

        //if (collisionsInfo.Length > 2)
        collisionsInfo = collisionsInfo
            .OrderByDescending(info => info.PenetrationVector.Length())
            .DistinctBy(info => CollisionToSide(info))
            .ToArray();

        if (collisionsInfo.Length == 2)
        {
            if (IsOppositeSides(CollisionToSide(collisionInfoBuffer[0]), CollisionToSide(collisionInfoBuffer[1])))
                return collisionsInfo;
            return GetRealCollision(collisionsInfo);
        }
        return collisionsInfo;
    }

    private CollisionEventArgs[] GetRealCollision(CollisionEventArgs[] collisionsInfo)
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

    public virtual void OnCollision(CollisionEventArgs[] collisionsInfo) 
    {
        var penetrationVector = collisionsInfo.Select(collisionInfo => collisionInfo.PenetrationVector).Aggregate((a, b) => a + b);
        var Position = Entity.GetType().GetProperty("Position");
        Position.SetValue(Entity, (Vector2)Position.GetValue(Entity) - penetrationVector);

        Entity.OnCollision(collisionsInfo);
    }

    public sealed override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (CollisionManager.OnCollisionDisabled && !this.Equals(CollisionManager.Processing)) return;

        if (collisionInfo.Other is HitboxTrigger) return;

        AddToBuffer(collisionInfo);
    }
}
