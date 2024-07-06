using System.Collections.Generic;
using nameless.Collisions;
using MonoGame.Extended.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Graphics;

namespace nameless;

public static class Globals
{
    public static GameTime GameTime;

    public static CollisionComponent CollisionComponent;

    public static List<Collider> Colliders = new List<Collider>();
    public static List<DynamicCollider> DynamicColliders = new List<DynamicCollider>();
    public static List<CharacterCollider> CharacterColliders = new List<CharacterCollider>();

    public static void Draw(Vector2 position, SpriteBatch spriteBatch, Sprite sprite)
    {
        var size = new Vector2(sprite.Width, sprite.Height);
        sprite.Draw(spriteBatch, position + Offset((int)size.X, (int)size.Y));
    }

    public static void Draw(Vector2 position, Vector2 size, SpriteBatch spriteBatch, SpriteAnimation animation)
    {
        animation.Draw(spriteBatch, position + Offset((int)size.X, (int)size.Y));
    }

    public static Vector2 Offset(int width, int height) => new Vector2(-width / 2, -height / 2);
}
