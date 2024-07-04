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
    public static List<DynamicCollider> DynamicColliders = new List<DynamicCollider>();
    public static List<Collider> Colliders = new List<Collider>();
    public static List<CharacterCollider> CharacterColliders = new List<CharacterCollider>();

    public static void Update(GameTime gameTime)
    {
        for (var i = 0; i < DynamicColliders.Count; i++)
            DynamicColliders[i].Update();
        CollisionComponent.Update(gameTime);
        for (var i = 0; i < CharacterColliders.Count; i++)
            CharacterColliders[i].UpdateCollision();
    }

    public static void DrawCollisions(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Colliders.Count; i++)
            Colliders[i].DrawCollision(spriteBatch);
    }

    //public static void Draw(Vector2 position, Vector2 size, SpriteBatch spriteBatch, Sprite sprite = null, SpriteAnimation animation = null)
    //{
    //    if (sprite == null)
    //        animation.Draw(spriteBatch, position + Offset((int)size.X, (int)size.Y));
    //    else
    //        spriteBatch.Draw(sprite.Texture, position + Offset((int)size.X, (int)size.Y), Color.White);
    //}

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
