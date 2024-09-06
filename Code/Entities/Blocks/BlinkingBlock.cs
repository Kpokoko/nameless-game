using MonoGame.Extended;
using nameless.Collisions;
using nameless.GameObjects;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class BlinkingBlock : Block, IBreakable
{
    TimerTriggerAutoStart Timer;
    float _interval;
    public float Interval 
    { 
        get => _interval; 
        private set
        {
            _interval = value;
            ResetInterval();
        } 
    }
    private bool _startBroken;
    public bool StartBroken 
    {
        get => _startBroken;
        private set
        {
            _startBroken = value;
            ResetStartState();
        } 
    }
    public bool Broken { get; set; }
    private bool _initializing = true;
    public BlinkingBlock(int x, int y, int intervalMs = 1500, bool broken = false) : base(x, y)
    {
        Interval = intervalMs;
        StartBroken = broken;

        Colliders[0].Color = Color.MonoGameOrange;

        Colliders.Add( new Collider(this, 64, 64));//TEMPORARY
        Globals.CollisionManager.CollisionComponent.Remove(Colliders[1]);
        Colliders[1].Color = Color.MonoGameOrange * 0.2f;

        _initializing = false;
        PrepareSerializationInfo();
    }

    public BlinkingBlock(Vector2 tilePosition) : this((int)tilePosition.X, (int)tilePosition.Y) { }

    private void Blink()
    {
        if (!Broken)
            Break();
        else
            Unbreak();

        var dirVec = new Vector2(1, 0);
        for (int i = 0; i < 50; i++)
        {
            var p = new BlendingParticle(Position, dirVec / Interval * 1000, (int)Interval, Color.MonoGameOrange * 0.02f);
            p.SetSecondColor(Color.Transparent);
            dirVec = dirVec.Rotate((float)Math.PI / 25);
        }
    }

    private void Unbreak()
    {
        Broken = false;

        //Colliders.ActivateAll();
        Colliders[0].ActivateCollider();
    }

    public void Break()
    {
        IsEnableToPlayer = false;
        Broken = true;

        //Colliders.DeactivateAll();
        Colliders[0].RemoveCollider();//TEMPORARY

        if (!_initializing)
            Globals.SceneManager.GetStorage().UpdateMovingBlocksState();
    }

    public void SetInterval(float interval)
    {
        Interval = interval;
        PrepareSerializationInfo();
    }

    private void ResetInterval()
    {
        if (Timer != null)
            Timer.RemoveTimer();
        Timer = new TimerTriggerAutoStart(Interval, GameObjects.SignalProperty.Continuous);
        Timer.OnTimeoutEvent += () => this.Blink();
        Timer.Start();
    }


    public void SetStartState(bool broken)
    {
        StartBroken = broken;
        PrepareSerializationInfo();
    }

    private void ResetStartState()
    {
        if (StartBroken != Broken)
            Blink();
    }

    public override void Remove()
    {
        base.Remove();
        Timer.RemoveTimer();
    }


    public override void PrepareSerializationInfo()
    {
        base.PrepareSerializationInfo();
        Info.Interval = Interval;
        Info.Flag = StartBroken; 
    }
}
