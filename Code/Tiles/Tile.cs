using Microsoft.Xna.Framework;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Tiles;

public struct Tile
{
    public static int Width = 64;
    public static int Height = 64;
    public Vector2 Position;

    public Tile(int x, int y)
    {
        Position = GetTileCenter(new Vector2(x, y));
    }

    public static Vector2 GetTileCenter(Vector2 absolutePos)
    {
        var posX = absolutePos.X * Width + Width / 2;
        var posY = absolutePos.Y * Height + Height / 2;
        return new Vector2(posX, posY);
    }

    public static Vector2 GetPosInTileCoordinats(Vector2 absolutePos)
    {
        var tileX = Math.Floor(absolutePos.X / 64);
        var tileY = Math.Floor(absolutePos.Y / 64);
        return new Vector2((float)tileX, (float)tileY);
            //item.Position = new Vector2((float)tileX, (float)tileY);
    }
}
