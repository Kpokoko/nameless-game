using Microsoft.Xna.Framework;
using nameless.Collisions;
using nameless.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public abstract class TileGridEntity
{
    public Vector2 Position
    {
        get { return _position; }
        set
        {
            _position = value;
            OnPositionChange(_position);
        }
    }
    public Vector2 TilePosition
    {
        get { return _tilePos; }
        set
        {
            _tilePos = value;
            Position = Tile.GetTileCenter(_tilePos);
        }
    }
    private Vector2 _position;
    private Vector2 _tilePos;

    public abstract void OnPositionChange(Vector2 position);
}
