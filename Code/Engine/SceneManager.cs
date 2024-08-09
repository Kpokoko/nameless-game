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
using nameless.Tiles;
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
    private PlayerModel _goodOlPlaye;
    public Vector2 CurrentLocation;

    public void SaveScene()
    {
        SceneLoader.SaveScene();
    }

    public void LoadScene(Vector2 currentLocation, EntryData entryData = null)
    {
        if (_currentScene != null)
            _goodOlPlaye = GetPlayer();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var scene = Globals.Map[(int)currentLocation.X, (int)currentLocation.Y];
        string sceneName;
        if (scene != null)
        {
            sceneName = scene.FullName;
        }
        else
        {
            sceneName = currentLocation.ToSimpleString();
            Globals.Map[(int)currentLocation.X, (int)currentLocation.Y] = new SceneInfo(null,currentLocation.ToPoint());
            //Map.SaveMap();
            File.Copy(Path.Combine("..", "net6.0", "Levels", ".xml"), Path.Combine("..", "net6.0", "Levels", sceneName + ".xml"));
        }
        CurrentLocation = currentLocation;
        Globals.Engine.LoadCollisions();
        _currentScene = new Scene(sceneName);
        if (entryData != null)
        {
            Globals.CanActivateSave = true;
            var tempPos = GetEntryPosition(entryData);
            GetPlayer().Position = tempPos;
            GetPlayer().Colliders.Position = tempPos;
        }
        GetPlayer().PrepareSerializationInfo();

        //var entities = _currentScene.Entities;
        //foreach (var block in entities.Where(item => item is Block))
        //{
        //    if (block is Pivot) continue;
        //    var colliderBlock = block as ICollider;
        //    var trigger2 = new HitboxTrigger(colliderBlock, 70, 70, ReactOnProperty.ReactOnEntityType, Collisions.SignalProperty.OnceOnEveryContact);
        //    trigger2.SetTriggerEntityTypes(typeof(PlayerModel));
        //    colliderBlock.Colliders.Add(trigger2);
        //    var trueColor = colliderBlock.Colliders[0].Color;
        //    trigger2.OnCollisionEvent += () =>
        //    {
        //        colliderBlock.Colliders[0].Color = Color.Blue;
        //    };
        //    trigger2.OnCollisionExitEvent += () =>
        //    {
        //        TimerTrigger.DelayEvent(500, () => { if (!trigger2.isActivated) colliderBlock.Colliders[0].Color = trueColor; });
        //    };
        //}
        Globals.Engine.LoadUtilities();
    }

    public void RemoveScene()
    {
        GetPlayer().Remove();
        _currentScene = null;
    }

    public void ReloadScene()
    {
        var sceneName = _currentScene.Name;
        _currentScene = null;
        LoadScene(CurrentLocation);
    }

    private Vector2 GetEntryPosition(EntryData entryData)
    {
        var enters = _getEnters();
        var entryPosition = entryData.PlayerPosition;

        if (!enters.Any())
        {
            enters = CreateEnterOnScene(entryData, entryPosition);
        }

        switch (entryData.Direction)
        {
            case SceneChangerDirection.top:
                entryPosition.Y = enters.Select(e => e.Y).Max() - 5;
                break;
            case SceneChangerDirection.bottom:
                entryPosition.Y = enters.Select(e => e.Y).Min() + 5;
                break;
            case SceneChangerDirection.left:
                entryPosition.X = enters.Select(e => e.X).Max() - 5;
                break;
            case SceneChangerDirection.right:
                entryPosition.X = enters.Select(e => e.X).Min() + 5;
                break;
        }
        return entryPosition;
    }

    private IEnumerable<Vector2> CreateEnterOnScene(EntryData entryData, Vector2 entryPosition)
    {
        IEnumerable<Vector2> enters;
        var newEnterPos = Tile.GetPosInTileCoordinats(entryPosition);
        enters = GetEntities()
        .Where(e => e is Block)
        .Select(e => ((Block)e).TilePosition);
        Globals.Constructor.SelectedEntity = EntityTypeEnum.Pivot;
        Globals.Constructor.SelectedEntityProperty = TriggerType.SwitchScene;
        switch (entryData.Direction)
        {
            case SceneChangerDirection.top:
                newEnterPos.Y = enters.Select(e => e.Y).Max();
                break;
            case SceneChangerDirection.bottom:
                newEnterPos.Y = enters.Select(e => e.Y).Min() + 65;
                break;
            case SceneChangerDirection.left:
                newEnterPos.X = enters.Select(e => e.X).Max();
                break;
            case SceneChangerDirection.right:
                newEnterPos.X = enters.Select(e => e.X).Min();
                break;
        }
        Globals.Constructor.SpawnBlock(newEnterPos);
        return enters.Select(e => Tile.GetTileCenter(e));
    }

    private IEnumerable<Vector2> _getEnters() => GetEntities()
            .Where(e => e is Pivot)
            .SelectMany(e => ((Pivot)e).Colliders.colliders)
            .Where(e => e is HitboxTrigger && ((HitboxTrigger)e).TriggerType is Entity.TriggerType.SwitchScene)
            .Select(e => e.Entity.Position)
            .Distinct();

    public PlayerModel GetPlayer() => _currentScene != null  ? _currentScene.Entities.Where(item => item is PlayerModel).First() as PlayerModel : _goodOlPlaye != null ? _goodOlPlaye : null;

    public Storage GetStorage() => _currentScene.Storage;

    public List<IEntity> GetEntities() => _currentScene.Entities;

    public string GetName() => _currentScene.Name;

    public void Update(GameTime gameTime) => _currentScene.Update(gameTime);

    public void Draw(SpriteBatch spriteBatch) => _currentScene.Draw(spriteBatch);

    public Scene GetScene() => _currentScene;
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