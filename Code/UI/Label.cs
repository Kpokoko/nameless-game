using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class Label : IEntity, IUI
{
    public Vector2 Position 
    { 
        get { return _position; } 
        set { _position = value; _drawPos = value + Globals.Offset((int)Size.X, (int)Size.Y); } 
    }
    private Vector2 _position;

    private Vector2 _drawPos;
    public Vector2 TilePosition { get; set; }
    public int DrawOrder => -1;
    public string Text { get; set; }
    public SpriteFont Font { get; set; } = Globals.UIManager.Font;
    public Vector2 Size { get; set; }

    public Label(Vector2 position, string text) 
    {
        Text = text;
        Size = Font.MeasureString(Text);
        Position = position;
        Globals.UIManager.Labels.Add(this);
    }

    public void Remove()
    {
        Globals.UIManager.Labels.Remove(this);
    }

    public void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(Font, Text, _drawPos, Color.Black);
    }
}
