using Microsoft.Xna.Framework;
using MonoGame.Extended;
using nameless.Controls;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public abstract class UIElement
{
    protected UIElement(Vector2 position, int width, int height, Alignment align)
    {
        Size = new Vector2(width, height);
        Alignment = align;
        Position = position;
    }

    protected UIElement(Vector2 position, Vector2 size, Alignment align) : this(position, (int)size.X, (int)size.Y, align)
    {}

    public Vector2 Position
    {
        get { return _position; }
        set { _position = value;
            if (Size == null) throw new ArgumentNullException(nameof(Size));
            Bounds = new Rectangle((_position + _offset + ParentPosition + Globals.Offset((int)Size.X, (int)Size.Y)).ToPoint(),new Point((int)Size.X,(int)Size.Y));
        }
    }
    private Vector2 _position;
    public Vector2 ParentPosition { get; set; }
    public Vector2 AbsolutePosition { get { return Position + ParentPosition; } }
    private Vector2 Size { get; set; }
    public Vector2 TilePosition { get; set; }
    public int DrawOrder => 1;
    public Rectangle Bounds { get; set; }
    protected Alignment Alignment { 
        set {
            if (value == Alignment.Center) _offset = Vector2.Zero;
            if (value == Alignment.CenterLeft) _offset = -Globals.Offset((int)Size.X, (int)Size.Y).SetY(0);
        } }
    public Vector2 _offset { get; set; }
    public bool Hovered { get { return MouseInputController.MouseBounds.Intersects(Bounds); } }

    public virtual void UpdatePosition()
    {
        Position = Position;
    }

    public abstract void Remove();
}
