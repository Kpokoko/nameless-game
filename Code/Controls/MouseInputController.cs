using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using nameless.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Tiles;

namespace nameless.Controls;

public static class MouseInputController
{
    public static MouseState MouseState { get; private set; } = new MouseState();
    public static Vector2 MousePos { get; private set; } = new Vector2(0, 0);
    public static Vector2 MouseTilePos { get { return Tile.GetPosInTileCoordinats(MousePos); } }

    public static MouseState PreviousMouseState { get; private set; } = new MouseState();
    public static Vector2 PreviousMousePos { get; private set; } = new Vector2(0, 0);
    public static Vector2 PreviousMouseTilePos { get { return Tile.GetPosInTileCoordinats(PreviousMousePos); } }

    public static Rectangle MouseBounds { get { return new Rectangle((int)MousePos.X, (int)MousePos.Y, 1, 1); } }

    public static bool OnUIElement {  get; private set; }

    private static bool OnUIBuffer { get; set; }


    public static LeftButton LeftButton = new();

    public static RightButton RightButton = new();

    public static MiddleButton MiddleButton = new();


    public static void ProcessControls()
    {
        PreviousMousePos = MousePos;
        PreviousMouseState = MouseState;
        MouseState = Mouse.GetState();
        MousePos = new Vector2(MouseState.X, MouseState.Y);

        OnUIElement = false;
        if (OnUIBuffer)
        {
            OnUIElement = true;
            OnUIBuffer = false;
        }
    }

    public static void SetOnUIState()
    {
        OnUIBuffer = true;
    }
}
