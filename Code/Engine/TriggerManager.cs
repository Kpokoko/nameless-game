using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using nameless.Collisions;
using nameless.Entity;
using nameless.GameObjects;
using nameless.UI;

namespace nameless.Engine;

public class TriggerManager
{
    public List<HitboxTrigger> TriggerHitboxes = new List<HitboxTrigger>();

    public List<TriggerType> ActivatedTriggers = new List<TriggerType>();

    public List<TimerTrigger> Timers = new List<TimerTrigger>();

    public List<TimerTrigger> AutoTimers = new List<TimerTrigger>();

    public void Update(GameTime gameTime)
    {
        //if (Globals.IsConstructorModeEnabled) return;
        for (var i = 0; i < Timers.Count; i++)
            Timers[i].Update(gameTime);
        for (var i = 0; i < TriggerHitboxes.Count; i++)
            TriggerHitboxes[i].UpdateActivation();
        if (!Globals.IsConstructorModeEnabled)
            for (var i = 0; i < AutoTimers.Count; i++)
                AutoTimers[i].Update(gameTime);
        else
            for (var i = 0; i < AutoTimers.Count; i++)
                AutoTimers[i].Reset();
        ActivatedTriggers.Clear();
    }
}
