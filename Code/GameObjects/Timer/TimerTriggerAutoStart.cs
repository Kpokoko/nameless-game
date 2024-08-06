using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.GameObjects;
public class TimerTriggerAutoStart : TimerTrigger
{
    public TimerTriggerAutoStart(double delayTime, SignalProperty signal) : base(delayTime, signal)
    {
        Globals.TriggerManager.Timers.Remove(this);
        Globals.TriggerManager.AutoTimers.Add(this);
    }
}
