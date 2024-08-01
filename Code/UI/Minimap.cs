﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Code.SceneManager;
using nameless.Entity;
using nameless.Interfaces;
using nameless.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class Minimap : UIElement, IEntity
{
    private EntityTypeEnum[,] _mapArray;
    public static int TileSize = (int)(20 / Globals.Camera.Zoom);
    public Minimap(Vector2 position, int width, int height, EntityTypeEnum[,] array, Alignment align) : base(position, width, height, align)
    {
        _mapArray = array;

        Globals.UIManager.Minimaps.Add(this);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = Color.Transparent;
        for (var i = 0; i < _mapArray.GetLength(0);i++)
            for (var j = 0; j < _mapArray.GetLength(1); j++)
            {
                var entity = _mapArray[i,j];
                if (entity is EntityTypeEnum.None || entity is EntityTypeEnum.HitboxTrigger) continue;
                var rect = new Rectangle(i * TileSize + (int)Position.X, j * TileSize + (int)Position.Y, TileSize, TileSize);

                switch (entity)
                {
                    case EntityTypeEnum.EditorBlock: color = Color.AliceBlue; break;
                    case EntityTypeEnum.InventoryBlock: color = Color.Red; break;
                    case EntityTypeEnum.Block: color = Color.Brown; break;
                    case EntityTypeEnum.Platform: color = Color.Green; rect.Height = 2; break;
                }

                spriteBatch.DrawRectangle(rect, color, TileSize/2 + 1);
            }
    }

    public override void Remove()
    {
        Globals.UIManager.Minimaps.Remove(this);
    }

    public void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }
}
