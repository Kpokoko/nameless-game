using Microsoft.Xna.Framework;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless_game_branch.Tiles;

public struct Tile
{
    public static int Width = 64;
    public static int Height = 64;
    public Vector2 Position;

    public Tile(int x, int y)
    {
        Position = GetTileCenter(x, y);
    }

    private static Vector2 GetTileCenter(int x, int y)
    {
        var posX = x * Width + Width / 2;
        var posY = y * Height + Height / 2;
        return new Vector2(posX, posY);
    }

    //public static void GetPosInTileCoordinats<T>(List<T> list)
    //    where T : IEntity
    //{
    //    foreach (var item in list)
    //    {
    //        var tileX = Math.Floor(item.Position.X / 64);
    //        var tileY = Math.Floor(item.Position.Y / 64);
    //        item.Position = new Vector2((float)tileX, (float)tileY);
    //    }
    //}
}
