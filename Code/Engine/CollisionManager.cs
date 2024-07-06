using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nameless.Engine;

internal class CollisionManager
{
    public static void Update(GameTime gameTime)
    {
        for (var i = 0; i < Globals.DynamicColliders.Count; i++)
            Globals.DynamicColliders[i].Update();
        Globals.CollisionComponent.Update(gameTime);
        for (var i = 0; i < Globals.CharacterColliders.Count; i++)
            Globals.CharacterColliders[i].UpdateCollision();
    }

    public static void DrawCollisions(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Globals.Colliders.Count; i++)
            Globals.Colliders[i].DrawCollision(spriteBatch);
    }
}
