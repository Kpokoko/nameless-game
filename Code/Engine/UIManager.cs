using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public void Update(GameTime gameTime)
    {
        for (var i = 0; i < Buttons.Count; i++)
            Buttons[i].Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Buttons.Count; i++)
            Buttons[i].Draw(spriteBatch);
    }
}
