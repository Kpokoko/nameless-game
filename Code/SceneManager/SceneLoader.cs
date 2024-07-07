using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using nameless.Code.Entities;
using nameless.Collisions;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.SceneManager
{
    public static class SceneLoader<T> where T : IEntity
    {
        private static Serializer _serialize = new Serializer();
        public static List<T> LoadScene(string sceneName, string dir)
        {
            var readedData = new List<T>();
            var sceneContent = new List<T>();
            var path = Path.Combine("..", "net6.0", "Levels", sceneName);
            foreach (var file in Directory.GetFiles(path))
            {
                var typeAsText = file.Split("\\")[^1];
                typeAsText = typeAsText.Remove(typeAsText.Count() - 4, 4);
                var type = Type.GetType(typeAsText);
                MethodInfo method = typeof(Serializer).GetMethod("Deserialize");
                MethodInfo generic = method.MakeGenericMethod(type);
                readedData.AddRange(((IEnumerable)generic.Invoke(_serialize, new object[] { file })).Cast<T>());
                foreach (var item in readedData)
                {
                    if (item is Block)
                    {
                        var constructor = item.GetType().GetConstructor(new Type[] { typeof(int), typeof(int) });
                        var x = (int)item.TilePosition.X;
                        var y = (int)item.TilePosition.Y;
                        sceneContent.Add((T)constructor.Invoke(new object[] {x, y}));
                    }
                    if (item is PlayerModel)
                    {
                        var constructor = item.GetType().GetConstructor(new Type[] { typeof(Texture2D) });
                        sceneContent.Add((T)constructor.Invoke(new object[] { Globals.SpriteSheet }));
                    }
                }
                readedData = new List<T>();
            }
            return sceneContent;
        }
    }
}
