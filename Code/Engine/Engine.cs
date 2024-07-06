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

namespace nameless.Engine;

public class Engine : Game
{
    private const string ASSET_NAME_SPRITESHEET = "TrexSpritesheet";


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
        var block1 = new Block(0, 0);
        var block = new Block(1, 1);
        var a = new Block(2, 2);
        var b = new Block(10, 10);
        var v = new Block(12, 12);
        var c = new Block(4, 4);
        var x = new Block(5, 5);
        var s1 = new Block(6, 11);
        var s2 = new Block(6, 12);
        var s3 = new Block(6, 13);
        var s4 = new Block(6, 9);
        var s5 = new Block(6, 10);
        var s6 = new Block(5, 13);
        var s7 = new Block(4, 13);
        var s8 = new Block(3, 13);
        var s9 = new Block(2, 13);
        var bl = new Block(8, 13);
        var b23423 = new Block(5, 13);
        var blocks = new List<Block>
        {
            block1,
            block,
            a,
            b,
            v,
            c,
            x,
            s1,
            s2,
            s3,
            s4,
            s6,
            s7,
            s8,
            s9,
            bl,
            s5,
            b23423
        };
        var serializer = new Serializer();
        serializer.Serialize("startScene", blocks);
        var openedData = serializer.Deserialize<Block>("startScene");


        base.Initialize();

        _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
        _graphics.ApplyChanges();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _spriteSheet = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET);

        _player = new PlayerModel(_spriteSheet);

        _inputController = new InputController(_player);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (Globals.GameTime == null)
            Globals.GameTime = gameTime;

        _player.Update(gameTime);

        CollisionManager.Update(gameTime);

        KeyboardState keyboardState = Keyboard.GetState();

        _inputController.ProcessControls(gameTime);
        _previousKeybardState = keyboardState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();

        _player.Draw(_spriteBatch, gameTime);

        CollisionManager.DrawCollisions(_spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
