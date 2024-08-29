using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public static class ResourceManager
{
    public static SpriteFont Font;

    public static Texture2D SpriteSheet;
    public static Texture2D SpriteSheet2;
    public static Texture2D SpriteParticle;
    public static Texture2D SpriteSlime;

    public static SoundEffect SoundDeath;
    public static SoundEffect SoundClick;

    public static void LoadContent(ContentManager content)
    {
        Font = content.Load<SpriteFont>("BasicFont");

        SpriteSheet = content.Load<Texture2D>("Sprites/TrexSpritesheet");
        SpriteSheet2 = content.Load<Texture2D>("Sprites/PlayerSpritesheet");
        SpriteParticle = content.Load<Texture2D>("Sprites/Smoke");
        SpriteSlime = content.Load<Texture2D>("Sprites/slime_split");

        SoundDeath = content.Load<SoundEffect>("Sounds/DeathSound");
        SoundClick = content.Load<SoundEffect>("Sounds/Click");

    }
}
