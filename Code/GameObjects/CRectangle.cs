using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.GameObjects;

public class CRectangle
{
    private Vector2 position;
    public Vector2 Position 
    { 
        get => position;
        set { 
            position = value; 
            RectangleF = new RectangleF(position + Globals.Offset((int)RectangleF.Width, (int)RectangleF.Height), Size);
        } 
    }
    public RectangleF RectangleF { get; set; }
    public Vector2 Size { get; set; }
    public CRectangle(Vector2 position, int width, int height)
    {
        Size = new Vector2(width, height);
        RectangleF = new RectangleF(position + Globals.Offset(width, height), Size);
        Position = position;
    }

    public CRectangle(Point position, Point size) : this(position.ToVector2(), size.X, size.Y) { }

    public CRectangle(Vector2 position, Vector2 size) : this(position, (int)size.X, (int)size.Y) { }

    public float Top => RectangleF.Top;
    public float Bottom => RectangleF.Bottom;
    public float Left => RectangleF.Left;
    public float Right => RectangleF.Right;
    public float Width => RectangleF.Width;
    public float Height => RectangleF.Height;
}
