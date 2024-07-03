using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.AbstractClasses;
using nameless.Interfaces;
using nameless_game_branch.Tiles;

namespace nameless_game_branch.Entity
{
    public class Block : Collider, IEntity
    {
        public Block(int x, int y)
        {
            Position = new Tile(x, y).Position;
            SetCollision(this, 64, 64);
        }

        public Vector2 Position { get; private set; }

        int IGameObject.DrawOrder => 1;

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        { }

        public void Update(GameTime gameTime)
        { }
    }
}
