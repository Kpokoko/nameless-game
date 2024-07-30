using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using nameless.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class AnimationManager
{
    public List<Graphics.AnimationHandler> AnimationHandlers = new List<Graphics.AnimationHandler>();
    public List<SpriteAnimation> SpriteAnimations= new List<SpriteAnimation>();

    public void Update(GameTime gameTime)
    {
        for (int i = 0; i < SpriteAnimations.Count; i++)
            SpriteAnimations[i].Update(gameTime);
        for (int i = 0; i < AnimationHandlers.Count; i++)
            AnimationHandlers[i].Update(gameTime);
    }
}
