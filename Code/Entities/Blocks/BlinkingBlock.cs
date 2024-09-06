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
    public class BlinkingBlock : Block, IBreakable
    {
        public bool Broken { get; set; }
        public BlinkingBlock(int x, int y, int intervalMs = 1500) : base(x, y)
        {
            var trigger = new TimerTriggerAutoStart(intervalMs, GameObjects.SignalProperty.Continuous);
            trigger.OnTimeoutEvent += () => this.Blink();
            trigger.Start();
            Colliders[0].Color = Color.MonoGameOrange;
        }

        public BlinkingBlock(Vector2 tilePosition) : this((int)tilePosition.X, (int)tilePosition.Y) { }
        
        private void Blink()
        {
            if (!Broken)
                Break();
            else
                Activate();
        }

        private void Activate()
        {
            Broken = false;
            Colliders.ActivateAll();
            //Colliders[0].Color = Color.MonoGameOrange;
        }



        public void Break()
        {
            IsEnableToPlayer = false;
            Broken = true;
            Colliders.DeactivateAll();
            Globals.SceneManager.GetStorage().UpdateMovingBlocksState();
            //Colliders[0].Color = Color.MonoGameOrange * 0.2f;
        }
    }
}
