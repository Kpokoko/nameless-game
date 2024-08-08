using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Controls;
using nameless.GameObjects;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class Container : UIElement
{
    private FlexDirection _flexDirection {  get; set; }
    private Vector2 _padding { get; set; }

    private CRectangle BorderBounds;

    private bool Grabbed;

    public event Action OnDrag;

    public Container(Vector2 position, int width, int height, 
        FlexDirection flexDir = FlexDirection.Horizontal, Vector2 padding = new Vector2()) : base(position, width, height)
    {
        _flexDirection = flexDir;
        _padding = padding;

        Globals.UIManager.Containers.Add(this);
    }

    /// <summary>
    /// Add UI element with relative position
    /// </summary>
    public void AddElements(params UIElement[] elements)
    {
        Elements = Elements.Concat(elements).ToList();

        PlaceElements();
    }

    private void PlaceElements()
    {
        if (Elements.Count == 1)
        {
            Elements[0].ParentPosition = AbsolutePosition;
            return;
        }

        foreach (var element in Elements)
            element.ParentPosition = AbsolutePosition;

        float x; float y;
        float justifiedSpaceBetween; float spaceBetween;
        if (_flexDirection == FlexDirection.Vertical)
        {
            y = Bounds.Top + _padding.Y;
            x = Bounds.Left + Bounds.Width / 2;
            justifiedSpaceBetween = (Bounds.Height - _padding.Y * 2 - Elements[0].Bounds.Height * Elements.Count) / (Elements.Count + 1);
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Offset = new Vector2(x, y + justifiedSpaceBetween * (i + 1) + ((i + 1) * 2 - 1) * Elements[0].Bounds.Height / 2) - AbsolutePosition;
            }
        }
        else if (_flexDirection == FlexDirection.Horizontal)
        {
            y = Bounds.Top + Bounds.Height / 2;
            x = Bounds.Left + _padding.X;
            justifiedSpaceBetween = (Bounds.Width - _padding.X * 2 - Elements[0].Bounds.Width * Elements.Count) / (Elements.Count + 1);
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Offset = new Vector2(x + justifiedSpaceBetween * (i + 1) + ((i + 1) * 2 - 1) * Elements[0].Bounds.Width / 2, y) - AbsolutePosition;
            }
        }
        //var spaceBetween = (Bounds.Height - _padding.Y * 2) / (Elements.Count + 1);

        //for (int i = 0; i < Elements.Count; i++)
        //{
        //    Elements[i].ParentPosition = new Vector2(x ,y + spaceBetween * (i+1));
        //    Elements[i].UpdatePosition();
        //}
    }

    public void SwitchButtons(Button clickedButton)
    {
        var switchButtons = Elements
            .Where(e => e is Button)
            .Select(e => (Button)e)
            .Where(e =>e.ActivatedProperty is ButtonActivationProperty.Switch)
            .ToArray();
        for (int i = 0; i < switchButtons.Length; i++)
        {
            var button = switchButtons[i];
            if (button != clickedButton)
                button.Deactivate();
        }
    }

    public override void Remove()
    {
        Globals.UIManager.Containers.Remove(this);
        for (var i = 0; i < Elements.Count; i++) 
            Elements[i].Remove();
    }

    public void Update(GameTime gameTime)
    {
        if (MouseInputController.LeftButton.IsJustReleased)
        {
            Grabbed = false;
        }

        if (Grabbed)
            Drag();

        if (!Hovered) return;
        MouseInputController.SetOnUIState(this);


        if (MouseInputController.OnUIElementsList.Count == 1
            && MouseInputController.OnUIElementsList.Contains(this)
            && MouseInputController.LeftButton.IsJustPressed)
        {
            Grabbed = true;
        }
    }

    private void Drag()
    {
        var delta = MouseInputController.MousePos - MouseInputController.PreviousMousePos;
        RelativePosition += delta;
        BorderBounds.Position += delta;
        if (OnDrag != null)
            OnDrag.Invoke();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var boundsSize = 5;
        var fillRect = Bounds.RectangleF;
        if (BorderBounds == null)
            BorderBounds = new CRectangle(Bounds.Position, Bounds.Size + new Vector2(boundsSize * 2, boundsSize * 2));

        spriteBatch.DrawRectangle(BorderBounds.RectangleF, Color.Black, boundsSize, 0.01f);
        spriteBatch.FillRectangle(Bounds.RectangleF, Globals.PrimaryColor, 0.01f);
    }
}
