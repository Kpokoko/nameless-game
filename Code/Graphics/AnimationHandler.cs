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
    public SpriteAnimation CurrentAnimation { get; set; }
    public Dictionary<AnimationType, SpriteAnimation> Animations { get; set; } = new();
    public Dictionary<AnimationType, int> AnimationPriority { get; set; } = new();
    public Queue<AnimationType> AnimationBuffer { get; set; } = new();

    public AnimationHandler(IKinematic entity)
    {
        _entity = entity;
        Globals.AnimationManager.Animations.Add(this);
    }

    public void AddAnimation(AnimationType type, SpriteAnimation animation, int priority = 0)
    {
        Animations[type] = animation;
        AnimationPriority[type] = priority;
    }

    public void Remove()
    {
        Globals.AnimationManager.Animations.Remove(this);
    }

    public void Update()
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        //Globals.Draw(_entity.Position, new Vector2(_currentSprite.Width, _currentSprite.Height), spriteBatch, _runLeftAnimation);
    }
}
