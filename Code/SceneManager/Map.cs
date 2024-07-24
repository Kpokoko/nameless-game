using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.SceneManager;

public class Map
{
    private string[][] _mapArray;
    private Point _center;

    public Map(string[][] map)
    {
        _mapArray = map;
        _center = FindCenter();
    }

    private Point FindCenter()
    {
        for (var y = 0; y < _mapArray[0].Length; y++)
        {
            for (var x = 0; x < _mapArray.Length; x++)
            {
                var center = _mapArray[x][y] == "center";
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
                return _mapArray[absX][absY];
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
            _mapArray[absX][absY] = value;
        }
    }

    private ValueTuple<int,int> GetAbsoluteCoordinates(int x, int y)
    {
        return  (x + _center.X,y + _center.Y);
    }

    private bool InBounds(int absX, int absY)
    {
        return (absX >= 0 && absX < _mapArray[0].Length
            && absY >= 0 && absY < _mapArray.Length);
    }

    private void ResizeMap(int absX, int absY)
    {
        var newArray = _mapArray.ToList();

        int xDist; int yDist;
        if (absX < 0)
        {
            xDist = absX;
            newArray.Insert(0, new string[_mapArray[0].Length]);
        }
        else if (absX > _mapArray.Length - 1)
        {
            xDist = absX - (_mapArray.Length - 1);
            newArray.Add(new string[_mapArray[0].Length]);
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
        else if (absY > _mapArray.Length - 1)
        {
            yDist = absY - (_mapArray.Length - 1);
            for (var i = 0; i < newArray.Count; i++)
            {
                var newSubArray = newArray[i].ToList();
                newSubArray.Add(null);
                newArray[i] = newSubArray.ToArray();
            }
        }

        _mapArray = newArray.ToArray();
    }
}
