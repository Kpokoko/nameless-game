using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using nameless.Controls;
using nameless.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Collisions;
using nameless.Interfaces;

namespace nameless.Entity;

public partial class Block : IConstructable
{
    public void UpdateConstructor(GameTime gameTime)
    {
        var mouseState = MouseInputController.MouseState;
        var mousePos = MouseInputController.MousePos;
        if (this.TilePosition == MouseInputController.PreviousMouseTilePos && mouseState.LeftButton is ButtonState.Pressed)
        {
            var mouseTilePos = Tile.GetPosInTileCoordinats(mousePos);
            if (this.TilePosition != mouseTilePos)
                TilePosition = mouseTilePos;
        }
    }
}
