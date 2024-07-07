using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Collisions;

namespace nameless.Collisions;

public class TriggerHitbox : Collider
{
    private List<Type> TriggerByEntityTypes = new List<Type>();
    public event Action OnCollisionEvent;

    public void SetTriggerEntityTypes(params Type[] entities)
    {
        TriggerByEntityTypes.Clear();
        TriggerByEntityTypes.AddRange(entities);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (TriggerByEntityTypes.Contains(collisionInfo.Other.GetType()))
            OnCollisionEvent.Invoke();
    }
}
