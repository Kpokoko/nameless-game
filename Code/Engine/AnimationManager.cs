using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class AnimationManager
{
    public List<Graphics.AnimationHandler> Animations = new List<Graphics.AnimationHandler>();

    public void Update()
    {
        for (int i = 0; i < Animations.Count; i++)
            Animations[i].Update();
    }
}
