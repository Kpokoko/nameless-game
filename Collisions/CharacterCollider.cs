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
            ClearCollision();
        }
    }

    private void ClearCollision() 
    {
        collisionInfoBuffer.Clear();
    }

    private CollisionEventArgs[] AnalyseCollisions()
    {
        var collisionsInfo = collisionInfoBuffer.ToArray();

        if (collisionInfoBuffer.Count == 1)
            return collisionsInfo;
        if (collisionInfoBuffer.Count == 2)
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
            ClearCollision();
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

    private bool IsPairSide(Side side1, Side side2) => (int)side1 + (int)side2 == 3; //Pair is: Top-Bottom, Left-Right

    public virtual void OnCollision(CollisionEventArgs[] collisionsInfo) { }

    public sealed override void OnCollision(CollisionEventArgs collisionInfo)
    {
        base.OnCollision(collisionInfo);

        if (collisionInfo.Other is not IEntity) return;
        collisionInfoBuffer.Add(collisionInfo);
    }
}
