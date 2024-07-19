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
        { colliders[0].Color = Color.Red; }

        public bool IsHolding { get; set; }

        public void UpdateConstructor(GameTime gameTime)
        {
            if (this.TilePosition != MouseInputController.MouseTilePos)
                TilePosition = MouseInputController.MouseTilePos;
        }
    }
}
