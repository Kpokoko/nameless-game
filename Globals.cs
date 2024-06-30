using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.AbstractClasses;
using MonoGame.Extended.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nameless;

public static class Globals
{
    public static CollisionComponent CollisionComponent;
    public static List<DynamicCollider> DynamicColliders = new List<DynamicCollider>();
    public static List<Collider> Colliders = new List<Collider>();

    public static void Update(GameTime gameTime)
    {
        CollisionComponent.Update(gameTime);
        for (var i = 0; i < DynamicColliders.Count; i++)
            DynamicColliders[i].Update();
    }

    public static void DrawCollisions(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Colliders.Count; i++)
            Colliders[i].DrawCollision(spriteBatch);
    }
}
