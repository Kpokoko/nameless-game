using nameless.Interfaces;
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
        using (var reader = new StreamReader(new FileStream(sceneName, FileMode.Open)))
        {
            var serializer = new XmlSerializer(typeof(List<SerializationInfo>));
            var scores = (List<SerializationInfo>)serializer.Deserialize(reader);
            return scores;
        }
    }
}
