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

    private Vector2 lastNonZeroVelocity;
    private PlayerState previousState = PlayerState.Still;

    public PlayerAnimationHandler(PlayerModel entity) : base(entity)
    {
        var spriteSheet = Globals.SpriteSheet2;
        CurrentAnimationType = AnimationType.MoveRight;
        var sprites = new SpriteSheet(spriteSheet, 44, 52);

        var right = sprites[0, 0];
        var left = sprites[1, 0];

        var runRight = new SpriteAnimation();
        runRight.AddFrame(right, 0.07f);
        runRight.AddFrame(sprites[2, 0], 0.14f);

        var runLeft = new SpriteAnimation();
        runLeft.AddFrame(left, 0.07f);
        runLeft.AddFrame(sprites[3, 0], 0.14f);

        var idleRight = new SpriteAnimation();
        idleRight.AddFrame(right,0);

        var idleLeft = new SpriteAnimation();
        idleLeft.AddFrame(left, 0);

        var jumpRight = new SpriteAnimation();
        jumpRight.AddFrame(sprites[0, 1], 0.08f);
        jumpRight.AddFrame(sprites[1, 1], 0.12f);
        jumpRight.AddFrame(sprites[2, 1], 0.18f);
        jumpRight.AddFrame(sprites[3, 1], 0.26f);
        jumpRight.AddFrame(sprites[2, 1], 0.32f);

        var jumpLeft = new SpriteAnimation();
        jumpLeft.AddFrame(sprites[0, 2], 0.08f);
        jumpLeft.AddFrame(sprites[1, 2], 0.12f);
        jumpLeft.AddFrame(sprites[2, 2], 0.18f);
        jumpLeft.AddFrame(sprites[3, 2], 0.26f);
        jumpLeft.AddFrame(sprites[2, 2], 0.32f);

        var fallingRight = new SpriteAnimation();
        fallingRight.AddFrame(sprites[1, 1], 0.1f);

        var fallingLeft = new SpriteAnimation();
        fallingLeft.AddFrame(sprites[1, 2], 0.1f);

        var landingRight = new SpriteAnimation();
        landingRight.AddFrame(right, 0.05f);

        var landingLeft = new SpriteAnimation();
        landingLeft.AddFrame(left, 0.05f);


        AddAnimation(AnimationType.MoveRight, runRight);
        AddAnimation(AnimationType.MoveLeft, runLeft);
        AddAnimation(AnimationType.IdleRight, idleRight);
        AddAnimation(AnimationType.IdleLeft, idleLeft);
        AddAnimation(AnimationType.JumpRight, jumpRight, 2);
        AddAnimation(AnimationType.JumpLeft, jumpLeft, 2);
        AddAnimation(AnimationType.FallingRight, fallingRight, 1);
        AddAnimation(AnimationType.FallingLeft, fallingLeft, 1);
        AddAnimation(AnimationType.LandingRight, landingRight, 3);
        AddAnimation(AnimationType.LandingLeft, landingLeft, 3);


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
        if (player.State is PlayerState.Still && previousState is not PlayerState.Still)
        {
            if (lastNonZeroVelocity.X > 0)
                AnimationBuffer.Enqueue(AnimationType.LandingRight);
            else
                AnimationBuffer.Enqueue(AnimationType.LandingLeft);
        }
        else if (player.State is PlayerState.Jumping && previousState is not PlayerState.Jumping)
        {
            if (lastNonZeroVelocity.X > 0) 
                AnimationBuffer.Enqueue(AnimationType.JumpRight);
            else
                AnimationBuffer.Enqueue(AnimationType.JumpLeft);
        }
        else if (player.State is PlayerState.Falling || player.State is PlayerState.Jumping)
        {
            if (lastNonZeroVelocity.X > 0)
                AnimationBuffer.Enqueue(AnimationType.FallingRight);
            else
                AnimationBuffer.Enqueue(AnimationType.FallingLeft);
        }

        previousState = player.State;
    }
}
