using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Controls;

public class MiddleButton
{
    public bool IsPressed
    { get { return MouseInputController.MouseState.MiddleButton is ButtonState.Pressed; } }

    public bool IsJustPressed
    { get { return (MouseInputController.PreviousMouseState.MiddleButton is ButtonState.Released && MouseInputController.MouseState.MiddleButton is ButtonState.Pressed); } }

    public bool IsJustReleased
    { get { return (MouseInputController.PreviousMouseState.MiddleButton is ButtonState.Pressed && MouseInputController.MouseState.MiddleButton is ButtonState.Released); } }
}
