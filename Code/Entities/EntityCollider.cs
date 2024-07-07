using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Collisions;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace nameless.Code.Entities
{
    public class EntityCollider : IEntity
    {
        public EntityCollider() { }
        [XmlIgnore]
        public Vector2 Position { get; set; }
        public Vector2 TilePosition { get; set; }
        public Type Type { get => this.GetType(); }
        int IGameObject.DrawOrder => 1;

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
