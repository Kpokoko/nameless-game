global using Microsoft.Xna.Framework;
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
using nameless.GameObjects;
using MonoGame.Extended.Collections;
using nameless.UI;
using nameless.Code.Constructors;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics.Metrics;
using System.Text;

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
        Globals.Serializer = new Serialize.Serializer();
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
        //_graphics.IsFullScreen = true; // Включить полноэкранный режим
        //_graphics.HardwareModeSwitch = false; // Убрать рамку окна
        _graphics.ApplyChanges();
        base.Initialize();
    }

    public void LoadCollisions()
    {
        var collisionComponent = () => new CollisionComponent(new RectangleF(0 - 100, 0 - 100, _windowWidth + 200, _windowHeight + 200));
        Globals.CollisionManager = new CollisionManager(collisionComponent());
        Globals.CollisionManager.TestCollisionComponent = collisionComponent();
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
        Globals.Serializer.GetMapPos();
        LoadUtilities();
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

        _spriteBatch.Begin(SpriteSortMode.FrontToBack);

        Globals.SceneManager.Draw(_spriteBatch);

        //_player.Draw(_spriteBatch, gameTime);

        Globals.CollisionManager.DrawCollisions(_spriteBatch);


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
        Globals.Serializer.Restart();
        Globals.UIManager.Minimaps.Clear();
        var visitedSceneStorage = Scene.GetSceneStorage("0 0 Center").ConvertToEnum();
        Globals.UIManager.Minimaps.Add(new Minimap(Vector2.Zero, 0, 0, visitedSceneStorage, Alignment.Center));
        Restart();
    }
}
