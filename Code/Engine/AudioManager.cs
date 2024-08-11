using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class AudioManager
{
    public ContentManager Content;
    private SoundEffectInstance _deathSound;

    public void Initialize()
    {
        _deathSound = Content.Load<SoundEffect>("Sounds/DeathSound").CreateInstance();
    }

    public void PlayDeathSound()
    {
        _deathSound.Volume = 0.7f;
        _deathSound.Play();
    }
}
