using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace nameless.Code.SceneManager;

public class Map
{
    public string[][] MapArray { get; private set; }
    private Point _center;

    public Map(string[][] map)
    {
        MapArray = map;
        _center = FindCenter();
    }

    private Point FindCenter()
    {
        for (var y = 0; y < MapArray[0].Length; y++)
        {
            for (var x = 0; x < MapArray.Length; x++)
            {
                var center = (MapArray[x][y]) == "0 0";
                if (center)
                {
                    return new Point(x, y);
                }
            }
        }
        throw new Exception("\"center\" scene not found on map");
    }

    /// <summary>
    /// interact with relative to center scene coordinates
    /// </summary>
    /// <returns>scene name or null, if coordinates out of bounds</returns>
    public string this[int x, int y]
    {
        get
        {
            var (absX, absY) = GetAbsoluteCoordinates(x, y);
            if (InBounds(absX, absY))
            {
                return MapArray[absX][absY];
            }
            else
            {
                return null;
            }
        }
        set 
        {
            var (absX, absY) = GetAbsoluteCoordinates(x, y);
            if (!InBounds(absX,absY))
            {
                ResizeMap(absX, absY);
                _center = FindCenter();
                (absX, absY) = GetAbsoluteCoordinates(x, y);
            }
            MapArray[absX][absY] = value;
        }
    }

    private ValueTuple<int,int> GetAbsoluteCoordinates(int x, int y)
    {
        return  (x + _center.X,y + _center.Y);
    }

    public string ParseCoordinates(string sceneName)
    {
        if (sceneName == null) 
            return null;
        var words = sceneName.Split(' ');
        return words[0]+ ' ' + words[1];
    }

    private bool InBounds(int absX, int absY)
    {
        return (absX >= 0 && absX < MapArray.Length
            && absY >= 0 && absY < MapArray[0].Length);
    }

    private void ResizeMap(int absX, int absY)
    {
        var newArray = MapArray.ToList();

        int xDist; int yDist;
        if (absX < 0)
        {
            xDist = absX;
            newArray.Insert(0, new string[MapArray[0].Length]);
        }
        else if (absX > MapArray.Length - 1)
        {
            xDist = absX - (MapArray.Length - 1);
            newArray.Add(new string[MapArray[0].Length]);
        }
        else if (absY < 0)
        {
            yDist = absY;
            for (var i= 0;i < newArray.Count;i++)
            {
                var newSubArray = newArray[i].ToList();
                newSubArray.Insert(0,null);
                newArray[i] = newSubArray.ToArray();
            }
        }
        else if (absY > MapArray.Length - 1)
        {
            yDist = absY - (MapArray.Length - 1);
            for (var i = 0; i < newArray.Count; i++)
            {
                var newSubArray = newArray[i].ToList();
                newSubArray.Add(null);
                newArray[i] = newSubArray.ToArray();
            }
        }

        MapArray = newArray.ToArray();
    }

    public static void LoadMap()
    {

        using (var reader = new StreamReader(new FileStream(Path.Combine("..", "net6.0", "Map.xml"), FileMode.Open)))
        {
            var serializer = new XmlSerializer(typeof(string[][]));
            var scores = (string[][])serializer.Deserialize(reader);
            Globals.Map = new Map(scores);
        }
    }

    public static void SaveMap()
    {
        using (var writer = new StreamWriter(new FileStream("Map.xml", FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(string[][]));
            var mapArray = Globals.Map.MapArray;
            serializer.Serialize(writer, mapArray);
        }
    }
}
