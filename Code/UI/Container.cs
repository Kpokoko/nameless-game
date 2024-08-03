using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Controls;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class Container : UIElement, IEntity
{
    private FlexDirection _flexDirection {  get; set; }
    private Vector2 _padding { get; set; }
    public List<UIElement> Elements { get; set; } = new();
    public Container(Vector2 position, int width, int height, Alignment align = Alignment.Center, FlexDirection flexDir = FlexDirection.Horizontal, Vector2 padding = new Vector2()) : base(position, width, height, align)
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
            Elements[0].ParentPosition = Position;
            Elements[0].UpdatePosition();
            return;
        }

        float x; float y;
        float justifiedSpaceBetween; float spaceBetween;
        if (_flexDirection == FlexDirection.Vertical)
        {
            y = Bounds.Top + _padding.Y;
            x = Bounds.Left + Bounds.Width / 2;
            justifiedSpaceBetween = (Bounds.Height - _padding.Y * 2 - Elements[0].Bounds.Height * Elements.Count) / (Elements.Count + 1);
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].ParentPosition = new Vector2(x, y + justifiedSpaceBetween * (i + 1) + ((i + 1) * 2 - 1) * Elements[0].Bounds.Height / 2);
                Elements[i].UpdatePosition();
            }
        }
        else if (_flexDirection == FlexDirection.Horizontal)
        {
            y = Bounds.Top + Bounds.Height / 2;
            x = Bounds.Left + _padding.X;
            justifiedSpaceBetween = (Bounds.Width - _padding.X * 2 - Elements[0].Bounds.Width * Elements.Count) / (Elements.Count + 1);
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].ParentPosition = new Vector2(x + justifiedSpaceBetween * (i + 1) + ((i + 1) * 2 - 1) * Elements[0].Bounds.Width / 2, y);
                Elements[i].UpdatePosition();
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
            if (button == clickedButton)
            {

            }
            else
            {
                button.Deactivate();
            }
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
        if (Hovered)
        {
            MouseInputController.SetOnUIState();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var boundsSize = 6;
        var fillRect = new Rectangle(Bounds.Location, Bounds.Size);
        var boundsRect = new Rectangle(Bounds.Location - (new Vector2(boundsSize, boundsSize)).ToPoint() , Bounds.Size + new Point(boundsSize * 2, boundsSize * 2));
        spriteBatch.DrawRectangle(boundsRect, Color.Black, boundsSize, 0.89f);
        spriteBatch.FillRectangle(fillRect, Globals.PrimaryColor , 0.89f);
    }
}
