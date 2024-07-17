using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Controls;

public class LeftButton
{
    public bool IsPressed
    { get { return MouseInputController.MouseState.LeftButton is ButtonState.Pressed; } }

    public bool IsJustPressed
    { get { return (MouseInputController.PreviousMouseState.LeftButton is ButtonState.Released && MouseInputController.MouseState.LeftButton is ButtonState.Pressed); } }

    public bool IsJustReleased
    { get { return (MouseInputController.PreviousMouseState.LeftButton is ButtonState.Pressed && MouseInputController.MouseState.LeftButton is ButtonState.Released); } }
}
