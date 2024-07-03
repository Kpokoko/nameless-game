using Microsoft.Xna.Framework;
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
}
