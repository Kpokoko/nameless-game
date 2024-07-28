using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using nameless.Graphics;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity.Player;

public class PlayerAnimationHandler : AnimationHandler
{
    public override IEntity _entity { get; set ; }

    private const float RUN_ANIMATION_FRAME_LENGTH = 1 / 10f;


    public const int RIGHT_SPRITE_POS_X = 848;
    public const int RIGHT_SPRITE_POS_Y = 0;
    public const int LEFT_SPRITE_POS_X = 980;
    public const int LEFT_SPRITE_POS_Y = 68;

    private const int RIGHT_RUN_FRAME_X = RIGHT_SPRITE_POS_X + SPRITE_WIDTH * 2;
    private const int LEFT_RUN_FRAME_X = LEFT_SPRITE_POS_X - SPRITE_WIDTH * 2;


    public const int SPRITE_WIDTH = 44;
    public const int SPRITE_HEIGHT = 52;

    private Vector2 lastNonZeroVelocity;
    private PlayerState previousState = PlayerState.Still;

    public PlayerAnimationHandler(PlayerModel entity) : base(entity)
    {
        var spriteSheet = Globals.SpriteSheet;
        CurrentAnimationType = AnimationType.MoveRight;

        var _rightSprite = new Sprite(spriteSheet,
        RIGHT_SPRITE_POS_X,
        RIGHT_SPRITE_POS_Y,
        SPRITE_WIDTH,
        SPRITE_HEIGHT);
        var _leftSprite = new Sprite(spriteSheet,
                LEFT_SPRITE_POS_X,
                LEFT_SPRITE_POS_Y,
                SPRITE_WIDTH,
                SPRITE_HEIGHT);
        var _currentSprite = _rightSprite;

        var _runRightAnimation = new SpriteAnimation();
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

        var _runLeftAnimation = new SpriteAnimation();
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

        var idleRightAnimation = new SpriteAnimation();
        idleRightAnimation.AddFrame(_rightSprite,0);

        var idleLeftAnimation = new SpriteAnimation();
        idleLeftAnimation.AddFrame(_leftSprite, 0);

        var jumpAnimation = new SpriteAnimation();
        jumpAnimation.AddFrame(new Sprite(spriteSheet,
        RIGHT_SPRITE_POS_X,
        RIGHT_SPRITE_POS_Y + 10,
        SPRITE_WIDTH,
        SPRITE_HEIGHT),0);
        jumpAnimation.AddFrame(new Sprite(spriteSheet,
        RIGHT_SPRITE_POS_X,
        RIGHT_SPRITE_POS_Y + 20,
        SPRITE_WIDTH,
        SPRITE_HEIGHT), 0.08f);
        jumpAnimation.AddFrame(new Sprite(spriteSheet,
        RIGHT_SPRITE_POS_X,
        RIGHT_SPRITE_POS_Y + 30,
        SPRITE_WIDTH,
        SPRITE_HEIGHT), 0.16f);
        jumpAnimation.AddFrame(new Sprite(spriteSheet,
        RIGHT_SPRITE_POS_X,
        RIGHT_SPRITE_POS_Y + 40,
        SPRITE_WIDTH,
        SPRITE_HEIGHT), 0.24f);
        jumpAnimation.AddFrame(new Sprite(spriteSheet,
        RIGHT_SPRITE_POS_X,
        RIGHT_SPRITE_POS_Y + 50,
        SPRITE_WIDTH,
        SPRITE_HEIGHT), 0.32f);
        jumpAnimation.AddFrame(new Sprite(spriteSheet,
        RIGHT_SPRITE_POS_X,
        RIGHT_SPRITE_POS_Y + 50,
        SPRITE_WIDTH,
        SPRITE_HEIGHT), 0.4f);

        AddAnimation(AnimationType.MoveRight, _runRightAnimation);
        AddAnimation(AnimationType.MoveLeft, _runLeftAnimation);
        AddAnimation(AnimationType.IdleRight, idleRightAnimation);
        AddAnimation(AnimationType.IdleLeft, idleLeftAnimation);
        AddAnimation(AnimationType.Jump, jumpAnimation, 1);

    }

    protected override void GetAnimation()
    {
        var player = (PlayerModel)_entity;
        var vel = player.InnerForce;
        if (vel.X != 0)
            lastNonZeroVelocity = vel;

        if (vel.X < 0)
        {
            AnimationBuffer.Enqueue(AnimationType.MoveLeft);
        }
        else if (vel.X > 0)
        {
            AnimationBuffer.Enqueue(AnimationType.MoveRight);
        }
        else
        {
            if (!CurrentAnimation.IsPlaying && lastNonZeroVelocity.X < 0)
                AnimationBuffer.Enqueue(AnimationType.IdleLeft);
            else if (!CurrentAnimation.IsPlaying && lastNonZeroVelocity.X > 0)
                AnimationBuffer.Enqueue(AnimationType.IdleRight);
        }
        if (player.State is PlayerState.Jumping && previousState is not PlayerState.Jumping)
        {
            AnimationBuffer.Enqueue(AnimationType.Jump);
        }

        previousState = player.State;
    }
}
