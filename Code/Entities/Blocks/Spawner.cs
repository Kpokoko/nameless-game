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
    private Vector2 _direction;
    private float _speed;
    private TimerTriggerAutoStart _timer = new TimerTriggerAutoStart(2000,SignalProperty.Continuous);

    public Spawner(int x, int y, Vector2 dir, float speed) : base(x, y)
    {
        _direction = dir;
        _speed = speed;
        PrepareSerializationInfo();
    }

    public void SetMovement(Vector2 dir, float speed)
    {
        _direction = dir.NormalizedCopy();
        _speed = speed;
        PrepareSerializationInfo();
    }

    public void SetInterval(float interval)
    {
        _timer = new TimerTriggerAutoStart(interval,SignalProperty.Continuous);
        _timer.Start();
        _timer.OnTimeoutEvent += SpawnMovingBlock;
    }

    public void SpawnMovingBlock()
    {
        var mb = new MovingBlock(TilePosition, _direction, _speed);
        mb.AllowSerialization = false;
        Globals.SceneManager.GetEntities().Add(mb);
    }

    public override void Remove()
    {
        base.Remove();
        _timer.RemoveTimer();
    }
}
