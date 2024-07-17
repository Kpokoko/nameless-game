using System.Collections.Generic;
using nameless.Collisions;
using MonoGame.Extended.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Graphics;
using nameless.GameObjects;
using nameless.Engine;
using nameless.Code.SceneManager;

namespace nameless;

public static class Globals
{
    public static GameTime GameTime;

    public static bool IsConstructorModeEnabled = false;

    public static Scene CurrentScene;

    public static Constructor Constructor = new();

    public static Texture2D SpriteSheet;

    public static CollisionManager CollisionManager;

    public static TriggerManager TriggerManager;

    public static UIManager UIManager;

    public static Color BackgroungColor { get { return IsConstructorModeEnabled ? SecondaryColor : PrimaryColor; } }
    public static Color PrimaryColor = Color.White;
    public static Color SecondaryColor = Color.LightGray;

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
