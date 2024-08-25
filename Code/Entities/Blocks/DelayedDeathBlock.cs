using nameless.Collisions;
using nameless.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class DelayedDeathBlock : Block
{
    private bool _isBlockAggressive = false;
    private TimerTrigger _timer;
    public DelayedDeathBlock(int x, int y) : base(x, y)
    {
        _timer = new TimerTrigger(500, GameObjects.SignalProperty.Once);
        var trigger = new HitboxTrigger(this, 65, 65, ReactOnProperty.ReactOnEntityType, Collisions.SignalProperty.Continuous);
        trigger.SetTriggerEntityTypes(typeof(PlayerModel), typeof(MovingBlock));
        _timer.OnTimeoutEvent += () => { _isBlockAggressive = true; };
        trigger.OnCollisionEvent += () =>
        {
            _timer.Start();
            if (_isBlockAggressive && !(Colliders.colliders.Count > 2))
            {
                var damageTrigger = HitboxTrigger.CreateDamagePlayerTrigger(this, 65, 65);
                Colliders.Add(damageTrigger);
            }
        };
        trigger.OnCollisionExitEvent += () =>
        {
            _timer.Stop();
        };
        Colliders.Add(trigger);
        Colliders[0].Color = Color.Red;
        IsEnableToPlayer = true;
    }
}
