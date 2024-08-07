using Microsoft.Xna.Framework;
using MonoGame.Extended;
using nameless.Controls;
using nameless.GameObjects;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public abstract class UIElement
{
    protected UIElement(Vector2 position, float width, float height)
    {
        Size = new Vector2(width, height);
        Bounds = new CRectangle(position, Size);
        RelativePosition = position;
    }

    protected UIElement(Vector2 position, Vector2 size) : this(position, (int)size.X, (int)size.Y)
    {}

    private Vector2 _relativePosition;
    public virtual Vector2 RelativePosition
    {
        get { return _relativePosition; }
        set 
        { 
            _relativePosition = value;
            Bounds.Position = AbsolutePosition;
            foreach (var e in Elements)
                e.ParentPosition = AbsolutePosition;
        }
    }
    private Vector2 _parentPosition;
    public Vector2 ParentPosition {
        get { return _parentPosition; }
        set 
        { 
            _parentPosition = value;
            Bounds.Position = AbsolutePosition;
            foreach (var e in Elements)
                e.ParentPosition = AbsolutePosition;
        }
    }
    public Vector2 AbsolutePosition { get { return RelativePosition + ParentPosition + Offset; } }
    public List<UIElement> Elements { get; set; } = new();
    protected Vector2 Size { get; set; }
    public int DrawOrder => 1;
    public CRectangle Bounds { get; set; }
    public Vector2 Offset { get; set; }
    public Alignment Alignment
    {
        set
        {
            if (value == Alignment.Center) Offset = Vector2.Zero;
            if (value == Alignment.CenterLeft) Offset = -Globals.Offset((int)Size.X, (int)Size.Y).SetY(0);
        }
    }
    public bool Hovered { get { return MouseInputController.MouseBounds.Intersects(Bounds.RectangleF); } }

    public abstract void Remove();
}
