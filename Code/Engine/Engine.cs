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
using nameless.Interfaces;

namespace nameless.Engine;

public class Engine : Game
{
    private int _windowWidth;
    private int _windowHeight;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private PlayerModel _player;
    private KeyboardInputController _inputController;
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
        EntityType.ParseEntityTypeEnum();
        Globals.Engine = this;
        Globals.UIManager = new UIManager();
        Globals.KeyboardInputController = new KeyboardInputController();
        Globals.AnimationManager = new AnimationManager();
        Globals.SceneManager = new SceneManager();
        Globals.Serializer = new Serialize.Serializer();
        Globals.AudioManager = new AudioManager();
        Globals.VisualEffectsManager = new VisualEffectsManager();

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
        _graphics.HardwareModeSwitch = false; // Убрать рамку окна
        _graphics.ApplyChanges();
        //HardReset();
        
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

        ResourceManager.LoadContent(Content);

        //Globals.AudioManager.Content = Content;
        Globals.AudioManager.Initialize();
        Globals.AnimationManager.Initialize();
         

        if (!Map.LoadMap() || Globals.Map[0,0] == null)
        {
            HardReset();
        }
        Globals.Inventory = new Inventory();

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
        //Globals.KeyboardInputController = new KeyboardInputController(_player);
        Globals.KeyboardInputController.SetPlayer();
        Globals.Inventory = new Inventory();
        //Globals.UIManager = new UIManager();
    }

    public void Restart()
    {
        //Globals.AudioManager.PlaySound("DeathSound", 0.7f);
        LoadCollisions();
        LoadScene();
        Globals.UIManager.PopupMessage("Restart");
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

        if (Globals.KeyboardInputController.IsJustPressed(Keys.R))
        {
            Restart();
            //return;
        }

        if (Globals.KeyboardInputController.IsJustPressed(Keys.L))
            HardReset();

        if (Globals.KeyboardInputController.IsPressed(Keys.F))
        {
            Globals.KeyboardInputController.ProcessControls(gameTime);
            return;
        }


        MouseInputController.ProcessControls();

        Globals.AnimationManager.Update(gameTime);
        Globals.VisualEffectsManager.Update(gameTime);

        Globals.KeyboardInputController.ProcessControls(gameTime);

        Globals.SceneManager.Update(gameTime);


        Globals.CollisionManager.Update(gameTime);
        Globals.TriggerManager.Update(gameTime);

        //here was Keyboard upd


        Globals.UIManager.Update(gameTime);

        base.Update(gameTime);


    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Globals.BackgroungColor);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: Globals.Camera.Transform);
        Globals.SceneManager.Draw(_spriteBatch);
        Globals.AnimationManager.Draw(_spriteBatch);
        Globals.VisualEffectsManager.Draw(_spriteBatch);
        Globals.CollisionManager.DrawCollisions(_spriteBatch);
        _spriteBatch.End();

        _spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: Globals.Camera.Transform);
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
        Globals.CopyFiles(Path.Combine("BaseLayout"), Path.Combine("Layout"), true);
        Globals.Serializer.Restart();
        Globals.UIManager.Minimaps.Clear();
        Globals.UIManager.Clear();
        var visitedSceneStorage = Scene.GetSceneStorage("0 0 Center").ConvertToEnum();
        Globals.UIManager.Minimaps.Add(new Minimap(Vector2.Zero, visitedSceneStorage));
        Map.LoadMap();
        var dict = new Dictionary<EntityTypeEnum, int>
        {
            { EntityTypeEnum.FragileBlock, 0 }
        };
        Globals.Serializer.SaveInventory(dict);
        Restart();
        Globals.UIManager.PopupMessage("Hard reset");
    }
}
