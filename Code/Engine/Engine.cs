global using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using nameless.Controls;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;
using System.Collections.Generic;
using nameless.Code.SceneManager;
using System;
using nameless.Code.Constructors;
using System.IO;
using System.Xml.Serialization;
using nameless.UI;

namespace nameless.Engine;

public class Engine : Game
{
    private const string ASSET_NAME_SPRITESHEET = "TrexSpritesheet";
    private const string ASSET_NAME_SPRITESHEET2 = "PlayerSpritesheet";

    private int _windowWidth;
    private int _windowHeight;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private PlayerModel _player;
    private PlayerInputController _inputController;
    private KeyboardState _previousKeybardState;

    private Texture2D _spriteSheet;

    public Engine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        LoadCollisions();
        Globals.Engine = this;
        Globals.UIManager = new UIManager();
        Globals.AnimationManager = new AnimationManager();
        Globals.SceneManager = new SceneManager();
        if (Globals.IsDeveloperModeEnabled)
        {
            Globals.Constructor = new DeveloperConstructor();
        }
        else
        {
            Globals.Constructor = new Constructor();
        }
        _windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferHeight = _windowHeight;
        _graphics.PreferredBackBufferWidth = _windowWidth;
        var camera = new Camera();
        camera.Zoom = _windowWidth / 23.0f / 64;
        Globals.Camera = camera;
        //_graphics.IsFullScreen = true; // Включить полноэкранный режим
        //_graphics.HardwareModeSwitch = false; // Убрать рамку окна
        _graphics.ApplyChanges();
        base.Initialize();
    }

    public void LoadCollisions()
    {
        var collisionComponent = () => new CollisionComponent(new RectangleF(0 - 100, 0 - 100, _windowWidth + 200, _windowHeight + 200));
        Globals.CollisionManager = new CollisionManager(collisionComponent());
        CollisionManager.TestCollisionComponent = collisionComponent();
        Globals.TriggerManager = new TriggerManager();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Globals.SpriteSheet = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET);
        Globals.SpriteSheet2 = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET2);

        Globals.UIManager.Font = Content.Load<SpriteFont>("BasicFont");

        Map.LoadMap();
        LoadScene();
    }


    private void LoadScene()
    {
        //Globals.Map[0, 2] = "down2";
        var serializer = new XmlSerializer(typeof(Vector2));
        using (var reader = new StreamReader(new FileStream("CurrentLoc.xml", FileMode.Open)))
        {
            var location = (Vector2)serializer.Deserialize(reader);
            Globals.SceneManager.LoadScene(location);
        }
        LoadUtilities();
        //var levelChanger = HitboxTrigger.CreateHitboxTrigger(TriggerType.SwitchScene, new Pivot(20, 12));

        //Globals.SceneManager.GetEntities().Add(levelChanger.Entity);
    }

    public void LoadUtilities()
    {
        _player = Globals.SceneManager.GetPlayer();
        Globals.InputController = new PlayerInputController(_player);
    }

    public void Restart()
    {
        LoadCollisions();
        LoadScene();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (Globals.GameTime == null)
            Globals.GameTime = gameTime;


        //Camera.Position = new Vector2(_player.Position.X, _player.Position.Y); // Пример следования за игроком
        Globals.Camera.Position = new Vector2(23*32, 13*32);
        Globals.Camera.Update();

        if (Keyboard.GetState().IsKeyDown(Keys.R))
        {
            Restart();
            return;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.L))
            HardReset();

        MouseInputController.ProcessControls();

        Globals.AnimationManager.Update(gameTime);

        Globals.SceneManager.Update(gameTime);

        Globals.CollisionManager.Update(gameTime);
        Globals.TriggerManager.Update(gameTime);


        Globals.InputController.ProcessControls(gameTime);

        Globals.UIManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Globals.BackgroungColor);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: Globals.Camera.Transform);
        Globals.SceneManager.Draw(_spriteBatch);
        Globals.CollisionManager.DrawCollisions(_spriteBatch);
        _spriteBatch.End();

        _spriteBatch.Begin(SpriteSortMode.FrontToBack);
        Globals.UIManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        Globals.SceneManager.SaveScene();
        base.OnExiting(sender, args);
    }

    private void HardReset()
    {
        Globals.CopyFiles("LevelsBaseCopy", "Levels", true);
        using (var writer = new StreamWriter(new FileStream("CurrentLoc.xml", FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(Vector2));
            var a = new Vector2(0, 0);
            serializer.Serialize(writer, a);
        }
        using (var writer = new StreamWriter(new FileStream("Map.xml", FileMode.Create)))
        {
            var serializer = new XmlSerializer(typeof(List<string>));
            var a = new List<string> { "0 0 Center" };
            serializer.Serialize(writer, a);
        }
        Globals.UIManager.Minimaps.Clear();
        Restart();
    }
}
