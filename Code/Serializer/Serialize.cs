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
        using (var writer = new StreamWriter(new FileStream(Path.Combine("Layout", "Levels", sceneName + ".xml"), FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(List<SerializationInfo>));
            var a = entities
                .Where(x => x.AllowSerialization)
                .Select(x => x.Info).ToList();
            serializer.Serialize(writer, a);
        }
        Globals.UIManager.PopupMessage("Serialized");
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
                serializer.Serialize(writer, new List<SerializationInfo> { new PlayerModel(ResourceManager.SpriteSheet).Info });
            }
            using (var reader = new StreamReader(new FileStream(sceneName, FileMode.Open)))
            {
                var scores = (List<SerializationInfo>)serializer.Deserialize(reader);
                return scores;
            }
        }
    }

    public Vector2 ReadSaveSpot()
    {
        var serializer = new XmlSerializer(typeof(SerializationInfo));
        using (var reader = new StreamReader(new FileStream(Path.Combine("Layout","SaveSpot.xml"), FileMode.Open)))
        {
            var block = (SerializationInfo)serializer.Deserialize(reader);
            Globals.LastVisitedCheckpoint = block;
            return block.TilePos;
        }
    }

    public void WriteSaveSpot()
    {
        var serializer = new XmlSerializer(typeof(SerializationInfo));
        using (var writer = new StreamWriter(new FileStream(Path.Combine("Layout", "SaveSpot.xml"), FileMode.Create)))
        {
            serializer.Serialize(writer, Globals.LastVisitedCheckpoint);
        }
    }

    public void Restart()
    {
        using (var writer = new StreamWriter(new FileStream(Path.Combine("Layout", "CurrentLoc.xml"), FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(Vector2));
            var a = new Vector2(0, 0);
            serializer.Serialize(writer, a);
        }
        using (var writer = new StreamWriter(new FileStream(Path.Combine("Layout", "Map.xml"), FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(List<string>));
            var a = new List<string> { "0 0" };
            serializer.Serialize(writer, a);
        }
    }

    public void SavePosition(Vector2 loc)
    {
        using (var writer = new StreamWriter(new FileStream(Path.Combine("Layout", "CurrentLoc.xml"), FileMode.Create)))
        {
            var serialize = new XmlSerializer(typeof(Vector2));
            serialize.Serialize(writer, loc);
        }
    }

    public List<string> ReadVisitedScenes()
    {
        if (!File.Exists(Path.Combine("Layout", "Map.xml")))
            return null;

        using (var reader = new StreamReader(new FileStream(Path.Combine("Layout","Map.xml"), FileMode.Open)))
        {
            var serialize = new XmlSerializer(typeof(List<string>));
            return (List<string>)serialize.Deserialize(reader);
        }
    }

    public void UpdateVisitedScenes(List<string> scenes)
    {
        using (var writer = new StreamWriter(new FileStream(Path.Combine("Layout","Map.xml"), FileMode.Create)))
        {
            var serialize = new XmlSerializer(typeof(List<string>));
            serialize.Serialize(writer, scenes);
        }
    }

    public void LoadMinimap(Vector2 pos, List<string> data, Vector2 newPos)
    {
        using (var writer = new StreamWriter(new FileStream(Path.Combine("Layout", "Map.xml"), FileMode.Create)))
        {
            var serialize = new XmlSerializer(typeof(List<string>));
            var name = newPos.ToSimpleString();
            if (!data.Contains(name))
            {
                data.Add(name);
                if (!Globals.UIManager.Minimaps.Select(m => m.Location).Contains(newPos))
                    Globals.UIManager.Minimaps.Add(new Minimap(newPos, Globals.SceneManager.GetStorage().ConvertToEnum()));
            }
            serialize.Serialize(writer, data);
        }
    }

    public void GetMapPos()
    {
        var serializer = new XmlSerializer(typeof(Vector2));
        using (var reader = new StreamReader(new FileStream(Path.Combine("Layout", "CurrentLoc.xml"), FileMode.Open)))
        {
            var location = (Vector2)serializer.Deserialize(reader);
            Globals.SceneManager.LoadScene(location);
        }
    }

    public Dictionary<EntityTypeEnum, int> GetInventory()
    {
        using (var reader = new StreamReader(new FileStream(Path.Combine("Layout","Inventory.xml"), FileMode.Open)))
        {
            var serializer = new XmlSerializer(typeof(List<MyTuple<EntityTypeEnum, int>>));
            var inventory = (List<MyTuple<EntityTypeEnum, int>>)serializer.Deserialize(reader);
            var dict = inventory.ToDictionary(x => x.Item1, x => x.Item2);
            return dict;
        }
    }

    public void SaveInventory(Dictionary<EntityTypeEnum, int> inventory)
    {
        using (var writer = new StreamWriter(new FileStream(Path.Combine("Layout", "Inventory.xml"), FileMode.Create)))
        {
            var serialize = new XmlSerializer(typeof(List<MyTuple<EntityTypeEnum, int>>));
            var list = inventory.Select(x => new MyTuple<EntityTypeEnum, int>(x.Key, x.Value)).ToList();
            serialize.Serialize(writer, list);
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
