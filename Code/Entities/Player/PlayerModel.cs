﻿using System;
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
using nameless.Entity.Player;
using System.Linq;

namespace nameless.Entity;

public partial class PlayerModel : ICollider, IEntity, IKinematic, ISerializable
{
    public Vector2 TilePosition { get => Tile.GetPosInTileCoordinats(Position); }

    #region "Movement constants"
    private const float _time_effect = 1 / 60f;

    private const float RUN_ANIMATION_FRAME_LENGTH = 1 / 10f;
    private const float MIN_POS_Y = 1900;

    private const float MIN_JUMP_HEIGHT = 4f / _time_effect;
    private const float JUMP_VELOCITY = -13f / _time_effect;
    private const float GRAVITY = 0.4f / _time_effect;
    private const float JUMP_GRAVITY = 1.1f / _time_effect;
    private const float CANCEL_JUMP_VELOCITY = -10f;
    private const float DROP_VELOCITY = 100f;

    private const float HOR_ACCELERATION = 0.6f / _time_effect;
    private const float MAX_HOR_VELOCITY = 5f / _time_effect;
    private const float HOR_STOP_ACCELERATION = 0.7f / _time_effect;

    private const float AIR_HOR_ACCELERATION = 0.6f;
    private const float AIR_HOR_STOP_ACCELERATION = 0.15f;
    private const float AIR_HOR_TURN_ACCELERATION = 1.5f;
    #endregion 

    private float _verticalVelocity;
    //private float _realVerticalVelocity;
    private float _horizontalVelocity;
    //private float _dropVelocity;

    public int DrawOrder => 10;


    public Vector2 Position { get { return _position; } 
        set 
        { 
            _position = value;
        } }
    private Vector2 _position;
    public PlayerState State { get; set; }
    public bool CanJump { get; set; }
    public GameObjects.TimerTrigger Coyote { get; private set; }
    public GameObjects.TimerTrigger JumpBuffer { get; private set; }
    public Colliders Colliders { get; set; } = new();
    public Vector2 Velocity { get; private set; }
    public Vector2 InnerForce { get; private set; }
    public Vector2 OuterForce { get => PullingForce + PushingForce; 
        private set  { PushingForce = value; PullingForce = value; } }
    public Vector2 PushingForce { get; private set; }
    public Vector2 PullingForce { get; private set; }


    public Stack<Action> Actions { get; set; } = new Stack<Action>();
    public Side? PreviousCollisionSide { get; set; } = null;

    private PlayerAnimationHandler _animationHandler;

    public SerializationInfo Info { get; set; } = new();

    public PlayerModel(Texture2D spriteSheet, Vector2? position = null, Vector2 velocity = new Vector2(), PlayerState state = PlayerState.Falling) 
    {
        if (position == null)
            Position = new Vector2(176, 450);
        else
            Position = (Vector2)position;
        State = state;
        _verticalVelocity = velocity.Y;
        _horizontalVelocity = velocity.X;
        Colliders.Add(new KinematicAccurateCollider(this, 44,52));
        Colliders[0].Color = Color.Transparent;

        PrepareSerializationInfo();

        Coyote = new GameObjects.TimerTrigger(100, GameObjects.SignalProperty.Once);
        Coyote.OnTimeoutEvent += () => CanJump = false;

        JumpBuffer = new GameObjects.TimerTrigger(200, GameObjects.SignalProperty.Once);

        _animationHandler = new PlayerAnimationHandler(this);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _animationHandler.Draw(spriteBatch);
        //if (_horizontalVelocity < 0)
        //{
        //    Globals.Draw(Position, new Vector2(_currentSprite.Width, _currentSprite.Height), spriteBatch, _runLeftAnimation);
        //} 
        //else if (_horizontalVelocity > 0)
        //{
        //    Globals.Draw(Position, new Vector2(_currentSprite.Width, _currentSprite.Height), spriteBatch, _runRightAnimation);
        //}
        //else
        //    Globals.Draw(Position, spriteBatch, _currentSprite);
    }

    public void Update(GameTime gameTime)
    {
        if (State is PlayerState.Falling)
            StartCoyoteTimer();

        //var oldPos = Position;
        if (State is PlayerState.Still)
        {
            CanJump = true;
            if (JumpBuffer.IsRunning)
                TryJump();
        }

        OuterForce = Vector2.Zero;

        while (Actions.TryPop(out var action))
            action();

        ApplyGravity();

        if (_verticalVelocity > 0)
        {
            State = PlayerState.Falling;
        }
        if (Position.Y >= MIN_POS_Y && !Globals.IsNoclipEnabled)
        {
            Globals.SceneManager.ReloadScene();
        }

        InnerForce = new Vector2(_horizontalVelocity, _verticalVelocity) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Velocity = InnerForce;// + OuterForce;
        Position = Position + InnerForce + PullingForce;// + OuterForce;

        //Velocity = Position - oldPos;
    }

    private void StartCoyoteTimer()
    {
        if (CanJump && !Coyote.IsRunning)
        {
            Coyote.Reset();
            Coyote.Start();
        }
    }

    private void StartJumpBufferTimer()
    {
        JumpBuffer.Reset();
        JumpBuffer.Start();
    }

    public void ApplyGravity()
    {
        if (Globals.IsNoclipEnabled) return;
        _verticalVelocity += GRAVITY;
    }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo) { }
    
    public void OnCollision(params MyCollisionEventArgs[] collisionsInfo)
    {
        IsOppositeSides(collisionsInfo);

        foreach (var collisionInfo in collisionsInfo)
        {
            var collisionSide = collisionInfo.CollisionSide;

            if (((Collider)collisionInfo.Other).Entity is MovingPlatform)
            {
                var platform = (MovingPlatform)(((Collider)collisionInfo.Other).Entity);
                if (collisionSide is Side.Bottom)
                    Actions.Push(() => Pull(platform.Velocity));
                else
                    Actions.Push(() => Push(platform.Velocity));
            }

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

    private void IsOppositeSides(MyCollisionEventArgs[] collisionsInfo)
    {
        var sides = collisionsInfo.Select(i => i.CollisionSide);
        if ((sides.Contains(Side.Top) && sides.Contains(Side.Bottom)))
            Globals.Engine.Restart();
        if ((sides.Contains(Side.Right) && sides.Contains(Side.Left)))
            Globals.Engine.Restart();
        //if (sides.Contains(Side.Left))
        //{
        //    if (PreviousCollisionSide != null && (Side)PreviousCollisionSide == Side.Right)
        //        Globals.Engine.Restart();
        //    PreviousCollisionSide = Side.Left;
        //}

        //if (sides.Contains(Side.Right))
        //{
        //    if (PreviousCollisionSide != null && (Side)PreviousCollisionSide == Side.Left)
        //        Globals.Engine.Restart();
        //    PreviousCollisionSide = Side.Right;
        //}
    }

    public void Pull(Vector2 force)
    {
        PullingForce = force;
    }

    public void Push(Vector2 force)
    {
        PushingForce = force;
    }

    public void TryJump()
    {
        if (State is PlayerState.Still || (State is PlayerState.Falling && Coyote != null && Coyote.IsRunning && CanJump))
        {
            Jump();
        }
        else
        {
            StartJumpBufferTimer();
        }
    }

    private bool Jump()
    {
        CanJump = false;
        State = PlayerState.Jumping;
        _verticalVelocity = JUMP_VELOCITY;

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
        var multiplier = 1f;
        if (State is not PlayerState.Still)
            multiplier = AIR_HOR_ACCELERATION;

        if (_horizontalVelocity > 0)
        {
            _horizontalVelocity = State is PlayerState.Still ? 0 : _horizontalVelocity - HOR_ACCELERATION * multiplier * AIR_HOR_TURN_ACCELERATION;
            //_horizontalVelocity -= HOR_ACCELERATION * multiplier * 2;
        }
        else if (Math.Abs(_horizontalVelocity) < MAX_HOR_VELOCITY)
        {
            _horizontalVelocity -= HOR_ACCELERATION * multiplier;
        }
    }

    public void MoveRight()
    {
        var multiplier = 1f;
        if (State is not PlayerState.Still)
            multiplier = AIR_HOR_ACCELERATION;

        if (_horizontalVelocity < 0)
        {
            //_horizontalVelocity = 0;
            _horizontalVelocity = State is PlayerState.Still ? 0 : _horizontalVelocity + HOR_ACCELERATION * multiplier * AIR_HOR_TURN_ACCELERATION;
        }
        else if (Math.Abs(_horizontalVelocity) < MAX_HOR_VELOCITY)
        {
            _horizontalVelocity += HOR_ACCELERATION * multiplier;
        }
    }

    public void Stop()
    {
        var multiplier = 1f;
        if (State is not PlayerState.Still)
            multiplier = AIR_HOR_STOP_ACCELERATION;

        if (Math.Abs(_horizontalVelocity) > HOR_STOP_ACCELERATION)
        {
            if (_horizontalVelocity < 0)
                _horizontalVelocity += HOR_STOP_ACCELERATION * multiplier;
            else
                _horizontalVelocity -= HOR_STOP_ACCELERATION * multiplier;
        }
        else
            _horizontalVelocity = 0;

    }

    public void PrepareSerializationInfo()
    {
        Info.TilePos = TilePosition;
        Info.TypeOfElement = this.GetType().Name;
    }

    public void Remove()
    {
        _animationHandler.Remove();
    }
}
