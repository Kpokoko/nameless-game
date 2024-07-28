using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Graphics;

public abstract class AnimationHandler
{
    public abstract IEntity _entity { get; set; }
    public AnimationType CurrentAnimationType { get; set; }
    protected SpriteAnimation CurrentAnimation { get { return CurrentAnimationType is AnimationType.None ? null : Animations[CurrentAnimationType]; } }
    private Vector2 SpriteSize { get 
        { return new Vector2(CurrentAnimation.CurrentFrame.Sprite.Width, CurrentAnimation.CurrentFrame.Sprite.Height); } }
    public Dictionary<AnimationType, SpriteAnimation> Animations { get; set; } = new();
    public Dictionary<AnimationType, int> AnimationPriority { get; set; } = new();
    public Queue<AnimationType> AnimationBuffer { get; set; } = new();

    public AnimationHandler(IEntity entity)
    {
        _entity = entity;
        Globals.AnimationManager.AnimationHandlers.Add(this);
        AnimationPriority[AnimationType.None] = -1;
    }

    public void AddAnimation(AnimationType type, SpriteAnimation animation, int priority = 0)
    {
        Animations[type] = animation;
        AnimationPriority[type] = priority;
    }

    public void Remove()
    {
        Globals.AnimationManager.AnimationHandlers.Remove(this);
    }

    public void Update(GameTime gameTime)
    {
        GetAnimation();

        if (AnimationBuffer.Count > 0)
        {
            var anim = AnimationBuffer.Dequeue();
            if (AnimationPriority[anim] >= AnimationPriority[CurrentAnimationType] || !CurrentAnimation.IsPlaying)
            {
                if (CurrentAnimationType != anim)
                {
                    CurrentAnimation.Stop();
                    CurrentAnimationType = anim;
                    Animations[anim].Play();

                }
                if (CurrentAnimationType == anim)
                {
                    if (!CurrentAnimation.IsPlaying)
                        CurrentAnimation.Play();
                }
                
            }
        }
    }

    protected abstract void GetAnimation();

    public void Draw(SpriteBatch spriteBatch)
    {
        Globals.Draw(_entity.Position, SpriteSize, spriteBatch, CurrentAnimation);
    }
}
