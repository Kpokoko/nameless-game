using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using nameless.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class UIManager
{
    public List<Button> Buttons = new List<Button>();
    public List<Label> Labels = new List<Label>();
    public Dictionary<Keys, Button> KeyboardButtons = new();
    public SpriteFont Font;
    public void Update(GameTime gameTime)
    {
        for (var i = 0; i < Buttons.Count; i++)
            Buttons[i].Update(gameTime);

        for (var i = 0; i < KeyboardButtons.Count; i++)
        {
            if (Keyboard.GetState().IsKeyDown(KeyboardButtons.Keys.ToArray()[i]))
                KeyboardButtons.Values.ToArray()[i].Activate();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Buttons.Count; i++)
            Buttons[i].Draw(spriteBatch);
        for (var i = 0; i < Labels.Count; i++)
            Labels[i].Draw(spriteBatch);
    }

    public void Clear()
    {
        Buttons.Clear();
        Labels.Clear();
        KeyboardButtons.Clear();
    }
}
