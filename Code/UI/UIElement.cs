using Microsoft.Xna.Framework;
using MonoGame.Extended;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class UIElement
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
            Bounds = new Rectangle((_position + _offset + Globals.Offset((int)Size.X, (int)Size.Y)).ToPoint(),new Point((int)Size.X,(int)Size.Y));
        }
    }
    private Vector2 _position;
    private Vector2 Size { get; set; }
    public Vector2 TilePosition { get; set; }
    public int DrawOrder => 1;
    public Rectangle Bounds { get; set; }
    protected Alignment Alignment { 
        set {
            if (value == Alignment.Center) _offset = Vector2.Zero;
            if (value == Alignment.Left) _offset = -Globals.Offset((int)Size.X, (int)Size.Y).SetY(0);
        } }
    private Vector2 _offset { get; set; }
}
