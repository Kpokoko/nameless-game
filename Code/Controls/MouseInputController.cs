using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using nameless.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Tiles;
using MonoGame.Extended;
using nameless.UI;

namespace nameless.Controls;

public static class MouseInputController
{
    public static MouseState MouseState { get; private set; } = new MouseState();
    public static Vector2 MousePos { get; private set; } = new Vector2(0, 0);
    public static Vector2 TransformedMousePos { get; private set; } = new Vector2(0, 0);
    public static Vector2 MouseTilePos { get { return Tile.GetPosInTileCoordinats(TransformedMousePos); } }

    public static MouseState PreviousMouseState { get; private set; } = new MouseState();
    public static Vector2 PreviousTransformedMousePos { get; private set; } = new Vector2(0, 0);
    public static Vector2 PreviousMousePos { get; private set; } = new Vector2(0, 0);
    public static Vector2 PreviousMouseTilePos { get { return Tile.GetPosInTileCoordinats(PreviousTransformedMousePos); } }

    public static RectangleF MouseBounds { get { return new RectangleF(MousePos.X, MousePos.Y, 1, 1); } }

    public static bool OnUIElement {  get; private set; }

    private static List<UIElement> OnUIBuffer { get; set; } = new();
    public static List<UIElement> OnUIElementsList { get; set; } = new();


    public static bool IsJustPressed {
        get { return LeftButton.IsJustPressed || RightButton.IsJustPressed; }}

    public static bool IsPressed {
        get { return LeftButton.IsPressed || RightButton.IsPressed; }}

    public static LeftButton LeftButton = new();

    public static RightButton RightButton = new();

    public static MiddleButton MiddleButton = new();


    public static void ProcessControls()
    {
        PreviousTransformedMousePos = TransformedMousePos;
        PreviousMousePos = MousePos;
        PreviousMouseState = MouseState;
        MouseState = Mouse.GetState();
        TransformedMousePos = new Vector2(MouseState.X, MouseState.Y) / Globals.Camera.Zoom;
        MousePos = new Vector2(MouseState.X, MouseState.Y) / Globals.Camera.Zoom;

        OnUIElement = false;
        OnUIElementsList.Clear();
        if (OnUIBuffer.Any())
        {
            OnUIElement = true;
            OnUIElementsList = OnUIBuffer.ToList();
            OnUIBuffer.Clear();
        }
    }

    public static void SetOnUIState(UIElement element)
    {
        OnUIBuffer.Add(element);
    }
}
