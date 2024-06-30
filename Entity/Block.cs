using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.AbstractClasses;
using nameless.Interfaces;

namespace nameless_game_branch.Entity
{
    public class Block : Collider, IEntity
    {
        public Block()
        {
            SetCollision(this, 50, 50);
        }

        public Vector2 Position => new Vector2(300, 900);

        int IGameObject.DrawOrder => 1;

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
           ;
        }

        public void Update(GameTime gameTime)
        {
            ;
        }
    }
}
