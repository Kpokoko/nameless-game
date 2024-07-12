using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using nameless.Collisions;
using nameless.GameObjects;

namespace nameless.Engine;

public class TriggerManager
{
    public List<HitboxTrigger> TriggerHitboxes = new List<HitboxTrigger>();

    public List<TimerTrigger> Timers = new List<TimerTrigger>();

    public void Update(GameTime gameTime)
    {
        for (var i = 0; i < TriggerHitboxes.Count; i++)
            TriggerHitboxes[i].UpdateActivation();
        for (var i = 0; i < Timers.Count; i++)
            Timers[i].Update(gameTime);
    }
}
