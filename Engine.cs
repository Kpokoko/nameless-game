using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using nameless.Controls;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;

namespace nameless
{
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

            _player.Update(gameTime);

            Globals.Update(gameTime);

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

            Globals.DrawCollisions(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}