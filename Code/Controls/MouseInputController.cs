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
using nameless.Code.SceneManager;

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
    public static float OnUIElementDrawOrder { get; private set; }

    private static List<UIElement> OnUIBuffer { get; set; } = new();
    public static List<UIElement> OnUIElementsList { get; set; } = new();


    public static bool IsJustReleased {
        get { return LeftButton.IsJustReleased || RightButton.IsJustReleased || MiddleButton.IsJustReleased; }}
    
    public static bool IsJustPressed {
        get { return LeftButton.IsJustPressed || RightButton.IsJustPressed || MiddleButton.IsJustPressed; }}

    public static bool IsPressed {
        get { return LeftButton.IsPressed || RightButton.IsPressed || MiddleButton.IsPressed; }}

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
            OnUIElementDrawOrder = OnUIElementsList.Max(e => e.DrawOrder);

            OnUIBuffer.Clear();
        }
    }

    public static void SetOnUIState(UIElement element)
    {
        OnUIBuffer.Add(element);
    }

    public static Vector2 GetNearestTilePos()
    {
        var nearestTile = new Vector2(-1, -1);
        for (int x = (int)MouseTilePos.X - 1; x <= MouseTilePos.X + 1; ++x)
            for (int y = (int)MouseTilePos.Y - 1; y <= MouseTilePos.Y + 1; ++y)
            {
                var tile = new Vector2(x, y);
                if (!Storage.IsInBounds(tile) || tile == MouseTilePos)
                    continue;
                if ((Tile.GetTileCenter(tile) - MouseInputController.MousePos).Length() < (Tile.GetTileCenter(nearestTile) - MouseInputController.MousePos).Length())
                    nearestTile = tile;
            }

        return nearestTile;
    }
}
