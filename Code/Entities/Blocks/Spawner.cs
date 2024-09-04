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
    public TimerTriggerAutoStart Timer { get; private set; } = new TimerTriggerAutoStart(1000,SignalProperty.Continuous);
    public Vector2 SpawnOffset = Vector2.Zero;

    public Spawner(int x, int y, Vector2 dir, float speed) : base(x, y)
    {
        Direction = dir;
        Speed = speed;
        PrepareSerializationInfo();
        Timer.Start();
        Timer.OnTimeoutEvent += SpawnMovingBlock;
    }

    public void SetMovement(Vector2 dir, float speed)
    {
        Direction = dir.NormalizedCopy();
        Speed = speed;
        PrepareSerializationInfo();
    }

    public void SetInterval(float interval)
    {
        Timer.RemoveTimer();
        Timer = new TimerTriggerAutoStart(interval,SignalProperty.Continuous);
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
}
