using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Collisions;
using nameless.Controls;
using nameless.Tiles;
using System;

namespace nameless.Entity
{
    public class InventoryBlock : Block
    {
        private Vector2 _oldMousePos;
        public InventoryBlock(int x, int y) : base(x, y)
        {
            _oldMousePos = new Vector2(0, 0);
        }

        public InventoryBlock() { }

        public override void Update(GameTime gameTime)
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
}
