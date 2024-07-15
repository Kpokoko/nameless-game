using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Code.SceneManager;
using nameless.Collisions;
using nameless.Controls;
using nameless.Interfaces;
using nameless.Tiles;
using System;
using System.Xml.Serialization;

namespace nameless.Entity
{
    public class InventoryBlock : Block, IConstructable
    {
        public InventoryBlock(int x, int y) : base(x, y)
        { }

        [XmlIgnore]
        private bool _isHolding;

        public InventoryBlock() { }

        public void UpdateConstructor(GameTime gameTime)
        {
            var mouseState = MouseInputController.MouseState;
            var mousePos = MouseInputController.MousePos;
            if (this.TilePosition == MouseInputController.PreviousMouseTilePos)
                _isHolding = true;
            if (_isHolding && mouseState.LeftButton is ButtonState.Pressed)
            {
                var mouseTilePos = Tile.GetPosInTileCoordinats(mousePos);
                if (this.TilePosition != mouseTilePos)
                    TilePosition = mouseTilePos;
            }
            if (!(MouseInputController.MouseState.LeftButton is ButtonState.Pressed))
                _isHolding = false;
        }
    }
}
