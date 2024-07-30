using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Entity;
using MonoGame.Extended.Collisions;
using nameless.Collisions;
using nameless.UI.Scenes;

namespace nameless.Controls;

public class PlayerInputController
{
    private KeyboardState _previousKeyboardState;
    private PlayerModel _player;

    public PlayerInputController(PlayerModel player)
    {
        _player = player;
    }

    public void ProcessControls(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        bool isJumpKeyPressed = false;
        bool isJumpKeyHolding = false;

        if (_previousKeyboardState != null)
        {
            isJumpKeyHolding = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Space);
            isJumpKeyPressed = isJumpKeyHolding && !(_previousKeyboardState.IsKeyDown(Keys.Up) || _previousKeyboardState.IsKeyDown(Keys.W) || _previousKeyboardState.IsKeyDown(Keys.Space));
        }


        if (Globals.IsNoclipEnabled)
        {
            MeasureNoclip(keyboardState, isJumpKeyHolding);
        }
        else
        {
            MeasureMovement(keyboardState, isJumpKeyPressed, isJumpKeyHolding);
        }




        if (Keyboard.GetState().IsKeyDown(Keys.N) && !_previousKeyboardState.IsKeyDown(Keys.N) && Globals.IsDeveloperModeEnabled)
        {
            SwitchNoclip();

        }

        if ((Globals.OnEditorBlock || Globals.IsConstructorModeEnabled || Globals.IsDeveloperModeEnabled) 
            && !_previousKeyboardState.IsKeyDown(Keys.E) && keyboardState.IsKeyDown(Keys.E))
        {
            Globals.Constructor.SwitchMode();
        }

        _player.Actions.Push(_player.UpdateNoclip);
        _previousKeyboardState = keyboardState;
    }

    private void SwitchNoclip()
    {
        Globals.IsNoclipEnabled = Globals.IsNoclipEnabled ? false : true;
    }

    private void MeasureMovement(KeyboardState keyboardState, bool isJumpKeyPressed, bool isJumpKeyHolding)
    {
        if (isJumpKeyPressed)
        {
            _player.Actions.Push(_player.TryJump);
        }
        else if (_player.State == PlayerState.Jumping && !isJumpKeyHolding)
        {
            _player.Actions.Push(_player.CancelJump);
        }
        else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
        {
            //if (_player.State != PlayerState.Still)
            //    _player.Drop();
        }

        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
        {
            _player.Actions.Push(_player.MoveLeft);
        }
        else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
        {
            _player.Actions.Push(_player.MoveRight);
        }
        else
            _player.Actions.Push(_player.Stop);
    }

    private void MeasureNoclip(KeyboardState keyboardState, bool isJumpKeyHolding)
    {
        if (isJumpKeyHolding)
        {
            _player.Actions.Push(_player.Up);
        }
        else if (keyboardState.IsKeyDown(Keys.S))
        {
            _player.Actions.Push(_player.Down);
        }
        else if (!isJumpKeyHolding && !keyboardState.IsKeyDown(Keys.S))
        {
            _player.Actions.Push(_player.StopVertical);
        }

        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
        {
            _player.Actions.Push(_player.Left);
        }
        else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
        {
            _player.Actions.Push(_player.Right);
        }
        else
            _player.Actions.Push(_player.StopHorizontal);
    }
}
