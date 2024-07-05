using Microsoft.Xna.Framework;
using nameless.Interfaces;
using nameless_game_branch.Entity;
using nameless_game_branch.Tiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace nameless.Serialize
{
    public struct Serializer
    {
        public Serializer() { }
        public void Serialize<T>(string sceneName, List<T> entities)
            where T : IEntity
        {
            using (var writer = new StreamWriter(new FileStream(sceneName, FileMode.Create)))
            {
                var serializer = new XmlSerializer(entities.GetType());
                serializer.Serialize(writer, entities);
            }
        }
        public List<T> Deserialize<T>(string sceneName)
            where T : IEntity
        {
            using (var reader = new StreamReader(new FileStream(sceneName, FileMode.Open)))
            {
                var serializer = new XmlSerializer(typeof(List<Block>));
                var scores = (List<T>)serializer.Deserialize(reader);
                return scores;
            }
        }
    }
}
