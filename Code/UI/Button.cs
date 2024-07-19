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

public class Button : UIElement, IEntity, IUI
{
    public event Action OnClickEvent;
    public Label Label { get; set; }
    public bool Hovered { get { return MouseInputController.MouseBounds.Intersects(Bounds); } }
    public bool Pressed { get; set; }
    public bool Activated { get; set; } = false;
    public ButtonActivationProperty ActivatedProperty { get; set; }
    private Keys Key { get; set; }

    public Button
        (Vector2 position, int width, int height, string text = null, 
        ButtonActivationProperty property = ButtonActivationProperty.Click, Alignment align = Alignment.Center) 
        : base(position, width, height, align)
    {
        Globals.UIManager.Buttons.Add(this);

        if (text != null)
            SetText(new Label(Vector2.Zero,text));
        ActivatedProperty = property;
    }

    /// <summary>
    /// Set Text label with relative position
    /// </summary>
    /// <param name="label"></param>
    public void SetText(Label label)
    {
        label.Position = label.Position + Position;
        Label = label;
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
    }

    public void Update(GameTime gameTime)
    {
        if (ActivatedProperty is ButtonActivationProperty.Click)
            Pressed = false;
        if ((Hovered && MouseInputController.LeftButton.IsJustPressed) || (Activated && MouseInputController.LeftButton.IsPressed))
        {
            Activate();
        }
        else
        {
            if (ActivatedProperty is ButtonActivationProperty.Click) Activated = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var offset = !Pressed ? Point.Zero : new Point(0,3);
        var boundsSize = 6;
        var fillRect = new Rectangle(Bounds.Location + offset,Bounds.Size);
        var boundsRect = new Rectangle(Bounds.Location - (new Vector2(boundsSize, boundsSize)).ToPoint() + offset, Bounds.Size + new Point(boundsSize*2, boundsSize*2));
        spriteBatch.DrawRectangle(boundsRect, !Pressed ? Color.Black : Color.Gray,boundsSize,0.9f);
        spriteBatch.FillRectangle(fillRect, (!Hovered) ? Globals.PrimaryColor : Globals.SecondaryColor,0.9f);
    }
}
