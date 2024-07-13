using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using nameless.Collisions;

namespace nameless.Engine;

public class CollisionManager
{
    public CollisionManager(CollisionComponent collisionComponent) {CollisionComponent = collisionComponent;}

    public CollisionComponent CollisionComponent;
    public static CollisionComponent TestCollisionComponent;

    public static bool OnCollisionDisabled;
    public static Collider Processing;

    public List<Collider> Colliders = new List<Collider>();
    public List<DynamicCollider> DynamicColliders = new List<DynamicCollider>();
    public List<KinematicCollider> KinematicColliders = new List<KinematicCollider>();

    public void Update(GameTime gameTime)
    {
        for (var i = 0; i < DynamicColliders.Count; i++)
            DynamicColliders[i].Update();
        OnCollisionDisabled = false;
        CollisionComponent.Update(gameTime);
        OnCollisionDisabled = true;
        for (var i = 0; i < KinematicColliders.Count; i++)
        {
            Processing = KinematicColliders[i];
            KinematicColliders[i].UpdateCollision();
        }
    }

    public void DrawCollisions(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Colliders.Count; i++)
            Colliders[i].DrawCollision(spriteBatch);
    }
}
