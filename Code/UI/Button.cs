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
using System.Reflection.Emit;
using nameless.GameObjects;

namespace nameless.UI;

public class Button : UIElement
{
    public event Action OnClickEvent;
    //private Rectangle Bounds { get; set; }

    public bool Pressed { get; set; }
    public bool Activated { get; set; } = false;
    public ButtonActivationProperty ActivatedProperty { get; set; }
    private Keys Key { get; set; }
    private CRectangle BorderBounds;

    public Button
        (Vector2 position, float width, float height, string text = null, 
        ButtonActivationProperty property = ButtonActivationProperty.Click) 
        : base(position, width, height)
    {
        Globals.UIManager.Buttons.Add(this);

        if (text != null)
            SetText(new Label(Vector2.Zero,text));
        ActivatedProperty = property;
    }

    /// <summary>
    /// Set Text label with relative position
    /// </summary>
    public void SetText(Label label)
    {
        label.ParentPosition = AbsolutePosition;
        Elements.Add(label);
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

    override public void Remove()
    {
        Globals.UIManager.Buttons.Remove(this);
        if (Globals.UIManager.KeyboardButtons.ContainsKey(Key))
            Globals.UIManager.KeyboardButtons.Remove(Key);
        if (Elements.Any())
            Elements[0].Remove();
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

        if (Hovered)
            MouseInputController.SetOnUIState(this);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var offset = !Pressed ? Vector2.Zero : new Vector2(0,3);

        var boundsSize = 5;
        var fillRect = Bounds.RectangleF;
        if (BorderBounds == null)
            BorderBounds = new CRectangle(Bounds.Position - new Vector2(boundsSize, boundsSize) + offset, Bounds.Size + new Vector2(boundsSize*2, boundsSize*2));

        Bounds.Position = AbsolutePosition + offset;
        BorderBounds.Position = AbsolutePosition + offset;

        spriteBatch.DrawRectangle(BorderBounds.RectangleF, !Pressed ? Color.Black : Color.Gray,boundsSize, 0.02f);
        spriteBatch.FillRectangle(Bounds.RectangleF, (!Hovered) ? Globals.PrimaryColor : Globals.SecondaryColor, 0.02f);
    }
}
