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

namespace nameless.Controls
{
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

            bool isJumpKeyPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W);

            if (isJumpKeyPressed)
            {
                _player.Actions.Push(_player.TryJump);
            }
            else if (_player.State == PlayerState.Jumping && !isJumpKeyPressed)
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


            if ((Globals.OnEditorBlock || Globals.IsConstructorModeEnabled || Globals.IsDeveloperModeEnabled) 
                && !_previousKeyboardState.IsKeyDown(Keys.E) && keyboardState.IsKeyDown(Keys.E))
            {
                Globals.Constructor.SwitchMode();
            }
            _previousKeyboardState = keyboardState;
        }
    }
}
