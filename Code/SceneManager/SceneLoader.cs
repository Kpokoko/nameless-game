using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Collisions;
using nameless.Engine;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
using nameless.Tiles;
using nameless.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace nameless.Code.SceneManager
{
    public static class SceneLoader
    {
        private static Serializer _serialize = new Serializer();
        public static List<IEntity> LoadScene(string sceneName, bool pizdec = false)
        {
            var readedData = new List<IEntity>();
            var sceneContent = new List<IEntity>();
            var path = Path.Combine("..", "net6.0", "Layout", "Levels", sceneName + ".xml");
            var rawData = _serialize.DeserializeScene(path);
            foreach (var data in rawData)
            {
                switch (data.TypeOfElement)
                {
                    case "Block":
                        sceneContent.Add(new Block((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "InventoryBlock":
                        sceneContent.Add(new InventoryBlock((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "EditorBlock":
                        sceneContent.Add(new EditorBlock((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "MovingPlatform":
                        sceneContent.Add(new MovingPlatform((int)data.TilePos.X, (int)data.TilePos.Y, data.Direction, data.Speed));
                        continue;
                    case "Platform":
                        sceneContent.Add(new Platform((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "FragileBlock":
                        sceneContent.Add(new FragileBlock((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "StickyBlock":
                        sceneContent.Add(new StickyBlock((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "TemporaryBlock":
                        sceneContent.Add(new TemporaryBlock((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "DelayedDeathBlock":
                        sceneContent.Add(new DelayedDeathBlock((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "RayCaster":
                        sceneContent.Add(new RayCaster((int)data.TilePos.X, (int)data.TilePos.Y, 64, 10, 0.1f));
                        continue;
                    default: break;
                }
            }
            foreach (var data in rawData.Where(d=>d.TypeOfElement == "Pivot"))
            {
                var pivot = new Pivot((int)data.TilePos.X, (int)data.TilePos.Y);
                HitboxTrigger.CreateHitboxTrigger(data.TriggerType, pivot, sceneContent);
                sceneContent.Add(pivot);
                continue;
            }
            var playerPos = _serialize.ReadSaveSpot();
            var vel = Vector2.Zero;
            var state = PlayerState.Falling;
            if (Globals.SceneManager.GetPlayer() != null)
            {
                vel = Globals.SceneManager.GetPlayer().InnerForce * 60;
                state = Globals.SceneManager.GetPlayer().State;
            }
            if (pizdec) return sceneContent;
            sceneContent.Add(new PlayerModel(ResourceManager.SpriteSheet, Tile.GetTileCenter(playerPos), vel, state));
            return sceneContent;
        }

        public static void SaveScene()
        {
            var entities = Globals.SceneManager.GetEntities();
            _serialize.SerializeScene(Globals.SceneManager.GetName(), entities.Select(x => x as ISerializable).ToList());
            _serialize.SaveInventory(Globals.Inventory.GetInventory());
        }

        public static void SwitchScene(HitboxTrigger trigger, SceneChangerDirection direction, Func<Vector2> playerPosition)
        {
            SaveScene();
            var currLoc = Globals.SceneManager.CurrentLocation;
            var newLoc = new Vector2(trigger.DestinationScene.X + currLoc.X, trigger.DestinationScene.Y + currLoc.Y);
            Globals.Serializer.SavePosition(newLoc);
            Globals.SceneManager.LoadScene(newLoc, new EntryData(direction, playerPosition()));
            Globals.SceneManager.SaveScene();
            var data = Globals.Serializer.ReadVisitedScenes();
            Globals.Serializer.LoadMinimap(currLoc, data, newLoc);
            //return newLoc;
        }
    }
}
