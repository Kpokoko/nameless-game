using Microsoft.Xna.Framework;
using nameless.Code.SceneManager;
using nameless.Collisions;
using nameless.Engine;
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
            case TriggerType.DamagePlayer:
                trigger = CreateDamagePlayerTrigger(pivot);
                break;
            case TriggerType.Disposable:
                trigger = CreateDisposableTrigger(pivot);
                break;
            case TriggerType.Saver:
                trigger = CreateSaverTrigger(pivot);
                break;
            default:
                throw new NotImplementedException();
        }
        if (trigger == null)
            return null;
        trigger.TriggerType = type;
        pivot.Colliders.Add(trigger);
        pivot.PrepareSerializationInfo();
        return trigger;
    }

    public static HitboxTrigger CreateSaverTrigger(Block pivot)
    {
        var trigger = new HitboxTrigger(pivot, 96, 96, ReactOnProperty.ReactOnEntityType, SignalProperty.Once);
        var triggerInfo = new SerializationInfo() { TilePos = pivot.TilePosition };
        trigger.TriggerType = TriggerType.Disposable;
        trigger.OnCollisionEvent += () =>
        {
            if (!Globals.CanActivateSave) return;
            Globals.CanActivateSave = false;
            //pivot.PrepareSerializationInfo();
            Globals.LastVisitedCheckpoint = triggerInfo;
            Globals.Serializer.WriteSaveSpot();
        };
        trigger.Color = Color.DarkViolet;
        return trigger;
    }

    public static HitboxTrigger CreateDisposableTrigger(Block pivot)
    {
        var trigger = new HitboxTrigger(pivot, 10, 10, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceAtGame);
        trigger.TriggerType = TriggerType.Disposable;
        trigger.OnCollisionEvent += () =>
        {
            Globals.Constructor.DeleteBlock(pivot);
        };
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
        trigger.SetTriggerEntityTypes(typeof(PlayerModel));
        trigger.Color = Color.SkyBlue;
        trigger.OnCollisionEvent += () =>  SceneLoader.SwitchScene(trigger, direction, playerPosition);
        return trigger;
    }

    public static HitboxTrigger CreateDamagePlayerTrigger(Block pivot)
    {
        HitboxTrigger trigger = new HitboxTrigger(pivot, 60, 60, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact,QuantityProperty.OneAtATime);
        var player = () => Globals.SceneManager.GetPlayer();

        trigger.Color = Color.IndianRed;
        trigger.OnCollisionEvent += () => { if (Globals.IsNoclipEnabled) return; Globals.SceneManager.ReloadScene(); };
        return trigger;
    }

}
