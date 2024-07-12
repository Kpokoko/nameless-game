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

namespace nameless.Entity;

public partial class Block
{
    public void UpdateConstructor(GameTime gameTime)
    {
        var mouseState = MouseInputController.MouseState;
        var mousePos = MouseInputController.MousePos;
        if (this.TilePosition == Tile.GetPosInTileCoordinats(MouseInputController.PreviousMousePos) && mouseState.LeftButton is ButtonState.Pressed)
        {
            var mouseTilePos = Tile.GetPosInTileCoordinats(mousePos);
            if (this.TilePosition != mouseTilePos)
                MoveToTile(mouseTilePos);
        }
    }

    private void MoveToTile(Vector2 mouseTilePos)
    {
        collider.RemoveCollider();
        this.Position = Tile.GetTileCenter(mouseTilePos);
        this.TilePosition = mouseTilePos;
        collider = new Collider(this, 64, 64);
    }
}
