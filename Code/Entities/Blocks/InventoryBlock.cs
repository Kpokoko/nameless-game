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
            if (this.TilePosition == Tile.GetPosInTileCoordinats(_oldMousePos) && mouseState.LeftButton is ButtonState.Pressed)
            {
                var mouseTilePos = Tile.GetPosInTileCoordinats(mousePos);
                if (this.TilePosition != mouseTilePos)
                    MoveToTile(mouseTilePos);
            }
            _oldMousePos = mousePos;
            base.Update(gameTime);
        }

        private void MoveToTile(Vector2 mouseTilePos)
        {
            collider.RemoveCollider();
            this.Position = Tile.GetTileCenter(mouseTilePos);
            this.TilePosition = mouseTilePos;
            collider = new Collider(this, 64, 64);
        }
    }
}
