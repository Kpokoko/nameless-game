using MonoGame.Extended;
using nameless.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class Spawner : Pivot
{
    public Vector2 Direction { get; private set; } = Vector2.UnitX;
    public float Speed { get; private set; } = 1;
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
    public TimerTriggerAutoStart Timer { get; private set; }
    public Vector2 SpawnOffset { get; private set; } = Vector2.Zero;

    public Spawner(int x, int y, Vector2 dir, float speed, 
        float intervalMillisec = 1000, Vector2 spawnOffset = new Vector2()) : base(x, y)
    {
        Direction = dir;
        Speed = speed;
        SpawnOffset = spawnOffset;
        Interval = intervalMillisec;
        PrepareSerializationInfo();
    }

    public void SetMovement(Vector2 dir, float speed)
    {
        Direction = dir.NormalizedCopy();
        Speed = speed;
        PrepareSerializationInfo();
    }

    public void SetInterval(float interval)
    {
        Interval = interval;
        PrepareSerializationInfo();
    }

    public void SetSpawnOffset(Vector2 offset)
    {
        SpawnOffset = offset;
        PrepareSerializationInfo();
    }

    private void ResetInterval()
    {
        if (Timer != null)
            Timer.RemoveTimer();
        Timer = new TimerTriggerAutoStart(Interval, SignalProperty.Continuous);
        Timer.Start();
        Timer.OnTimeoutEvent += SpawnMovingBlock;
    }

    public void SpawnMovingBlock()
    {
        var mb = new MovingBlock(TilePosition + SpawnOffset, Direction, Speed);
        mb.AllowSerialization = false;
        Globals.SceneManager.GetEntities().Add(mb);
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
        Info.Offset = SpawnOffset;
        Info.Speed = Speed;
        Info.Direction = Direction;
    }
}
