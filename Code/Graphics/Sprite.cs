using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Graphics;

public class Sprite
{
    public Sprite(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Sprite(Texture2D texture) : this(texture, 0, 0, texture.Width, texture.Height) { }

    public Texture2D Texture { get; private set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Color TintColor { get; set; } = Color.White;
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        spriteBatch.Draw(Texture, position, new Rectangle(X, Y, Width, Height), color, 0, Vector2.Zero,1,SpriteEffects.None,0.03f);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position) => Draw(spriteBatch, position, TintColor);

}
