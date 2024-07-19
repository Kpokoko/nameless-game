using System.Collections.Generic;
using nameless.Collisions;
using MonoGame.Extended.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Graphics;
using nameless.GameObjects;
using nameless.Engine;
using nameless.Code.SceneManager;
using Microsoft.Xna.Framework.Content;
using nameless.Code.Constructors;

namespace nameless;

public static class Globals
{

    public static GameTime GameTime;

    public static bool IsConstructorModeEnabled = false;

    //THIS BOOL SWITCHES DEV MODE
    public static bool IsDeveloperModeEnabled = false;

    public static bool OnEditorBlock = false;

    public static SceneManager SceneManager;

    //Invokes when devmode is disabled
    public static Constructor Constructor = new();

    //This is DeveloperConstructor, invokes when devmode is enabled
    public static DeveloperConstructor DevMode = new();

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
