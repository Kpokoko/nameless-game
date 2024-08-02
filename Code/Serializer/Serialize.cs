using nameless.Engine;
using nameless.Entity;
using nameless.Interfaces;
using nameless.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace nameless.Serialize;

public struct Serializer
{
    public Serializer() { }
    public void SerializeScene(string sceneName, List<ISerializable> entities)
    {
        using (var writer = new StreamWriter(new FileStream("Levels/" + sceneName + ".xml", FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(List<SerializationInfo>));
            var a = entities.Select(x => x.Info).ToList();
            serializer.Serialize(writer, a);
        }
    }
    public List<SerializationInfo> DeserializeScene(string sceneName)
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

    public void Restart()
    {
        using (var writer = new StreamWriter(new FileStream("CurrentLoc.xml", FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(Vector2));
            var a = new Vector2(0, 0);
            serializer.Serialize(writer, a);
        }
        using (var writer = new StreamWriter(new FileStream("Map.xml", FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(List<string>));
            var a = new List<string> { "0 0 Center" };
            serializer.Serialize(writer, a);
        }
    }

    public void SavePosition(Vector2 loc)
    {
        using (var writer = new StreamWriter(new FileStream("CurrentLoc.xml", FileMode.Create)))
        {
            var serialize = new XmlSerializer(typeof(Vector2));
            serialize.Serialize(writer, loc);
        }
    }

    public List<string> ReadVisitedScenes()
    {
        using (var reader = new StreamReader(new FileStream("Map.xml", FileMode.Open)))
        {
            var serialize = new XmlSerializer(typeof(List<string>));
            return (List<string>)serialize.Deserialize(reader);
        }
    }

    public void UpdateVisitedScenes(List<string> scenes)
    {
        using (var writer = new StreamWriter(new FileStream("Map.xml", FileMode.Create)))
        {
            var serialize = new XmlSerializer(typeof(List<string>));
            serialize.Serialize(writer, scenes);
        }
    }

    public void LoadMinimap(Vector2 pos, List<string> data, Vector2 newPos)
    {
        using (var writer = new StreamWriter(new FileStream("Map.xml", FileMode.Create)))
        {
            var serialize = new XmlSerializer(typeof(List<string>));
            var name = newPos.ToSimpleString();
            if (!data.Contains(name))
            {
                data.Add(name);
                Globals.UIManager.Minimaps.Add(new Minimap(newPos, 0, 0, Globals.SceneManager.GetStorage().ConvertToEnum(), Alignment.Center));
            }
            serialize.Serialize(writer, data);
        }
    }

    public void GetMapPos()
    {
        var serializer = new XmlSerializer(typeof(Vector2));
        using (var reader = new StreamReader(new FileStream("CurrentLoc.xml", FileMode.Open)))
        {
            var location = (Vector2)serializer.Deserialize(reader);
            Globals.SceneManager.LoadScene(location);
        }
    }

    //public void SaveMap()
    //{
    //    using (var writer = new StreamWriter(new FileStream("Map.xml", FileMode.Create)))
    //    {
    //        var serializer = new XmlSerializer(typeof(string[][]));
    //        var mapArray = Globals.Map.MapArray;
    //        serializer.Serialize(writer, mapArray);
    //    }
    //}
}
