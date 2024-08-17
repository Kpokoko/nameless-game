using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Collisions;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class RayCaster : Block
{
    public Ray CastedRay;

    public RayCaster(int x, int y, int width, int height, float speed) : base(x, y)
    {
        CastedRay = new Ray(x, y, width, height, speed, this);
        //Colliders.Add(new Collider(this, 64, 64));
        Colliders[0].Color = Color.Pink;
    }

    public override void Update(GameTime gameTime)
    {
        CastedRay.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        CastedRay.Draw(spriteBatch);
    }
}
