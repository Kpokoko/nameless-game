using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Code.SceneManager;
using nameless.Collisions;
using nameless.Engine;
using nameless.Entity;
using nameless.GameObjects;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class SceneManager
{
    private Scene _currentScene;
    public Vector2 CurrentLocation;

    public void LoadScene(Vector2 currentLocation, EntryData entryData = null)
    {
        var scene = Globals.Map[(int)currentLocation.X, (int)currentLocation.Y];
        string sceneName;
        if (scene != null)
        {
            sceneName = scene.FullName;
        }
        else
        {
            sceneName = currentLocation.ToSimpleString();
            Globals.Map[(int)currentLocation.X, (int)currentLocation.Y] = new SceneInfo(sceneName,currentLocation.ToPoint());
            //Map.SaveMap();
            File.Copy(Path.Combine("..", "net6.0", "Levels", ".xml"), Path.Combine("..", "net6.0", "Levels", sceneName + ".xml"));
        }
        CurrentLocation = currentLocation;
        Globals.Engine.LoadCollisions();
        _currentScene = new Scene(sceneName);

        if (entryData != null)
        {
            GetPlayer().Position = GetEntryPosition(entryData);
            GetPlayer().PrepareSerializationInfo();
        }

        var trigger = new HitboxTrigger(new Pivot(17, 12), 80, 80, ReactOnProperty.ReactOnEntityType, Collisions.SignalProperty.OnceOnEveryContact);
        trigger.SetTriggerEntityTypes(typeof(PlayerModel));
        trigger.Color = Color.SkyBlue;
        trigger.OnCollisionEvent += () => GetPlayer().Position = new Vector2(GetPlayer().Position.X, GetPlayer().Position.Y - 60);
        var entities = _currentScene.Entities;
        foreach (var block in entities.Where(item => item is Block))
        {
            if (block is Pivot) continue;
            var colliderBlock = block as ICollider;
            var trigger2 = new HitboxTrigger(colliderBlock, 70, 70, ReactOnProperty.ReactOnEntityType, Collisions.SignalProperty.OnceOnEveryContact);
            trigger2.SetTriggerEntityTypes(typeof(PlayerModel));
            colliderBlock.Colliders.Add(trigger2);
            var trueColor = colliderBlock.Colliders[0].Color;
            trigger2.OnCollisionEvent += () =>
            {
                colliderBlock.Colliders[0].Color = Color.Blue;
            };
            trigger2.OnCollisionExitEvent += () =>
            {
                TimerTrigger.DelayEvent(500, () => { if (!trigger2.isActivated) colliderBlock.Colliders[0].Color = trueColor; });
            };
        }
        Globals.Engine.LoadUtilities();
    }

    public void ReloadScene()
    {
        var sceneName = _currentScene.Name;
        _currentScene = null;
        LoadScene(CurrentLocation);
    }

    private Vector2 GetEntryPosition(EntryData entryData)
    {
        var enters = GetEntities()
            .Where(e => e is Pivot)
            .SelectMany(e => ((Pivot)e).Colliders.colliders)
            .Where(e => e is HitboxTrigger && ((HitboxTrigger)e).TriggerType is Entitiy.TriggerType.SwitchScene)
            .Select(e => e.Entity.Position)
            .Distinct();
        var entryPosition = entryData.PlayerPosition;

        switch (entryData.Direction)
        {
            case SceneChangerDirection.top:
                entryPosition.Y = enters.Select(e => e.Y).Max() - 5;
                break;
            case SceneChangerDirection.bottom:
                entryPosition.Y = enters.Select(e => e.Y).Min() + 5;
                break;
            case SceneChangerDirection.left:
                entryPosition.X = enters.Select(e => e.X).Max() ;
                break;
            case SceneChangerDirection.right:
                entryPosition.X = enters.Select(e => e.X).Min() ;
                break;
        }
        return entryPosition;
    }

    public PlayerModel GetPlayer() => _currentScene.Entities.Where(item => item is PlayerModel).First() as PlayerModel;

    public Storage GetStorage() => _currentScene.Storage;

    public List<IEntity> GetEntities() => _currentScene.Entities;

    public string GetName() => _currentScene.Name;

    public void Update(GameTime gameTime) => _currentScene.Update(gameTime);

    public void Draw(SpriteBatch spriteBatch) => _currentScene.Draw(spriteBatch);
}

public class EntryData
{
    public readonly SceneChangerDirection Direction;
    public readonly Vector2 PlayerPosition;

    //public static Vector2 EntryPosition;

    public EntryData(SceneChangerDirection direction, Vector2 playerPosition)
    {
        this.Direction = direction;
        this.PlayerPosition = playerPosition;
    }
}