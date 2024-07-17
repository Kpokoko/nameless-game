using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using nameless.Interfaces;

namespace nameless.GameObjects;

public class Timer 
{
    private double _delayTime;
    private double _timeLeft;
    public event Action OnTimeoutEvent;
    private SignalProperty signalProperty;
    public string TimeLeftSeconds { get { return (_timeLeft / 1000).ToString(); } }

    public Timer(double delayTimeMilliseconds,SignalProperty signal)
    {
        _delayTime = delayTimeMilliseconds;
        _timeLeft = delayTimeMilliseconds;
        signalProperty = signal;
    }

    public static void DelayEvent(double delayTimeMilliseconds, Action action)
    {
        var timer = new Timer(delayTimeMilliseconds,SignalProperty.Once);
        timer.OnTimeoutEvent += action;
    }

    public void Reset()
    {
        _timeLeft = _delayTime;
    }

    public void Update(GameTime gameTime)
    {
        if (_timeLeft <= 0)
        {
            OnTimeoutEvent.Invoke();

            if (signalProperty is SignalProperty.Continuous)
                Reset();
        }
        _timeLeft -= gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    public void AddTime()
    {
        _timeLeft += _delayTime;
    }
}

