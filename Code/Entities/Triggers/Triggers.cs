using Microsoft.Xna.Framework;
using nameless.Collisions;
using nameless.Engine;
using nameless.Entitiy;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
using nameless.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Collisions;

public partial class HitboxTrigger
{
    public Vector2 DestinationScene;
    public static HitboxTrigger CreateHitboxTrigger(TriggerType type, Block pivot, List<IEntity> sceneContent = null)
    {
        HitboxTrigger trigger = null;
        switch (type)
        {
            case TriggerType.None:
                break;
            case TriggerType.SwitchScene:
                trigger = CreateSwitchSceneTrigger(pivot,sceneContent);
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

    public static HitboxTrigger CreateSwitchSceneTrigger(Block pivot, List<IEntity> sceneContent)
    {
        HitboxTrigger trigger = null;
        var playerPosition = () => Globals.SceneManager.GetPlayer().Position;
        var direction = pivot.GetBlockDirection(sceneContent); // need update

        if (direction is SceneChangerDirection.top)
        {
            trigger = new HitboxTrigger(pivot, 64, 10, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact, QuantityProperty.OneAtATime);
            trigger.DestinationScene = new Vector2(0, -1);
            trigger.SetOffset(new Vector2(0, -30));
        }
        else if (direction is SceneChangerDirection.bottom)
        {
            trigger = new HitboxTrigger(pivot, 64, 10, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact, QuantityProperty.OneAtATime);
            trigger.DestinationScene = new Vector2(0, 1);
            trigger.SetOffset(new Vector2(0, 30));
        }
        else
            trigger = new HitboxTrigger(pivot, 10, 64, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact, QuantityProperty.OneAtATime);
        if (direction is SceneChangerDirection.left)
        {
            trigger.SetOffset(new Vector2(-30, 0));
            trigger.DestinationScene = new Vector2(-1, 0);

        }
        if (direction is SceneChangerDirection.right)
        {
            trigger.SetOffset(new Vector2(30, 0));
            trigger.DestinationScene = new Vector2(1, 0);
        }
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
            Globals.SceneManager.LoadScene(Globals.Map[(int)newLoc.X,(int)newLoc.Y], newLoc, new EntryData(direction, playerPosition()));
            //Globals.Engine.Restart();
        };
        return trigger;
    }
}
