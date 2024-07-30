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
using nameless.GameObjects;
using MonoGame.Extended.Collections;
using nameless.UI;
using nameless.Code.Constructors;
using System.IO;
using System.Xml.Serialization;

namespace nameless.Engine;

public class Engine : Game
{
    private const string ASSET_NAME_SPRITESHEET = "TrexSpritesheet";
    private const string ASSET_NAME_SPRITESHEET2 = "PlayerSpritesheet";


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
        base.Initialize();
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.ApplyChanges();
    }

    public void LoadCollisions()
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
        Globals.SpriteSheet2 = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET2);

        Globals.UIManager.Font = Content.Load<SpriteFont>("BasicFont");
        Map.LoadMap();
        LoadScene();
        //Globals.Map = new string[3][]
        //{
        //    new string[3],
        //    new string[3],
        //    new string[3],
        //};
        //Globals.Map[1][1] = "center";
        //Globals.Map[1][0] = "left";
        //Globals.Map[1][2] = "right";
        //Globals.Map[0][1] = "up";
        //Globals.Map[2][1] = "down";
        //using (var writer = new StreamWriter(new FileStream("Map.xml", FileMode.Create)))
        //{
        //    //var a = typeof(List<T>);
        //    var serializer = new XmlSerializer(typeof(string[][]));
        //    var a = Globals.Map;
        //    serializer.Serialize(writer, a);
        //}
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
        _inputController = new PlayerInputController(_player);
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

        //if (Keyboard.GetState().IsKeyDown(Keys.X))
        //{
        //    var mov = new MovingPlatform(2, 5, new Vector2(1, 0), 1.0f);
        //    var a = new List<ISerializable> { mov };
        //    var serializer = new Serializer();
        //    serializer.Serialize("afasfafsafsafsafsafsafsafsafsafsafsafsafsa", a);
        //}

        if (Keyboard.GetState().IsKeyDown(Keys.Q))
        {
            var serializer = new Serializer();
            serializer.Serialize(Globals.SceneManager.GetName(), Globals.SceneManager.GetEntities().Select(x => x as ISerializable).ToList());
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

        Globals.AnimationManager.Update(gameTime);

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

    protected override void OnExiting(object sender, EventArgs args)
    {
        Globals.SceneManager.SaveScene();
        base.OnExiting(sender, args);
    }
}
