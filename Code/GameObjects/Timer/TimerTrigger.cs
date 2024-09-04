using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using nameless.Interfaces;

namespace nameless.GameObjects;

public class TimerTrigger 
{
    public double DelayTime { get; private set; }
    public double TimeLeft { get; private set; }
    public event Action OnTimeoutEvent;
    private SignalProperty _signalProperty;
    public bool IsRunning { get; private set; } = false;
    public string TimeLeftSeconds { get { return (TimeLeft / 1000).ToString(); } }

    public TimerTrigger(double delayTimeMilliseconds,SignalProperty signal)
    {
        DelayTime = delayTimeMilliseconds;
        TimeLeft = delayTimeMilliseconds;
        _signalProperty = signal;
        Globals.TriggerManager.Timers.Add(this);
    }

    public TimerTrigger(double delayTimeMilliseconds, SignalProperty signal, Action action) : this(delayTimeMilliseconds, signal)
    {
        _signalProperty = SignalProperty.Continuous;
        OnTimeoutEvent = action;
    }

    public static void DelayEvent(double delayTimeMilliseconds, Action action)
    {
        var timer = new TimerTrigger(delayTimeMilliseconds,SignalProperty.Once);
        timer.OnTimeoutEvent += action;
        timer.Start();
    }

    public void Start()
    {
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false; 
    }

    public void Reset()
    {
        TimeLeft = DelayTime;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsRunning) return;

        if (TimeLeft <= 0)
        {
            if (OnTimeoutEvent != null)
                OnTimeoutEvent.Invoke();

            if (_signalProperty is SignalProperty.Continuous)
                Reset();
            else
            {
                IsRunning = false;
            }
                //RemoveTimer();
        }
        TimeLeft -= gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    public virtual void RemoveTimer()
    {
        Globals.TriggerManager.Timers.Remove(this);
    }

    public void AddTime()
    {
        TimeLeft += DelayTime;
    }
}

