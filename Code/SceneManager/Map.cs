using Microsoft.Xna.Framework;
using nameless.Engine;
using nameless.Entity;
using nameless.Serialize;
using nameless.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace nameless.Code.SceneManager;

public class Map
{
    //public string[][] MapArray { get; private set; }
    public SceneInfo[][] MapArray { get; private set; }
    private Point _center;

    //public Map(string[][] map)
    //{
    //    MapArray = map;
    //    _center = FindCenter();
    //}

    public Map(IEnumerable<SceneInfo> info)
    {
        info = info.OrderBy(x => x.Coordinates.ToVector2().Length()).ToArray();
        var center = info.FirstOrDefault(i => i.Coordinates == Point.Zero);
        _center = Point.Zero;
        MapArray = new[] { new[] { center } };
        foreach (var i in info)
        {
            this[i.Coordinates.X,i.Coordinates.Y] = i;
        }
    }

    private Point FindCenter()
    {
        for (var y = 0; y < MapArray[0].Length; y++)
        {
            for (var x = 0; x < MapArray.Length; x++)
            {
                var center = (MapArray[x][y].Coordinates) == Point.Zero;
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
    public SceneInfo this[int x, int y]
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
                (absX, absY) = GetAbsoluteCoordinates(x, y);
            }
            MapArray[absX][absY] = value;
        }
    }

    private ValueTuple<int,int> GetAbsoluteCoordinates(int x, int y)
    {
        return  (x + _center.X,y + _center.Y);
    }

    public static Point? ParseCoordinates(string scenePath, out string sceneName)
    {
        sceneName = null;
        if (scenePath == null) 
            return null;
        var parts = scenePath.Split(Path.DirectorySeparatorChar);
        if (parts[^1].Length <= 5) 
            return null;
        var words = parts[^1].Split(new[] { ' ', '.' }).SkipLast(1).ToArray();

        var lastWords = words.Skip(2);
        if (lastWords.Count() > 0)
            sceneName = String.Join(' ', lastWords);

        return new Point(int.Parse(words[0]), int.Parse(words[1]));
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
            newArray.Insert(0, new SceneInfo[MapArray[0].Length]);
            _center.X++;
        }
        else if (absX > MapArray.Length - 1)
        {
            xDist = absX - (MapArray.Length - 1);
            newArray.Add(new SceneInfo[MapArray[0].Length]);
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
            _center.Y++;
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

    public static bool LoadMap()
    {
        var visitedScenes = Globals.Serializer.ReadVisitedScenes();
        if (visitedScenes == null)
            return false;
        var scenes = Directory.GetFiles(Path.Combine("..", "net6.0", "Layout", "Levels"));
        var sceneInfo = new List<SceneInfo>();
        foreach (var scene in scenes)
        {
            var sceneCoords = ParseCoordinates(scene, out var name);
            if (sceneCoords == null) continue;
            var coolName = new SceneInfo(name, (Point)sceneCoords);
            sceneInfo.Add(coolName);
            var coolNameNAME = coolName.FullName;
            if (visitedScenes.Contains(coolNameNAME) || Globals.IsDeveloperModeEnabled)
            {
                var visitedSceneStorage = Scene.GetSceneStorage(coolNameNAME).ConvertToEnum();
                coolName.Minimap = new Minimap(
                    new Vector2(coolName.Coordinates.X, coolName.Coordinates.Y ),
                    visitedSceneStorage);
                visitedSceneStorage = null;
                GC.Collect();
            }
        }
        Globals.Map = new Map(sceneInfo);
        return true;
    }
}

public class SceneInfo
{
    public string Name;
    public Point Coordinates;
    public Minimap Minimap;
    public string FullName { get { return Name != null ? Coordinates.ToSimpleString() + ' ' + Name : Coordinates.ToSimpleString(); } }

    public SceneInfo(string name, Point coords, Minimap minimap = null)
    {
        Name = name;
        Coordinates = coords;
        Minimap = minimap;
    }
}
