using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;
using nameless.Interfaces;

namespace nameless.Collisions;
public class CharacterCollider : DynamicCollider
{
    private List<CollisionEventArgs> collisionInfoBuffer = new List<CollisionEventArgs>();

    public static void SetCollider(ICollider entity, int width, int height)
    {
        entity.collider = new CharacterCollider();
        entity.collider.SetCollision(entity, width, height);
    }

    public override void SetCollision(IEntity gameObject, int width, int height)
    {
        Globals.CharacterColliders.Add(this);
        base.SetCollision(gameObject, width, height);
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
            if (IsPairSide(CollisionToSide(collisionInfoBuffer[0]), CollisionToSide(collisionInfoBuffer[1])))
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
            Globals.CollisionComponent.Update(Globals.GameTime);
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
        var Position = entity.GetType().GetProperty("Position");
        Position.SetValue(entity, (Vector2)Position.GetValue(entity) - penetrationVector);

        entity.OnCollision(collisionsInfo);
    }

    public sealed override void OnCollision(CollisionEventArgs collisionInfo)
    {
        base.OnCollision(collisionInfo);

        //if (collisionInfo.Other is not IEntity) return;
        AddToBuffer(collisionInfo);
    }
}
