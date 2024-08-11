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
    private Dictionary<string, SoundEffect> _soundEffects;

    public void Initialize()
    {
        var deathSound = Content.Load<SoundEffect>("Sounds/DeathSound");
        var clickSound = Content.Load<SoundEffect>("Sounds/Click");
        _soundEffects = new Dictionary<string, SoundEffect>
        {
            { "DeathSound", deathSound },
            { "Click", clickSound }
        };
    }

    public void PlaySound(string name, float volume = 1)
    {
        var instance = _soundEffects[name].CreateInstance();
        instance.Volume = volume;
        instance.Play();
    }
}
