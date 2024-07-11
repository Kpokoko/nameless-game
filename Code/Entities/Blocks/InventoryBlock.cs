using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Collisions;
using nameless.Tiles;
using System;

namespace nameless.Entity
{
    public class InventoryBlock : Block
    {
        private Vector2 _oldMousePos;
        public InventoryBlock(int x, int y)
        {
            var block = new Block(x, y);  //Если пытаться инициализировать без вот этих танцев с бубном - почему-то вызывается пустой конструктор, и там гг
            this.Position = block.Position;
            this.TilePosition = block.TilePosition;
            this.collider = block.collider;
            _oldMousePos = new Vector2(0, 0);
        }

        public InventoryBlock() { }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var mousePos = new Vector2(mouseState.X, mouseState.Y);
            collider.RemoveCollision();
            if (this.TilePosition == Tile.GetPosInTileCoordinats(_oldMousePos) && mouseState.LeftButton is ButtonState.Pressed)
            {
                this.Position = Tile.GetTileCenter(Tile.GetPosInTileCoordinats(mousePos));
                this.TilePosition = Tile.GetPosInTileCoordinats(mousePos);
            }
            _oldMousePos = mousePos;
            collider = new Collider(this, 64, 64);
            base.Update(gameTime);
        }
    }
}
