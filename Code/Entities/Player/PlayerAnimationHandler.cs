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
        jumpRight.AddFrame(sprites[2, 1], 0.04f);
        jumpRight.AddFrame(sprites[1, 1], 0.08f);
        jumpRight.AddFrame(sprites[0, 1], 0.13f);
        jumpRight.AddFrame(sprites[1, 1], 0.18f);
        jumpRight.AddFrame(sprites[2, 1], 0.25f);
        jumpRight.AddFrame(sprites[3, 1], 0.32f);
        jumpRight.AddFrame(sprites[2, 1], 0.46f);

        var jumpLeft = new SpriteAnimation();
        jumpLeft.AddFrame(sprites[2, 2], 0.04f);
        jumpLeft.AddFrame(sprites[1, 2], 0.08f);
        jumpLeft.AddFrame(sprites[0, 2], 0.13f);
        jumpLeft.AddFrame(sprites[1, 2], 0.18f);
        jumpLeft.AddFrame(sprites[2, 2], 0.25f);
        jumpLeft.AddFrame(sprites[3, 2], 0.32f);
        jumpLeft.AddFrame(sprites[2, 2], 0.46f);


        AddAnimation(AnimationType.MoveRight, runRight);
        AddAnimation(AnimationType.MoveLeft, runLeft);
        AddAnimation(AnimationType.IdleRight, idleRight);
        AddAnimation(AnimationType.IdleLeft, idleLeft);
        AddAnimation(AnimationType.JumpRight, jumpRight, 1);
        AddAnimation(AnimationType.JumpLeft, jumpLeft, 1);


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
            if (lastNonZeroVelocity.X > 0) 
                AnimationBuffer.Enqueue(AnimationType.JumpRight);
            else
                AnimationBuffer.Enqueue(AnimationType.JumpLeft);
        }

        previousState = player.State;
    }
}
