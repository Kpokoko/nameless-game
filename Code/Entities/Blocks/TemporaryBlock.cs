using nameless.Collisions;
using nameless.GameObjects;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity
{
    public class TemporaryBlock : Block, IBreakable
    {
        public bool Broken { get; set; }
        public TemporaryBlock(int x, int y, int timeMs = 3000) : base(x, y)
        {
            var trigger = new TimerTriggerAutoStart(timeMs, GameObjects.SignalProperty.Once);
            trigger.OnTimeoutEvent += () => this.Break();
            trigger.Start();
            Colliders[0].Color = Color.DarkSeaGreen;
        }
        public TemporaryBlock(Vector2 tilePosition) : this((int)tilePosition.X, (int)tilePosition.Y) { }


        public void Break()
        {
            IsEnableToPlayer = false;
            Broken = true;
            Colliders.RemoveAll();
            IsDeleted = true;
            Globals.SceneManager.GetStorage().UpdateMovingBlocksState();
        }
    }
}
