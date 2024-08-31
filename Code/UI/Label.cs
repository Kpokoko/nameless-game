using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Engine;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class Label : UIElement
{
    public string Text { get; set; }
    public SpriteFont Font { get; set; } = ResourceManager.Font;

    public Label(Vector2 position, string text, Alignment align = Alignment.Center) : base(position, ResourceManager.Font.MeasureString(text))
    {
        Alignment = align;
        Text = text;
        Globals.UIManager.Labels.Add(this);
    }

    override public void Remove()
    {
        Globals.UIManager.Labels.Remove(this);
    }

    public void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(Font, Text, (Bounds.RectangleF.Position), Color.Black,0,Vector2.Zero,1,SpriteEffects.None,0.03f + DrawOrder);
    }
}
