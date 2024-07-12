using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using nameless.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Controls;

public static class MouseInputController
{
    public static MouseState MouseState { get; private set; }
    public static Vector2 MousePos { get; private set; } = new Vector2(0, 0);
    public static MouseState PreviousMouseState { get; private set; }
    public static Vector2 PreviousMousePos { get; private set; }

    private static PlayerModel _player;

    public static void ProcessControls()
    {
        PreviousMousePos = MousePos;
        PreviousMouseState = MouseState;
        MouseState = Mouse.GetState();
        MousePos = new Vector2(MouseState.X, MouseState.Y);
    }
}
