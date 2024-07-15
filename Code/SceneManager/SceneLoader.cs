using Microsoft.Xna.Framework.Graphics;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace nameless.Code.SceneManager
{
    public static class SceneLoader
    {
        private static Serializer _serialize = new Serializer();
        public static List<IEntity> LoadScene(string sceneName, string dir)
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
                        sceneContent.Add(new PlayerModel(Globals.SpriteSheet));
                        continue;
                    case "InventoryBlock":
                        sceneContent.Add(new InventoryBlock((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                    case "EditorBlock":
                        sceneContent.Add(new EditorBlock((int)data.TilePos.X, (int)data.TilePos.Y));
                        continue;
                }
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
    }
}
