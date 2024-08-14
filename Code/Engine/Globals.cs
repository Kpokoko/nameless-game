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
using nameless.Interfaces;
using System.IO;
using nameless.Controls;
using nameless.Serialize;
using nameless.Entity;

namespace nameless;

public static class Globals
{
    public static Inventory Inventory;
    public static Engine.Engine Engine;
    public static Camera Camera;

    public static SerializationInfo LastVisitedCheckpoint;
    public static bool CanActivateSave = false;

    public static Serializer Serializer;
    public static Map Map;

    public static GameTime GameTime;

    public static bool IsConstructorModeEnabled = false;

    //THIS BOOL SWITCHES DEV MODE
    public static bool IsDeveloperModeEnabled = true;
    public static bool IsNoclipEnabled = false;

    public static bool OnEditorBlock = false;

    public static KeyboardInputController KeyboardInputController;

    public static SceneManager SceneManager;
    public static AudioManager AudioManager;

    public static Constructor Constructor;

    public static CollisionManager CollisionManager;

    public static TriggerManager TriggerManager;

    public static UIManager UIManager;

    public static AnimationManager AnimationManager;

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

    public static string GetPath(string name)
    {
        return Path.Combine("..", "net6.0", name);
    }

    public static void CopyFiles(string source, string target, bool copyAllFiles)
    {
        var fullSource = GetPath(source);
        var fullTarget = GetPath(target);
        if (!Path.HasExtension(fullTarget) && !Directory.Exists(fullTarget))
        {
            Directory.CreateDirectory(fullTarget);
        }

        if (copyAllFiles)
        {
            var sourceData = Directory.GetFiles(fullSource);
            foreach (var file in sourceData)
            {
                var name = Path.GetFileName(file);
                var targetFile = Path.Combine(fullTarget, name);
                File.Copy(file, targetFile, true);
            }
        }
        else
        {
            File.Copy(fullSource, fullTarget, true);
        }
    }
}
