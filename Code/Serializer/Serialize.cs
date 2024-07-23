using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace nameless.Serialize;

public struct Serializer
{
    public Serializer() { }
    public void Serialize(string sceneName, List<ISerializable> entities)
    {
        using (var writer = new StreamWriter(new FileStream("Levels/" + sceneName /*+ '/' + typeof(T).ToString()*/ + ".xml", FileMode.Create)))
        {
            //var a = typeof(List<T>);
            var serializer = new XmlSerializer(typeof(List<SerializationInfo>));
            var a = entities.Select(x => x.Info).ToList();
            serializer.Serialize(writer, a);
        }
    }
    public List<SerializationInfo> Deserialize(string sceneName)
    {
        var serializer = new XmlSerializer(typeof(List<SerializationInfo>));
        try
        {
            using (var reader = new StreamReader(new FileStream(sceneName, FileMode.Open)))
            {
                var scores = (List<SerializationInfo>)serializer.Deserialize(reader);
                return scores;
            }
        }
        catch (FileNotFoundException)
        {
            using (StreamWriter writer = new StreamWriter(sceneName))
            {
                serializer.Serialize(writer, new List<SerializationInfo> { new PlayerModel(Globals.SpriteSheet).Info });
            }
            using (var reader = new StreamReader(new FileStream(sceneName, FileMode.Open)))
            {
                var scores = (List<SerializationInfo>)serializer.Deserialize(reader);
                return scores;
            }
        }
    }
}
