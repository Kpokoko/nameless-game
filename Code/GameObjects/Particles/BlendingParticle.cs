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

public class BlendingParticle : Particle
{
    public bool IsColorSet;
    public int BlendTimeStartMs { get; set; }
    public int BlendTimeFinishMs { get; set; }

    public Color PrimaryColor { get; set; }
    public Color SecondaryColor { get; set; }

    public BlendingParticle(Vector2 position, Vector2 velocity, int lifeTimeMs, Color color, Vector2 acceleration = new Vector2()) 
        : base(position, velocity, lifeTimeMs, color, acceleration)
    { }
    public BlendingParticle(Vector2 position, Vector2 velocity, int lifeTimeMs, Vector2 acceleration = new Vector2())
        : base(position, velocity, lifeTimeMs, Color.White, acceleration) { }

    public void SetSecondColor(Color color, int startBlend = 0, int finishBlend = 0)
    {
        IsColorSet = true;

        PrimaryColor = Color;
        SecondaryColor = color;
        if (finishBlend == 0)
        {
            BlendTimeFinishMs = DurationMs;
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsColorSet) { }
        else if (LifeTimeMs > BlendTimeStartMs)
        {
            float t = 1 - (BlendTimeFinishMs - LifeTimeMs) / (BlendTimeFinishMs - BlendTimeStartMs * 1f);
            Color = VisualEffectsManager.Lerp(PrimaryColor, SecondaryColor, t);
        }
        base.Update(gameTime);
    }
}
