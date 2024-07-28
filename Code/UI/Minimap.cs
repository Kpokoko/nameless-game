using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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
    private TileGridEntity[,] _mapArray;
    private int blockSize;
    public Minimap(Vector2 position, int width, int height, TileGridEntity[,] array, Alignment align) : base(position, width, height, align)
    {
        _mapArray = array;
        blockSize = 10;

        Globals.UIManager.Minimaps.Add(this);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = Color.Transparent;
        for (var i = 0;i< _mapArray.GetLength(0);i++)
            for (var j =0; j< _mapArray.GetLength(1); j++)
            {
                var entity = _mapArray[i,j];
                if (entity == null) continue;
                var rect = new Rectangle(i * blockSize + (int)Position.X, j * blockSize + (int)Position.Y, blockSize, blockSize);

                switch (entity.GetType().Name)
                {
                    case "Block": color = Color.Brown; break;
                    case "EditorBlock": color = Color.Red; break;
                    case "Platform": color = Color.Green; rect.Height = 2; break;
                }

                spriteBatch.DrawRectangle(rect, color, blockSize/2);
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
