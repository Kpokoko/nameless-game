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
using nameless.Code.Entities;
using System.ComponentModel.Design;
using nameless.Entities.Blocks;

namespace nameless.Engine;

public class Engine : Game
{
    private const string ASSET_NAME_SPRITESHEET = "TrexSpritesheet";

    private Scene _currentScene;
    public const int WINDOW_WIDTH = 1920;
    public const int WINDOW_HEIGHT = 1200;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private PlayerModel _player;
    private InputController _inputController;
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
        var collisionComponent = new CollisionComponent(new RectangleF(0 - 100, 0 - 100, WINDOW_WIDTH + 100, WINDOW_HEIGHT + 100));
        Globals.CollisionComponent = collisionComponent;
        var blocks = new List<Block>();
        //for (var i = 0; i < 14; i++)
        //{
        //    for (var j = 0; j < 21; j++)
        //    {
        //        if (i == 13)
        //            blocks.Add(new Block(j, i));
        //        if (j == 0)
        //            blocks.Add(new Block(j, i));
        //        if (j == 20)
        //            blocks.Add(new Block(j, i));
        //    }
        //}
        //blocks.Add(new Block(3, 11));
        //blocks.Add(new Block(5, 8));
        //
        //var serializer = new Serializer();
        //serializer.Serialize("startScene", new List<InventoryBlock> { new InventoryBlock(10, 10) });
        //serializer.Serialize("startScene", blocks);
        //var openedData = serializer.Deserialize<Block>("startScene");



        base.Initialize();

        _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
        _graphics.ApplyChanges();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Globals.SpriteSheet = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET);

        //var serializer = new Serializer();
        //_player = new PlayerModel(_spriteSheet);
        //serializer.Serialize("startScene", new List<PlayerModel> { _player });
        _currentScene = new Scene("startScene", Content.RootDirectory);
        _player = _currentScene._entities.Where(item => item is PlayerModel).First() as PlayerModel;
        _inputController = new InputController(_player);
        var trigger = new TriggerHitbox(new Pivot(17,12), 80, 80, ReactOnProperty.ReactOnEntityType,SignalProperty.OnceOnEveryContact);
        trigger.SetTriggerEntityTypes(typeof(PlayerModel));
        trigger.OnCollisionEvent += () => _player.Position = new Vector2(_player.Position.X,_player.Position.Y-60);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (Globals.GameTime == null)
            Globals.GameTime = gameTime;

        _currentScene.Update(gameTime);

        //_player.Update(gameTime);

        CollisionManager.Update(gameTime);
        TriggerManager.Update(gameTime);

        KeyboardState keyboardState = Keyboard.GetState();

        _inputController.ProcessControls(gameTime);
        _previousKeybardState = keyboardState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();

        _currentScene.Draw(_spriteBatch, gameTime);

        //_player.Draw(_spriteBatch, gameTime);

        CollisionManager.DrawCollisions(_spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
