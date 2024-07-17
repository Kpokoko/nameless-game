using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Controls;

public class RightButton
{
    public bool IsPressed
    { get { return MouseInputController.MouseState.RightButton is ButtonState.Pressed; } }

    public bool IsJustPressed
    { get { return (MouseInputController.PreviousMouseState.RightButton is ButtonState.Released && MouseInputController.MouseState.RightButton is ButtonState.Pressed); } }

    public bool IsJustReleased
    { get { return (MouseInputController.PreviousMouseState.RightButton is ButtonState.Pressed && MouseInputController.MouseState.RightButton is ButtonState.Released); } }
}
