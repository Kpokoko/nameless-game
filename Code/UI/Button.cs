using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Controls;

namespace nameless.UI;

public class Button : IEntity
{
    public Vector2 Position { get; set; }

    public Vector2 TilePosition { get; set; }

    public int DrawOrder => 0;

    public event Action OnClickEvent;
    private Rectangle Bounds { get; set; }

    public string Text { get; set; }
    public bool Hovered { get { return MouseInputController.MouseBounds.Intersects(Bounds); } }
    public bool Pressed { get; set; }

    public Button(Vector2 position, int width, int height) 
    {
        Position = position;
        Bounds = new Rectangle((Globals.Offset(width,height) + position).ToPoint(), new Size(width,height));

        Globals.UIManager.Buttons.Add(this);
    }

    public void SetText(string text)
    {
        Text = text;
    }

    public void Update(GameTime gameTime)
    {
        Pressed = false;
        if (Hovered && MouseInputController.LeftButton.IsPressed)
        {
            Pressed = true;
            if (OnClickEvent != null)
                OnClickEvent.Invoke();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var offset = !Pressed ? Point.Zero : new Point(0,3);
        var boundsSize = 6;
        var fillRect = new Rectangle(Bounds.Location + offset,Bounds.Size);
        var boundsRect = new Rectangle(Bounds.Location - (new Vector2(boundsSize, boundsSize)).ToPoint() + offset, Bounds.Size + new Point(boundsSize*2, boundsSize*2));
        spriteBatch.DrawRectangle(boundsRect, !Pressed ? Color.Black : Color.Gray,boundsSize,DrawOrder);
        spriteBatch.FillRectangle(fillRect, !Hovered ? Globals.PrimaryColor : Globals.SecondaryColor,DrawOrder);
    }
}
