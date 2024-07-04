using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
            TilePosition = new Vector2(x, y);
            SetCollision(this, 64, 64);
        }

        public Block() { }
        [XmlIgnore]
        public Vector2 Position { get; set; }
        public Vector2 TilePosition { get; set; }
        int IGameObject.DrawOrder => 1;

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        { }

        public void Update(GameTime gameTime)
        { }
    }
}
