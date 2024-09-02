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
    private double _delayTime;
    private double _timeLeft;
    public event Action OnTimeoutEvent;
    private SignalProperty signalProperty;
    public bool IsRunning { get; private set; } = false;
    public string TimeLeftSeconds { get { return (_timeLeft / 1000).ToString(); } }

    public TimerTrigger(double delayTimeMilliseconds,SignalProperty signal)
    {
        _delayTime = delayTimeMilliseconds;
        _timeLeft = delayTimeMilliseconds;
        signalProperty = signal;
        Globals.TriggerManager.Timers.Add(this);
    }

    public TimerTrigger(double delayTimeMilliseconds, SignalProperty signal, Action action) : this(delayTimeMilliseconds, signal)
    {
        signalProperty = SignalProperty.Continuous;
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
        _timeLeft = _delayTime;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsRunning) return;

        if (_timeLeft <= 0)
        {
            if (OnTimeoutEvent != null)
                OnTimeoutEvent.Invoke();

            if (signalProperty is SignalProperty.Continuous)
                Reset();
            else
            {
                IsRunning = false;
            }
                //RemoveTimer();
        }
        _timeLeft -= gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    public virtual void RemoveTimer()
    {
        Globals.TriggerManager.Timers.Remove(this);
    }

    public void AddTime()
    {
        _timeLeft += _delayTime;
    }
}

