using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using nameless.Graphics;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class AnimationManager
{
    public List<Graphics.AnimationHandler> AnimationHandlers = new List<Graphics.AnimationHandler>();
    public List<SpriteAnimation> SpriteAnimations= new List<SpriteAnimation>();

    private Dictionary<AnimationType, SpriteAnimation> _animations = new();
    private Dictionary<SpriteAnimation, Vector2> _animationPosition = new();
    private Dictionary<SpriteAnimation, float> _animationSpeed = new();
    public List<SpriteAnimation> ExternalSpriteAnimations = new List<SpriteAnimation>();
    public List<SpriteAnimation> ToRemove = new List<SpriteAnimation>();

    //private Dictionary<string, int> _playingSounds;

    public void Update(GameTime gameTime)
    {
        for (int i = 0; i < SpriteAnimations.Count; i++)
            SpriteAnimations[i].Update(gameTime);
        for (int i = 0; i < AnimationHandlers.Count; i++)
            AnimationHandlers[i].Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < ExternalSpriteAnimations.Count; i++)
        {
            var animation = ExternalSpriteAnimations[i];
            if (!animation.IsPlaying)
            {
                ToRemove.Add(animation);
            }
            Globals.Draw(_animationPosition[animation], SpriteSize(animation), spriteBatch, animation);
        }

        for (int i = 0; i < ToRemove.Count; i++)
        {
            ExternalSpriteAnimations.Remove(ToRemove[i]);
        }
        ToRemove.Clear();

    }

    public void Initialize()
    {
        var spriteSheet = ResourceManager.SpriteSheet2;
        var sprites = new SpriteSheet(spriteSheet, 44, 52);

        var right = sprites[0, 0];
        var left = sprites[1, 0];

        var jumpRight = new SpriteAnimation();
        jumpRight.AddFrame(sprites[0, 1], 0.08f);
        jumpRight.AddFrame(sprites[1, 1], 0.12f);
        jumpRight.AddFrame(sprites[2, 1], 0.18f);
        jumpRight.AddFrame(sprites[3, 1], 0.26f);
        jumpRight.AddFrame(sprites[2, 1], 0.32f);

        _animations[AnimationType.JumpRight] = jumpRight;
    }

    public void PlayAnimation(AnimationType animationType, Vector2 position, float speed = 1)
    {
        if (!_animations.ContainsKey(animationType))
            throw new ArgumentException();

        var instance = _animations[animationType].Clone();
        instance.SpeedMultiplyer = speed;

        ExternalSpriteAnimations.Add(instance);
        _animationPosition[instance] = position;

        instance.Play();
    }
    private Vector2 SpriteSize(SpriteAnimation animation) => new Vector2(animation.CurrentFrame.Sprite.Width, animation.CurrentFrame.Sprite.Height);
}
