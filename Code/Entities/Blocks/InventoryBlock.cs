﻿using Microsoft.Xna.Framework;
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
        { Colliders[0].Color = Color.Green; IsEnableToPlayer = true; }
        public InventoryBlock(Vector2 tilePosition) : this((int)tilePosition.X, (int)tilePosition.Y) { }

        //public bool IsHolding { get; set; }

        //public void UpdateConstructor(GameTime gameTime)
        //{
        //    if (this.TilePosition != MouseInputController.MouseTilePos)
        //        TilePosition = MouseInputController.MouseTilePos;
        //}
    }
}
