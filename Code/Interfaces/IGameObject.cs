using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Interfaces;

public interface IGameObject 
{
    int DrawOrder { get; }

    void Update(GameTime gameTime);

    void Draw(SpriteBatch spriteBatch);
}
