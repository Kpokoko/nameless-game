using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Collisions;
using nameless.Interfaces;

namespace nameless.Engine;

public class CollisionManager
{
    public CollisionManager(CollisionComponent collisionComponent) {CollisionComponent = collisionComponent;}

    public CollisionComponent CollisionComponent;
    public static CollisionComponent TestCollisionComponent;

    public static bool OnCollisionDisabled;
    public static Collider Processing;

    public List<Collider> Colliders = new List<Collider>();
    public List<Collider> PlatformColliders = new List<Collider>();
    public List<Collider> InactivePlatformColliders = new List<Collider>();
    public List<DynamicCollider> DynamicColliders = new List<DynamicCollider>();
    public List<KinematicCollider> KinematicColliders = new List<KinematicCollider>();
    public List<KinematicAccurateCollider> KinematicAccurateColliders = new List<KinematicAccurateCollider>();

    public void Update(GameTime gameTime)
    {
        if (Globals.IsConstructorModeEnabled)
            return;

        MeasurePlatforms();

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
        for (var i = 0; i < KinematicAccurateColliders.Count; i++)
        {
            Processing = KinematicAccurateColliders[i];
            KinematicAccurateColliders[i].UpdateCollision();
        }
    }

    public void MeasurePlatforms()
    {
        var player = ((RectangleF)((ICollider)Globals.SceneManager.GetPlayer()).Colliders[0].Bounds).Bottom;
        while (PlatformColliders.Count > 0)
        {
            var platform = PlatformColliders[PlatformColliders.Count - 1];
            var platformPos = ((RectangleF)platform.Bounds).Top;
            if (player > platformPos)
            {
                CollisionComponent.Remove(platform);
                PlatformColliders.RemoveAt(PlatformColliders.Count - 1);
                InactivePlatformColliders.Add(platform);
            }
            else break;
        }
        while (InactivePlatformColliders.Count > 0)
        {
            var platform = InactivePlatformColliders[InactivePlatformColliders.Count - 1];
            var platformPos = ((RectangleF)platform.Bounds).Top;
            if (player < platformPos + 1)
            {
                CollisionComponent.Insert(platform);
                InactivePlatformColliders.RemoveAt(InactivePlatformColliders.Count - 1);
                PlatformColliders.Add(platform);
            }
            else break;
        }
    }

    public void DrawCollisions(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Colliders.Count; i++)
            Colliders[i].DrawCollision(spriteBatch);
    }
}
