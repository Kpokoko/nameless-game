using Microsoft.Xna.Framework.Graphics;
using nameless.Engine;
using nameless.Graphics;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.GameObjects;

public class Particle : IEntity
{
    public Vector2 Position { get; set; }

    public Vector2 TilePosition => throw new NotImplementedException();
    public Vector2 Velocity { get; set; }
    public Vector2 Acceleration { get; set; }
    public int DurationMs { get; set; }
    public int LifeTimeMs { get; set; }

    public Color Color { get; set; }
    public int DrawOrder => throw new NotImplementedException();
    public static Sprite Sprite { get; set; }

    public Particle(Vector2 position, Vector2 velocity, int durationMs, Color color, Vector2 acceleration = new Vector2())
    {
        Position = position;
        Velocity = velocity;
        DurationMs = durationMs;
        Acceleration = acceleration;
        Color = color;
        Globals.VisualEffectsManager.Particles.Add(this);
    }
    public Particle(Vector2 position, Vector2 velocity, int lifeTimeMs, Vector2 acceleration = new Vector2())
        : this(position, velocity, lifeTimeMs, Color.White, acceleration) { }


    public void Draw(SpriteBatch spriteBatch)
    {
        if (Sprite == null)
            Sprite = new Sprite(ResourceManager.SmokeSprite);
        Globals.Draw(Position, spriteBatch, Sprite, Color);
    }

    public void Remove()
    {
        Globals.VisualEffectsManager.Particles.Remove(this);
    }

    public virtual void Update(GameTime gameTime)
    {
        LifeTimeMs += gameTime.ElapsedGameTime.Milliseconds;
        if (DurationMs <= LifeTimeMs)
            Globals.VisualEffectsManager.ToRemove.Add(this);
        Position += Velocity;
        Velocity += Acceleration;
    }
}
