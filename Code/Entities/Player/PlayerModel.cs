using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Interfaces;
using nameless.Graphics;
using nameless.Collisions;
using MonoGame.Extended.Collisions;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;
using nameless.Serialize;
using nameless.Tiles;
using nameless.UI.Scenes;

namespace nameless.Entity;

public class PlayerModel : ICollider, IEntity, IKinematic, ISerializable
{
    public Vector2 TilePosition { get => Tile.GetPosInTileCoordinats(Position); }

    private const float RUN_ANIMATION_FRAME_LENGTH = 1 / 10f;
    private const float MIN_POS_Y = 900;

    private const float MIN_JUMP_HEIGHT = 40f;
    private const float JUMP_START_VELOCITY = -900f;
    private const float GRAVITY = 1600f;
    private const float CANCEL_JUMP_VELOCITY = -100f;
    private const float DROP_VELOCITY = 1000f;

    private const float BOOST_HOR_VELOCITY = 1f;
    private const float MOVE_START_VELOCITY = 2f;
    private const float MAX_HOR_VELOCITY = 8f;

    public const int RIGHT_SPRITE_POS_X = 848;
    public const int RIGHT_SPRITE_POS_Y = 0;
    public const int LEFT_SPRITE_POS_X = 980;
    public const int LEFT_SPRITE_POS_Y = 68;

    private const int RIGHT_RUN_FRAME_X = RIGHT_SPRITE_POS_X + SPRITE_WIDTH * 2;
    private const int LEFT_RUN_FRAME_X = LEFT_SPRITE_POS_X - SPRITE_WIDTH * 2;


    public const int SPRITE_WIDTH = 44;
    public const int SPRITE_HEIGHT = 52;

    private float _verticalVelocity;
    private float _realVerticalVelocity;
    private float _horizontalVelocity;
    private float _dropVelocity;

    public int DrawOrder => 10;

    private Sprite _currentSprite;
    private Sprite _leftSprite;
    private Sprite _rightSprite;

    private SpriteAnimation _runRightAnimation;
    private SpriteAnimation _runLeftAnimation;

    public Vector2 Position { get; set; }
    public PlayerState State { get; set; }
    [XmlIgnore]
    public Colliders Colliders { get; set; } = new();
    [XmlIgnore]
    public Vector2 Velocity { get; private set; }

    public bool IsCollidedX;
    public bool IsCollidedY;

    public PlayerModel() { }
    public SerializationInfo Info { get; set; } = new();

    public PlayerModel(Texture2D spriteSheet) 
    {
        Position = new Vector2(176, 450);//new Vector2(96, 550);
        _rightSprite = new Sprite(spriteSheet,
                RIGHT_SPRITE_POS_X,
                RIGHT_SPRITE_POS_Y,
                SPRITE_WIDTH,
                SPRITE_HEIGHT);
        _leftSprite = new Sprite(spriteSheet,
                LEFT_SPRITE_POS_X,
                LEFT_SPRITE_POS_Y,
                SPRITE_WIDTH,
                SPRITE_HEIGHT);
        _currentSprite = _rightSprite;

        _runRightAnimation = new SpriteAnimation();
        _runRightAnimation.AddFrame(
            new Sprite(
                spriteSheet,
                RIGHT_RUN_FRAME_X,
                RIGHT_SPRITE_POS_Y,
                SPRITE_WIDTH,
                SPRITE_HEIGHT), 0);
        _runRightAnimation.AddFrame(
            new Sprite(
                spriteSheet,
                RIGHT_RUN_FRAME_X + SPRITE_WIDTH,
                RIGHT_SPRITE_POS_Y,
                SPRITE_WIDTH,
                SPRITE_HEIGHT), RUN_ANIMATION_FRAME_LENGTH);
        _runRightAnimation.AddFrame(_runRightAnimation[0].Sprite, 2 * RUN_ANIMATION_FRAME_LENGTH);
        _runRightAnimation.Play();

        _runLeftAnimation = new SpriteAnimation();
        _runLeftAnimation.AddFrame(
            new Sprite(
                spriteSheet,
                LEFT_RUN_FRAME_X,
                LEFT_SPRITE_POS_Y,
                SPRITE_WIDTH,
                SPRITE_HEIGHT), 0);
        _runLeftAnimation.AddFrame(
            new Sprite(
                spriteSheet,
                LEFT_RUN_FRAME_X - SPRITE_WIDTH,
                LEFT_SPRITE_POS_Y,
                SPRITE_WIDTH,
                SPRITE_HEIGHT), RUN_ANIMATION_FRAME_LENGTH);
        _runLeftAnimation.AddFrame(_runLeftAnimation[0].Sprite, 2 * RUN_ANIMATION_FRAME_LENGTH);
        _runLeftAnimation.Play();

        State = PlayerState.Still;
        _verticalVelocity = 0;
        _horizontalVelocity = 0;

        Colliders.Add(new KinematicAccurateCollider(this, _currentSprite.Width,_currentSprite.Height));

        PrepareSerializationInfo();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_horizontalVelocity < 0)
        {
            Globals.Draw(Position, new Vector2(_currentSprite.Width, _currentSprite.Height), spriteBatch, _runLeftAnimation);
            _currentSprite = _leftSprite;
        } 
        else if (_horizontalVelocity > 0)
        {
            Globals.Draw(Position, new Vector2(_currentSprite.Width, _currentSprite.Height), spriteBatch, _runRightAnimation);
            _currentSprite = _rightSprite;
        }
        else
            Globals.Draw(Position, spriteBatch, _currentSprite);
    }

    public void Update(GameTime gameTime)
    {
        var oldPos = Position;

        IsCollidedX = false;
        IsCollidedY = false;
        if (State != PlayerState.Still || !IsCollidedY)
        {
            Position = new Vector2(Position.X, Position.Y + _verticalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds + _dropVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            _verticalVelocity += GRAVITY * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_verticalVelocity >= 0)
            {
                State = PlayerState.Falling;
            }
            if (Position.Y >= MIN_POS_Y)
            {
                State = PlayerState.Still;
                _verticalVelocity = 0;
            }
        }
        Position = new Vector2(Position.X + _horizontalVelocity, Position.Y);
        _dropVelocity = 0;
        //((DynamicCollider)this).Update();
        //Globals.CollisionComponent.Update(gameTime);
        if (_horizontalVelocity < 0)
        {
            _runLeftAnimation.Update(gameTime);
            _currentSprite = _leftSprite;
        }
        else if (_horizontalVelocity > 0)
        {
            _runRightAnimation.Update(gameTime);
            _currentSprite = _rightSprite;
        }

        Velocity = Position - oldPos;
    }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo) { }
    

    private bool _prevKeyboardState;
    public void OnCollision(params MyCollisionEventArgs[] collisionsInfo)
    {
        foreach (var collisionInfo in collisionsInfo)
        {
            //var collisionSide = Collider.CollisionToSide(collisionInfo);
            var collisionSide = collisionInfo.CollisionSide;
            //Position -= collisionInfo.PenetrationVector;
            if (collisionInfo.Other is Collider)
            {
                if (collisionSide is Side.Left || collisionSide is Side.Right)
                {
                    _horizontalVelocity = 0;
                }
                else if (collisionSide is Side.Bottom && State != PlayerState.Still && State != PlayerState.Jumping)
                {
                    _verticalVelocity = 0;
                    State = PlayerState.Still;
                }
                else if (collisionSide is Side.Top && State != PlayerState.Falling)
                {
                    _verticalVelocity = 0;
                    State = PlayerState.Falling;
                }
            }
        }
    }

    public bool BeginJump()
    {
        State = PlayerState.Jumping;
        _verticalVelocity = JUMP_START_VELOCITY;
        return true;
    }

    public bool CancelJump()
    {
        if (MIN_POS_Y - Position.Y < MIN_JUMP_HEIGHT)
        {
            return false;
        }
        _verticalVelocity = _verticalVelocity < CANCEL_JUMP_VELOCITY ? CANCEL_JUMP_VELOCITY : 0;

        return true;
    }

    public bool Drop()
    {
        if (IsCollidedY)
        {
            return false;
        }
        _dropVelocity = DROP_VELOCITY;
        return true;
    }

    public void MoveLeft()
    {

        if (_horizontalVelocity > 0)
        {
            _horizontalVelocity = 0;
        }
        else if (_horizontalVelocity == 0)
        {
            _horizontalVelocity -= MOVE_START_VELOCITY;
        }
        else if (Math.Abs(_horizontalVelocity) < MAX_HOR_VELOCITY)
        {
            _horizontalVelocity -= BOOST_HOR_VELOCITY;
        }
    }

    public void MoveRight()
    {
        if (_horizontalVelocity < 0)
        {
            _horizontalVelocity = 0;
        }
        else if (_horizontalVelocity == 0)
        {
            _horizontalVelocity += MOVE_START_VELOCITY;
        }
        else if (Math.Abs(_horizontalVelocity) < MAX_HOR_VELOCITY)
        {
            _horizontalVelocity += BOOST_HOR_VELOCITY;
        }
    }

    public void Stop()
    {
        if (Math.Abs(_horizontalVelocity) > 1)
        {
            if (_horizontalVelocity < 0)
                _horizontalVelocity += 1;
            else
                _horizontalVelocity -= 1;
        }
        else
            _horizontalVelocity = 0;
    }

    public void PrepareSerializationInfo()
    {
        Info.TilePos = TilePosition;
        Info.TypeOfElement = this.GetType().Name;
    }
}
