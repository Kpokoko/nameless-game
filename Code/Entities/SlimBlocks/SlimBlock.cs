using MonoGame.Extended;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class SlimBlock : Block {

    Vector2[] _betweenTiles;
    public Vector2[] BetweenTiles
    {
        get => _betweenTiles;
        set
        {
            _betweenTiles = value.OrderBy(v => v.Length()).ToArray();
        }
    }
    public Vector2 Direction { get; set; }
    public Vector2 BetweenTilePosition { get; set; }

    public SlimBlock(int x, int y, Vector2 dir) : base(x, y)
    {
        BetweenTiles = new Vector2[] { TilePosition, TilePosition + dir };
        Direction = dir;
        BetweenTilePosition = TilePosition + dir / 2;
        Colliders[0].SetOffset(dir * 32);
        PrepareSerializationInfo();
    }

    public SlimBlock(Vector2 block1, Vector2 block2) : this((int)block1.X, (int)block1.Y, block2 - block1) { }

    public SlimBlock(Vector2 betweenTilePos) : this
        (
        betweenTilePos.X % 1 == 0.5f ?
        betweenTilePos.SetX((int)betweenTilePos.X) : betweenTilePos.SetY((int)betweenTilePos.Y),
        betweenTilePos.X % 1 == 0.5f ?
        betweenTilePos.SetX((int)Math.Ceiling(betweenTilePos.X)) : betweenTilePos.SetY((int)Math.Ceiling(betweenTilePos.Y))
        )
    {}

    public bool IsBetweenTiles(Vector2 tile1, Vector2 tile2)
    {
        var ordered = new[] { tile1, tile2 };
        ordered = ordered.OrderBy(x => x.Length()).ToArray();
        return BetweenTiles[0] == ordered[0] && BetweenTiles[1] == ordered[1];
    }

    public bool IsBetweenTiles(Vector2[] orderedTiles)
    {
        return BetweenTiles[0] == orderedTiles[0] && BetweenTiles[1] == orderedTiles[1];
    }

    public override void PrepareSerializationInfo()
    {
        base.PrepareSerializationInfo();
        Info.Direction = Direction;
    }

}
