using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;
using nameless.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity
{
    public class EditorBlock : Block
    {
        public EditorBlock(int x, int y) : base(x, y)
        { 
            colliders[0].Color = Color.Violet;

            var trigger = new HitboxTrigger(this, 10, 10, ReactOnProperty.ReactOnEntityType, SignalProperty.OnceOnEveryContact);
            trigger.SetOffset(new Vector2(0, -42));
            trigger.Color = Color.SkyBlue;
            trigger.SetTriggerEntityTypes(typeof(PlayerModel));
            trigger.OnCollisionEvent += () => Globals.OnEditorBlock = true;
            trigger.OnCollisionExitEvent += () => Globals.OnEditorBlock = false;
            colliders.Add(trigger);
        }
    }
}
