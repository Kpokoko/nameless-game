using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace nameless.Engine;

internal class TriggerManager
{
    public static void Update(GameTime gameTime)
    {
        for (var i = 0; i < Globals.TriggerHitboxes.Count; i++)
            Globals.TriggerHitboxes[i].UpdateActivation();
    }
}
