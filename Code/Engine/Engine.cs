using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using nameless.Controls;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;
using nameless.Collisions;
using nameless.Serialize;
using System.Collections.Generic;
using nameless.Interfaces;
using System.Linq;
using nameless.Code.SceneManager;
using System;
using System.ComponentModel.Design;
using nameless.Entities.Blocks;
using nameless.GameObjects;
using MonoGame.Extended.Collections;
using nameless.UI;

namespace nameless.Engine;

public class Engine : Game
{
    private const string ASSET_NAME_SPRITESHEET = "TrexSpritesheet";

    public const int WINDOW_WIDTH = 1920;
    public const int WINDOW_HEIGHT = 1200;

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
        Globals.UIManager = new UIManager();
        Globals.SceneManager = new SceneManager();
        base.Initialize();
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.ApplyChanges();
    }

    private void LoadCollisions()
    {
        var collisionComponent = () => new CollisionComponent(new RectangleF(0 - 100, 0 - 100, WINDOW_WIDTH + 100, WINDOW_HEIGHT + 100));
        Globals.CollisionManager = new CollisionManager(collisionComponent());
        CollisionManager.TestCollisionComponent = collisionComponent();
        Globals.TriggerManager = new TriggerManager();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Globals.SpriteSheet = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET);
        Globals.UIManager.Font = Content.Load<SpriteFont>("BasicFont");
        LoadScene();
    }

    private void LoadScene()
    {
        Globals.SceneManager.LoadScene("startScene");
        _player = Globals.SceneManager.GetPlayer();
        _inputController = new PlayerInputController(_player);

        var levelChanger = new HitboxTrigger(new Pivot(20, 12), 10, 64, ReactOnProperty.ReactOnEntityType, Collisions.SignalProperty.OnceOnEveryContact);
        levelChanger.SetTriggerEntityTypes(typeof(PlayerModel));
        levelChanger.Color = Color.SkyBlue;
        levelChanger.SetOffset(new Vector2(30, 0));
        levelChanger.OnCollisionEvent += () =>
        {
            var serializer = new Serializer();
            var entities = Globals.SceneManager.GetEntities();
            serializer.Serialize(Globals.SceneManager.GetName(), entities.Select(x => x as ISerializable).ToList());
            Restart();
        };
    }

    private void Restart()
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

        if (Keyboard.GetState().IsKeyDown(Keys.Q))
        {
            var serializer = new Serializer();
            serializer.Serialize("startScene", Globals.SceneManager.GetEntities().Select(x => x as ISerializable).ToList());
        }

        if (Keyboard.GetState().IsKeyDown(Keys.R))
        {
            Restart();
            //Globals.SceneManager.ReloadScene();
            //_player = Globals.SceneManager.GetPlayer();
            //_inputController = new PlayerInputController(_player);
            return;
        }

        MouseInputController.ProcessControls();

        Globals.SceneManager.Update(gameTime);

        //_player.Update(gameTime);

        Globals.CollisionManager.Update(gameTime);
        Globals.TriggerManager.Update(gameTime);

        KeyboardState keyboardState = Keyboard.GetState();

        _inputController.ProcessControls(gameTime);
        _previousKeybardState = keyboardState;

        Globals.UIManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Globals.BackgroungColor);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack);

        Globals.SceneManager.Draw(_spriteBatch);

        //_player.Draw(_spriteBatch, gameTime);

        Globals.CollisionManager.DrawCollisions(_spriteBatch);

        Globals.UIManager.Draw(_spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
