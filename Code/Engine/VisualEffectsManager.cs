using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.GameObjects;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class VisualEffectsManager
{
    public List<Particle> Particles = new();
    public List<IEntity> ToRemove = new();

    public void Update(GameTime gameTime)
    {
        for (int i = 0; i < Particles.Count; i++)
        {
            Particles[i].Update(gameTime);
        }

        for (int i = 0; i < ToRemove.Count; i++)
        {
            ToRemove[i].Remove();
        }
        ToRemove.Clear();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Particles.Count; i++)
        {
            Particles[i].Draw(spriteBatch);
        }
    }

    public static Color Lerp(Color a, Color b, float t)
    {
        var argba = new[] { a.R, a.G, a.B, a.A };
        var brgba = new[] { b.R, b.G, b.B, b.A };
        var newrgba = argba.Select((c, i) => (int)Lerp(c, brgba[i], t)).ToArray();
        var c = new Color(newrgba[0], newrgba[1], newrgba[2], newrgba[3]);
        return c;
    }

    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}
