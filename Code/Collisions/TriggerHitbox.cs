using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Collisions;
using nameless.Entity;
using nameless.Interfaces;
using Microsoft.Xna.Framework;
using nameless.Engine;

namespace nameless.Collisions;

public class TriggerHitbox : Collider
{
    private List<Type> triggerByEntityTypes = new();
    private List<Type> EntityBuffer = new();
    private List<string> triggerByCollidersId = new();
    private List<string> idBiffer = new();
    private ReactOnProperty reactOnProperty;
    private ActivateProperty activateProperty;
    private SignalProperty signalProperty;
    private bool isActivated = false;
    private bool triggerInside = false;

    public TriggerHitbox(ICollider entity, int width, int height, 
        ReactOnProperty reactOn, SignalProperty signal) : base(entity, width, height)
    {
        Color = Color.DarkGoldenrod;
        reactOnProperty = reactOn;
        signalProperty = signal;
        if (signalProperty is SignalProperty.OnceOnEveryContact) 
            Globals.TriggerHitboxes.Add(this);
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

    public void AddToEntityBuffer(params Type[] entities)
    {
        triggerByEntityTypes.Clear();
        triggerByEntityTypes.AddRange(entities);
    }

    public void UpdateActivation()
    {
        if (triggerInside) triggerInside = false;
        else isActivated = false;
    }

    public sealed override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (CollisionManager.OnCollisionDisabled) return;
        if (isActivated && signalProperty is SignalProperty.Once) return;

        switch (reactOnProperty)
        {
            case ReactOnProperty.ReactOnEntityType:
                if (collisionInfo.Other is TriggerHitbox) return;
                if (triggerByEntityTypes.Contains(((Collider)collisionInfo.Other).Entity.GetType()))
                {
                    TryInvoke();
                }
                break;

            case ReactOnProperty.ReactOnColliderId:
                if (collisionInfo.Other is not TriggerHitbox) return;
                if (triggerByCollidersId.Contains(((TriggerHitbox)collisionInfo.Other).Id))
                {
                    TryInvoke();
                }
                break;

        }
    }

    private void TryInvoke()
    {
        triggerInside = true;

        if (isActivated && signalProperty is SignalProperty.OnceOnEveryContact) return;

        if (OnCollisionEvent != null)
            OnCollisionEvent.Invoke();
        isActivated = true;
    }
}
