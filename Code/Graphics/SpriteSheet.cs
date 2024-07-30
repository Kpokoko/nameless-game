using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Graphics;

public class SpriteSheet
{
    public Sprite[,] Sprites { get; private set; }
    public SpriteSheet(Texture2D spriteSheet, int spriteWidth, int spriteHeight)
    {
        var countX = spriteSheet.Width / spriteWidth;
        var countY = spriteSheet.Height / spriteHeight;
        Sprites = new Sprite[countX, countY];

        for (int i = 0; i < countX; i++)
            for (int j = 0; j < countY; j++)
            {
                Sprites[i,j] = new Sprite(spriteSheet, i * spriteWidth, j * spriteHeight, spriteWidth, spriteHeight);
            }
    }

    public Sprite this[int x, int y]
    {
        get { return Sprites[x, y]; }
    }
}
