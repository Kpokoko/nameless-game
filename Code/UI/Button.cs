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
using Microsoft.Xna.Framework.Input;

namespace nameless.UI;

public class Button : IEntity, IUI
{
    public Vector2 Position { get; set; }

    public Vector2 TilePosition { get; set; }

    public int DrawOrder => 0;

    public event Action OnClickEvent;
    private Rectangle Bounds { get; set; }

    public Label Label { get; set; }
    public bool Hovered { get { return MouseInputController.MouseBounds.Intersects(Bounds); } }
    public bool Pressed { get; set; }
    public bool Activated { get; set; } = false;
    public ActivatedProperty ActivatedProperty { get; set; }
    private Keys Key { get; set; }

    public Button(Vector2 position, int width, int height, string text = null, ActivatedProperty property = ActivatedProperty.WhilePressing) 
    {
        Position = position;
        Bounds = new Rectangle((Globals.Offset(width,height) + position).ToPoint(), new Size(width,height));

        Globals.UIManager.Buttons.Add(this);

        if (text != null)
            SetText(text);
        ActivatedProperty = property;
    }

    public void SetText(string text)
    {
        Label = new Label(Position,text);
    }

    public void SetKeyboardKey(Keys key)
    {
        Globals.UIManager.KeyboardButtons[key] = this;
        Key = key;
    }

    public void Deactivate()
    {
        Activated = false;
        Pressed = false;
    }

    public void Activate()
    {
        Pressed = true;
        if (Activated) return;
        if (OnClickEvent != null)
            OnClickEvent.Invoke();
        Activated = true;
    }

    public void Remove()
    {
        Globals.UIManager.Buttons.Remove(this);
        if (Globals.UIManager.KeyboardButtons.ContainsKey(Key))
            Globals.UIManager.KeyboardButtons.Remove(Key);
        if (Label != null)
            Label.Remove();
            //Globals.UIManager.KeyboardButtons = Globals.UIManager.KeyboardButtons.Where(p => p.Value != this).ToDictionary(p=>p.Key,p=>p.Value);
    }

    public void Update(GameTime gameTime)
    {
        if (ActivatedProperty is ActivatedProperty.WhilePressing)
            Pressed = false;
        if ((Hovered && MouseInputController.LeftButton.IsJustPressed) || (Activated && MouseInputController.LeftButton.IsPressed))
        {
            Activate();
        }
        else
        {
            if (ActivatedProperty is ActivatedProperty.WhilePressing) Activated = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var offset = !Pressed ? Point.Zero : new Point(0,3);
        var boundsSize = 6;
        var fillRect = new Rectangle(Bounds.Location + offset,Bounds.Size);
        var boundsRect = new Rectangle(Bounds.Location - (new Vector2(boundsSize, boundsSize)).ToPoint() + offset, Bounds.Size + new Point(boundsSize*2, boundsSize*2));
        spriteBatch.DrawRectangle(boundsRect, !Pressed ? Color.Black : Color.Gray,boundsSize,DrawOrder);
        spriteBatch.FillRectangle(fillRect, (!Hovered) ? Globals.PrimaryColor : Globals.SecondaryColor,DrawOrder);
    }
}
