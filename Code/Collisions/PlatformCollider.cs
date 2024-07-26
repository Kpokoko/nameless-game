using Microsoft.Xna.Framework;
using MonoGame.Extended;
using nameless.Collisions;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.Collisions
{
    public class PlatformCollider : Collider
    {
        public PlatformCollider(ICollider entity, int width, int height) : base(entity, width, height)
        {
            Globals.CollisionManager.PlatformColliders.Add(this);
        }


        public void Update()
        {
            var player = Globals.SceneManager.GetPlayer();
            var characterBottom = ((RectangleF)player.Colliders[0].Bounds).Bottom;
            if (characterBottom > this.Position.Y)
                Globals.CollisionManager.CollisionComponent.Remove(this);
            else if (characterBottom - 1 < ((RectangleF)this.Bounds).Top)
                Globals.CollisionManager.CollisionComponent.Insert(this);
        }

        public override void RemoveCollider()
        {
            Globals.CollisionManager.PlatformColliders.Remove(this);
            base.RemoveCollider();
        }
    }
}
