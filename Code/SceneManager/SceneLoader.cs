using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Collisions;
using nameless.Engine;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
using nameless.Tiles;
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
        public static List<IEntity> LoadScene(string sceneName)
        {
            var readedData = new List<IEntity>();
            var sceneContent = new List<IEntity>();
            var path = Path.Combine("..", "net6.0", "Levels", sceneName + ".xml");
            var rawData = _serialize.Deserialize(path);
            foreach (var data in rawData)
            {
                switch (data.TypeOfElement)
                {
                    case "Block":
                        sceneContent.Add(new Block((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "PlayerModel":
                        sceneContent.Add(new PlayerModel(Globals.SpriteSheet, Tile.GetTileCenter(data.TilePos)));
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
            //var file = File.OpenRead(path);

            //foreach (var file in Directory.GetFiles(path))
            
                //var typeAsText = file.Split("\\")[^1];
                //typeAsText = typeAsText.Remove(typeAsText.Count() - 4, 4);
                //var type = Type.GetType(typeAsText);
                //MethodInfo method = typeof(Serializer).GetMethod("Deserialize");
                //MethodInfo generic = method.MakeGenericMethod(type);
                //readedData.AddRange(((IEnumerable)generic.Invoke(_serialize, new object[] { file })).Cast<T>());
                //foreach (var item in readedData)
                //{
                //    if (item is InventoryBlock)
                //    {
                //        var constructor = item.GetType().GetConstructor(new Type[] { typeof(int), typeof(int) });
                //        var x = (int)item.TilePosition.X;
                //        var y = (int)item.TilePosition.Y;
                //        sceneContent.Add((T)constructor.Invoke(new object[] { x, y }));
                //        continue;
                //    }
                //    if (item is Block)
                //    {
                //        var constructor = item.GetType().GetConstructor(new Type[] { typeof(int), typeof(int) });
                //        var x = (int)item.TilePosition.X;
                //        var y = (int)item.TilePosition.Y;
                //        sceneContent.Add((T)constructor.Invoke(new object[] {x, y}));
                //        continue;
                //    }
                //    if (item is PlayerModel)
                //    {
                //        var constructor = item.GetType().GetConstructor(new Type[] { typeof(Texture2D) });
                //        sceneContent.Add((T)constructor.Invoke(new object[] { Globals.SpriteSheet }));
                //        continue;
                //    }
                //}
                //readedData = new List<T>();
            
            return sceneContent;
        }

        public static void SaveScene()
        {
            var serializer = new Serializer();
            var entities = Globals.SceneManager.GetEntities();
            serializer.Serialize(Globals.SceneManager.GetName(), entities.Select(x => x as ISerializable).ToList());
        }

        public static Vector2 SwitchScene(HitboxTrigger trigger)
        {
            SaveScene();
            var currLoc = Globals.SceneManager.CurrentLocation;
            var newLoc = new Vector2(trigger.DestinationScene.X + currLoc.X, trigger.DestinationScene.Y + currLoc.Y);
            using (var writer = new StreamWriter(new FileStream("CurrentLoc.xml", FileMode.Create)))
            {
                var serialize = new XmlSerializer(typeof(Vector2));
                serialize.Serialize(writer, newLoc);
            }
            return newLoc;
        }
    }
}
