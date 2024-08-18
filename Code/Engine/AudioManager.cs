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
    private Dictionary<string, SoundEffectInstance> _soundEffects;
    private Dictionary<string, int> _playingSounds;

    public void Initialize()
    {
        //var deathSound = Content.Load<SoundEffect>("Sounds/DeathSound").CreateInstance();
        //var clickSound = Content.Load<SoundEffect>("Sounds/Click").CreateInstance();
        _soundEffects = new Dictionary<string, SoundEffectInstance>
        {
            { "DeathSound", ResourceManager.SoundDeath.CreateInstance() },
            { "Click", ResourceManager.SoundClick.CreateInstance() }
        };
        _playingSounds = new Dictionary<string, int>()
        {
            { "DeathSound", 0 },
            { "Click", 0 }
        };
    }

    public void PlaySound(string name, float volume = 1, float pitch = 0)
    {
        var instance = _soundEffects[name];
        if (_playingSounds[name] > 3)
        {
            instance.Stop();
            _playingSounds[name]--;
        }
        instance.Volume = volume;
        instance.Pitch = pitch;
        instance.Play();
        _playingSounds[name]++;
    }
}
