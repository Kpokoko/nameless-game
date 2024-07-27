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
using System.Collections.Generic;
using System.Timers;

namespace nameless.Entity;

public class PlayerModel : ICollider, IEntity, IKinematic, ISerializable
{
    public Vector2 TilePosition { get => Tile.GetPosInTileCoordinats(Position); }

    private const float _time_effect = 1 / 60f;

    private const float RUN_ANIMATION_FRAME_LENGTH = 1 / 10f;
    private const float MIN_POS_Y = 900;

    private const float MIN_JUMP_HEIGHT = 4f / _time_effect;
    private const float JUMP_START_VELOCITY = -13f / _time_effect;
    private const float GRAVITY = 25f;
    private const float JUMP_GRAVITY = 70f;
    private const float CANCEL_JUMP_VELOCITY = -10f;
    private const float DROP_VELOCITY = 100f;

    private const float BOOST_HOR_VELOCITY = 0.6f / _time_effect;
    private const float MOVE_START_VELOCITY = 0.1f / _time_effect;
    private const float MAX_HOR_VELOCITY = 5f / _time_effect;
    private const float HOR_STOP = 0.7f / _time_effect;

    public const int RIGHT_SPRITE_POS_X = 848;
    public const int RIGHT_SPRITE_POS_Y = 0;
    public const int LEFT_SPRITE_POS_X = 980;
    public const int LEFT_SPRITE_POS_Y = 68;

    private const int RIGHT_RUN_FRAME_X = RIGHT_SPRITE_POS_X + SPRITE_WIDTH * 2;
    private const int LEFT_RUN_FRAME_X = LEFT_SPRITE_POS_X - SPRITE_WIDTH * 2;


    public const int SPRITE_WIDTH = 44;
    public const int SPRITE_HEIGHT = 52;

    private float _verticalVelocity;
    //private float _realVerticalVelocity;
    private float _horizontalVelocity;
    //private float _dropVelocity;

    public int DrawOrder => 10;

    private Sprite _currentSprite;
    private Sprite _leftSprite;
    private Sprite _rightSprite;

    private SpriteAnimation _runRightAnimation;
    private SpriteAnimation _runLeftAnimation;

    public Vector2 Position { get { return _position; } 
        set 
        { 
            _position = value;
        } }
    private Vector2 _position;
    public PlayerState State { get; set; }
    public Colliders Colliders { get; set; } = new();
    public Vector2 Velocity { get; private set; }
    public Stack<Action> Actions { get; set; } = new Stack<Action>();


    public PlayerModel() { }
    public SerializationInfo Info { get; set; } = new();

    public PlayerModel(Texture2D spriteSheet, Vector2? position = null) 
    {
        if (position == null)
            Position = new Vector2(176, 450);
        else
            Position = (Vector2)position;

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
        if (Globals.IsNoclipEnabled)
            Globals.CollisionManager.KinematicAccurateColliders.Remove(this.Colliders.colliders[0] as KinematicAccurateCollider);
        else if (!Globals.IsNoclipEnabled && !Globals.CollisionManager.KinematicAccurateColliders.Contains(this.Colliders.colliders[0] as KinematicAccurateCollider))
            Globals.CollisionManager.KinematicAccurateColliders.Add(this.Colliders.colliders[0] as KinematicAccurateCollider);
        var oldPos = Position;
        while (Actions.TryPop(out var action))
            action();

        if (!Globals.IsNoclipEnabled)
            ApplyGravity();

        if (_verticalVelocity >= 0)
        {
            State = PlayerState.Falling;
        }
        if (Position.Y >= MIN_POS_Y && !Globals.IsNoclipEnabled)
        {
            Globals.SceneManager.ReloadScene();
        }


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
        Velocity = new Vector2(_horizontalVelocity, _verticalVelocity) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position = Position + Velocity;

        //Velocity = Position - oldPos;
    }

    private void ApplyGravity()
    {
        _verticalVelocity += GRAVITY;
    }

    public void Up() => _verticalVelocity = -1000;
    public void StopVertical() => _verticalVelocity = 0;
    public void Down() => _verticalVelocity = 1000;

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

    public void TryJump()
    {
        if (State is PlayerState.Still)
        {
            Jump();
        }
    }

    private bool Jump()
    {
        State = PlayerState.Jumping;
        _verticalVelocity = JUMP_START_VELOCITY;
        return true;
    }

    public void CancelJump()
    {
        if (State is PlayerState.Jumping && _verticalVelocity < CANCEL_JUMP_VELOCITY)
            _verticalVelocity += JUMP_GRAVITY;
    }

    //public bool Drop()
    //{
    //    if (IsCollidedY)
    //    {
    //        return false;
    //    }
    //    _dropVelocity = DROP_VELOCITY;
    //    return true;
    //}

    public void MoveLeft()
    {
        if (Globals.IsNoclipEnabled)
        {
            _horizontalVelocity = -1000;
        }
        else if (_horizontalVelocity > 0)
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
        if (Globals.IsNoclipEnabled)
        {
            _horizontalVelocity = 1000;
        }
        else if (_horizontalVelocity < 0)
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
        if (Globals.IsNoclipEnabled)
        {
            _horizontalVelocity = 0;
            _verticalVelocity = 0;
        }
        else if (State is PlayerState.Still)
        {
            if (Math.Abs(_horizontalVelocity) > HOR_STOP)
            {
                if (_horizontalVelocity < 0)
                    _horizontalVelocity += HOR_STOP;
                else
                    _horizontalVelocity -= HOR_STOP;
            }
            else
                _horizontalVelocity = 0;
        }
        else
        {
            if (Math.Abs(_horizontalVelocity) > HOR_STOP)
            {
                if (_horizontalVelocity < 0)
                    _horizontalVelocity += HOR_STOP * 0.2f;
                else
                    _horizontalVelocity -= HOR_STOP * 0.2f;
            }
            else
                _horizontalVelocity = 0;
        }
    }

    public void PrepareSerializationInfo()
    {
        Info.TilePos = TilePosition;
        Info.TypeOfElement = this.GetType().Name;
    }
}
