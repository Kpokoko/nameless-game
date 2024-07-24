using Microsoft.Xna.Framework;
using nameless.Collisions;
using nameless.Engine;
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
    public Vector2 DestinationScene;
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
        HitboxTrigger trigger = null;
        var location = pivot.GetBlockPlace();
        if (location is SceneChangerLocation.top)
        {
            trigger = new HitboxTrigger(pivot, 64, 10, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact);
            trigger.DestinationScene = new Vector2(0, -1);
            trigger.SetOffset(new Vector2(0, -30));
        }
        else if (location is SceneChangerLocation.bottom)
        {
            trigger = new HitboxTrigger(pivot, 64, 10, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact);
            trigger.DestinationScene = new Vector2(0, 1);
            trigger.SetOffset(new Vector2(0, 30));
        }
        else
            trigger = new HitboxTrigger(pivot, 10, 64, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact);
        if (location is SceneChangerLocation.left)
        {
            trigger.SetOffset(new Vector2(-30, 0));
            trigger.DestinationScene = new Vector2(-1, 0);
        }
        if (location is SceneChangerLocation.right)
        {
            trigger.SetOffset(new Vector2(30, 0));
            trigger.DestinationScene = new Vector2(1, 0);
        }
        //if (Globals.SceneManager.GetName() != "center") trigger.DestinationScene = "center";
        trigger.TriggerType = TriggerType.SwitchScene;
        trigger.SetTriggerEntityTypes(typeof(PlayerModel));
        trigger.Color = Color.SkyBlue;
        trigger.OnCollisionEvent += () =>
        {
            var serializer = new Serializer();
            var entities = Globals.SceneManager.GetEntities();
            serializer.Serialize(Globals.SceneManager.GetName(), entities.Select(x => x as ISerializable).ToList());
            var currLoc = Globals.SceneManager.CurrentLocation;
            var newLoc = new Vector2(trigger.DestinationScene.X + currLoc.X, trigger.DestinationScene.Y + currLoc.Y);
            Globals.SceneManager.LoadScene(Globals.Map[(int)newLoc.X][(int)newLoc.Y], newLoc);
            //Globals.Engine.Restart();
        };
        return trigger;
    }
}
