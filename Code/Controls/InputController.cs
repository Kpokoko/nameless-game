using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Entity;

namespace nameless.Controls
{
    public class InputController
    {
        private KeyboardState _previousKeyboardState;
        private PlayerModel _player;

        public InputController(PlayerModel player)
        {
            _player = player;
        }

        public void ProcessControls(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            bool isJumpKeyPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W);

            if (isJumpKeyPressed)
            {
                if (_player.State == PlayerState.Still)
                    _player.BeginJump();
            }
            else if (_player.State == PlayerState.Jumping && !isJumpKeyPressed)
            {
                _player.CancelJump();
            }
            else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                if (_player.State != PlayerState.Still)
                    _player.Drop();
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                _player.MoveLeft();
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                _player.MoveRight();
            }
            else
                _player.Stop();
            _previousKeyboardState = keyboardState;
        }
    }
}
