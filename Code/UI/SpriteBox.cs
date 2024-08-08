using Microsoft.Xna.Framework.Graphics;
using nameless.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class SpriteBox : UIElement
{
    public Sprite Sprite;
    public SpriteBox(Vector2 position, Sprite sprite, float width, float height) : base(position, width, height)
    {
        this.Sprite = sprite;
        Globals.UIManager.SpriteBoxes.Add(this);
    }

    public override void Remove()
    {
        Globals.UIManager.SpriteBoxes.Remove(this);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Globals.Draw(AbsolutePosition, spriteBatch, Sprite);
    }
}
