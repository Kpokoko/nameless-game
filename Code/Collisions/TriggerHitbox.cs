using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Collisions;
using nameless.Entity;
using nameless.Interfaces;
using Microsoft.Xna.Framework;


namespace nameless.Collisions;

public class TriggerHitbox : Collider
{
    private List<Type> triggerByEntityTypes = new();
    private List<string> triggerByCollidersId = new();
    private ReactProperty reactProperty;
    private Vector2 offset;

    public TriggerHitbox(ICollider entity, int width, int height, ReactProperty reactOn) : base(entity, width, height)
    {
        Color = Color.DarkGoldenrod;
        reactProperty = reactOn;
    }

    public event Action OnCollisionEvent;
    
    public void SetTriggerEntityTypes(params Type[] entities)
    {
        triggerByEntityTypes.Clear();
        triggerByEntityTypes.AddRange(entities);
    }

    public void SetTriggerHitboxesId(params string[] id)
    {
        triggerByCollidersId.Clear();
        triggerByCollidersId.AddRange(id);
    }

    public sealed override void OnCollision(CollisionEventArgs collisionInfo)
    {
        switch (reactProperty)
        {
            case ReactProperty.ReactOnEntityType:
                if (collisionInfo.Other is TriggerHitbox) return;
                if (triggerByEntityTypes.Contains(((Collider)collisionInfo.Other).Entity.GetType()))
                    if (OnCollisionEvent != null)
                        OnCollisionEvent.Invoke();
                break;

            case ReactProperty.ReactOnColliderId:
                if (collisionInfo.Other is not TriggerHitbox) return;
                if (triggerByCollidersId.Contains(((TriggerHitbox)collisionInfo.Other).Id))
                    if (OnCollisionEvent != null)
                        OnCollisionEvent.Invoke();
                break;

        }
    }
}
