﻿using Microsoft.Xna.Framework.Input;
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
    public static MouseState MouseState { get; private set; }
    public static Vector2 MousePos { get; private set; } = new Vector2(0, 0);
    public static Vector2 MouseTilePos { get { return Tile.GetPosInTileCoordinats(MousePos); } }
    public static Vector2 PreviousMousePos { get; private set; }
    public static Vector2 PreviousMouseTilePos { get { return Tile.GetPosInTileCoordinats(PreviousMousePos); } }

    public static MouseState PreviousMouseState { get; private set; }

    public static bool IsJustClickedLeft 
    { get { return (PreviousMouseState.LeftButton is ButtonState.Released && MouseState.LeftButton is ButtonState.Pressed); } }

    public static bool IsJustClickedRight
    { get { return (PreviousMouseState.RightButton is ButtonState.Released && MouseState.RightButton is ButtonState.Pressed); } }


    private static PlayerModel _player;

    public static void ProcessControls()
    {
        PreviousMousePos = MousePos;
        PreviousMouseState = MouseState;
        MouseState = Mouse.GetState();
        MousePos = new Vector2(MouseState.X, MouseState.Y);
    }
}
