using Microsoft.Xna.Framework;
using nameless.Collisions;
using nameless.Entitiy;
using nameless.Entity;
using nameless.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Collisions;

public partial class HitboxTrigger
{
    public static HitboxTrigger CreateHitboxTrigger(TriggerType type, Block pivot)
    {
        HitboxTrigger trigger = null;
        switch (type)
        {
            case TriggerType.None:
                break;
            case TriggerType.SwitchScene:
                trigger = SwitchScene(pivot);
                break;
            default:
                throw new NotImplementedException();
        }
        if (trigger == null)
            return null;
        pivot.Colliders.Add(trigger);
        pivot.PrepareSerializationInfo();
        return trigger;
    }

    public static HitboxTrigger SwitchScene(Block pivot)
    {
        var trigger = new HitboxTrigger(pivot, 10, 64, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact);
        trigger.TriggerType = TriggerType.SwitchScene;
        trigger.SetTriggerEntityTypes(typeof(PlayerModel));
        trigger.Color = Color.SkyBlue;
        trigger.SetOffset(new Vector2(30, 0));
        trigger.OnCollisionEvent += () =>
        {
            var serializer = new Serializer();
            var entities = Globals.SceneManager.GetEntities();
            serializer.Serialize(Globals.SceneManager.GetName(), entities.Select(x => x as ISerializable).ToList());
            Globals.Engine.Restart();
        };
        return trigger;
    }
}
